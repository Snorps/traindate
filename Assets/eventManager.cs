using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public struct Decision
{
    public Effect type;
    public string data;
    public string display;
}

public enum Effect
{
    gotoEvent,
    skipEvent,
    addDialog
}


public class eventManager : MonoBehaviour
{

    static char NEWLINE = ';';
    static char ENDEVENT = '~';
    static char KEYSTART = '[';
    static char KEYEND = ']';


    // Base event class
    public class BaseEvent
    {

        // List of potential events to go to next
        public List<string> nextEventList;

        // bool to check if event has finished
        public bool End;


        // constructor/destructor
        public BaseEvent()
        {

        }

        ~BaseEvent()
        {

        }


        // virtual methods for inheritance
        //
        // begin is called when event starts
        public virtual void begin()
        {

        }

        // OnMouse Down is called when left mouse button is pressed
        public virtual void OnInput()
        {
              
        }

        // returns next event
        public virtual string nextEvent()
        {
            return nextEventList[0];
        }
          
    }
    
    // Audio event class
    public class AudioEvent : BaseEvent
    {

        
        private string file;
        private bool blocking;

        private GameObject Audio;

        // constructor/destructor
        public AudioEvent(string filePath, bool block = false)
        {

            // set attributes

            file = filePath;
            blocking = block;

            Audio = GameObject.FindGameObjectWithTag("AudioHandler");

            End = false;
        }

        ~AudioEvent()
        {

        }

        // begin is called when event starts
        public override void begin()
        {

            // play audio from file


            // check if event will block next event
            if (!blocking)
            {

                // if not blocking end the event
                End = true;
            }
        }

        // OnMouse Down is called when left mouse button is pressed
        public override void OnInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // end the event
                End = true;
            }
        }

        // returns next event
        public override string nextEvent()
        {
            return nextEventList[0];
        }

    }

    public class DecisionEvent : BaseEvent
    {

        // attributes
        private int currentSelect;
        private List<Decision> selection;

        GameObject UI;
        GameObject EManager;

        public DecisionEvent(List<Decision> newSelection)
        {
            selection = newSelection;

            UI = GameObject.FindGameObjectWithTag("Canvas");
            EManager = GameObject.FindGameObjectWithTag("EventManager");

            End = false;
        }

        ~DecisionEvent()
        {

        }

        public override void begin()
        {
            currentSelect = 0;
            // display the options
            //UI.GetComponent<UIhandler>().displaySelection(selection);
            //UI.GetComponent<UIhandler>().displayCursor(currentSelect);
        }

        public override void OnInput()
        {
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentSelect++;
                currentSelect %= selection.Count;

                //UI.GetComponent<UIhandler>().displayCursor(currentSelect);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Choose();
            }
        }

        private void Choose()
        {
            switch (selection[currentSelect].type)
            {
                case Effect.gotoEvent:
                    gotoEvent();
                    break;
                case Effect.skipEvent:
                    skipEvent();
                    break;
                case Effect.addDialog:
                    addDialog();
                    break;
            }
        }

        private void addDialog()
        {

        }

        private void skipEvent()
        {
            EManager.GetComponent<eventManager>().GotoNextEvent();
        }
        
        private void gotoEvent()
        {
            EManager.GetComponent<eventManager>().setNextEvent(selection[currentSelect].data);
            End = true;
        }

        public override string nextEvent()
        {
            
            return "";
        }

    }
    

    // Character Change event class
    public class CharacterChangeEvent : BaseEvent
    {

        // attributes
        private string file;
        private string charNum;
        private bool blocking;

        private GameObject UI;

        // constructor/destructor
        public CharacterChangeEvent(string filePath, string characterNum, bool block = false )
        {

            // set attributes

            file = filePath;
            charNum = characterNum;
            blocking = block;

            UI = GameObject.FindGameObjectWithTag("Canvas");

            End = false;
        }

        ~CharacterChangeEvent()
        {

        }


        // begin is called when event starts
        public override void begin()
        {

            // display character
            UI.GetComponent<UIhandler>().changeImageSprite(charNum, file);


            // check if event is blocking
            if (!blocking)
            {

                // end the event
                End = true;
            }

        }

        // OnMouse Down is called when left mouse button is pressed
        public override void OnInput()
        {
            if (Input.GetMouseButtonDown(0))
            {

                // end the event
                End = true;
            }
        }

        // returns next event
        public override string nextEvent()
        {
            return nextEventList[0];
        }

    }


    // dialog event class
    public class DialogEvent : BaseEvent
    {

        // List of dialog used
        private List<Dialog> dialog;

        // index of current dialog
        private int currentDialog;

        // pointers to UI/AUDIO objects
        private GameObject UI;
        private GameObject Audio;

        

        // constructor/destructor
        public DialogEvent(List<Dialog> newDialog)//, string nextEvent, GameObject setUI, GameObject setAudio )
        {

            // set current dialog to start
            currentDialog = 0;

            // set attributes
            dialog = newDialog;

            //nextEventList.Add(nextEvent);

            UI = GameObject.FindGameObjectWithTag("Canvas");
            Audio = GameObject.FindGameObjectWithTag("Canvas");

            //UI = setUI;
            //Audio = setAudio;

            End = false;

        }


        // called when event starts
        public override void begin()
        {
            Debug.Log("begin is happening");
            UI.GetComponent<UIhandler>().changeText(dialog[currentDialog]);
            Debug.Log(dialog[currentDialog].name);
            // UI display first dialog

        }

        // when mouse button down
        public override void OnInput()
        {

            if (Input.GetMouseButtonDown(0))
            {
                // goto next dialog 
                currentDialog++;

                // check if at end of dialog
                if (currentDialog < dialog.Count)
                {

                    // if there is dialog display it

                    //Debug.Log("mouse down displaying new poop hehe got 'em");
                    UI.GetComponent<UIhandler>().changeText(dialog[currentDialog]);
                    Debug.Log(dialog[currentDialog].name);
                    // UI display dialog[currentDialog]
                }
                else
                {

                    // else end the event
                    End = true;
                }
            }
        }

        // returns location of next event
        public override string nextEvent()
        {
            
            return nextEventList[0];
        }

    }

    // index of which is the current event
    private int eventIndex;
    private BaseEvent currentEvent;
    private List<BaseEvent> eventList = new List<BaseEvent> ();

    private string nextEvent;

    void setNextEvent(string newEvent)
    {
        nextEvent = newEvent;
    }

    // Use this for initialisation
    void Start ()
    {

        // set default values
        eventIndex = 0;
        nextEvent = "Assets/Resources/Events/dialog1.event";
        currentEvent = null;
    }

    // Update is called once per frame
    void Update()
    {

        // check if there is a current event
        if (currentEvent != null)
        {

            
            // check for inputs

            currentEvent.OnInput();

            // check if event has ended
            if (currentEvent.End)
            {

                // change current event to the next one
                GotoNextEvent();
            }
        }
        else
        {

            // if there's no event try loading new events
            LoadEvent();
        }
    }

    private char readLine(StreamReader reader, ref string line, bool skipWhite = false)
    {

        char character = ENDEVENT;

        if (skipWhite)
        {
            character = skipWhiteSpace(reader);
        }

        if (character != NEWLINE && character != ENDEVENT)
        {

            line += character;

            while (reader.Peek() > -1)
            {

                if (character == NEWLINE || character == ENDEVENT)
                {
                    break;
                }
                else
                {
                    line += character;
                }

                character = (char)reader.Read();
            }
        }

        return character;
    }

    // create event types
    //
    // adds Dialog event to event list
    private void CreateDialogEvent(StreamReader reader)
    {

        // create variables for creating dialog event
        List<Dialog> DialogList = new List<Dialog>();

        string name = "";
        string dialog = "";
        char character;

        string buffer = "";

        // while file has data and not at end of event data
        while (reader.Peek() > -1)
        {

            // skip the whitespace
            character = skipWhiteSpace(reader);

            // if display setting
            if (character == KEYSTART)
            {

                buffer = "";

                while (reader.Peek() > -1)
                {

                    character = (char)reader.Read();

                    if (character == KEYEND)
                    {
                        break;
                    }
                    else
                    {
                        buffer += character;
                    }
                }

                switch (buffer.ToUpper())
                {
                    case "NAME":
                        name = "";

                        readLine(reader, ref name, true);
                        break;
                    default:
                        break;
                }

            }

            else if (character == NEWLINE)
            {

                DialogList.Add(new Dialog(name, dialog));
                dialog = "";
            }

            else if (character == ENDEVENT)
            {
                DialogList.Add(new Dialog(name, dialog));
                break;
            }

            else
            {

                // else add character to dialog string
                dialog += character;

                character = readLine(reader, ref dialog);

                if (character == ENDEVENT)
                {

                    DialogList.Add(new Dialog(name, dialog));
                    break;
                }
                else if (character == NEWLINE)
                {
                    DialogList.Add(new Dialog(name, dialog));
                    dialog = "";
                }

                /*
                // while not at end of current dialog
                while (reader.Peek() > -1)
                {

                    // read next character
                    character = (char)reader.Read();

                    // check for symbols
                    //
                    // if end of event symbol
                    if (character == '~')
                    {

                        //add current dialog to list then exit loop
                        DialogList.Add(new Dialog(name, dialog));
                        end = true;
                        break;
                    }

                    // if end of dialog box symbol
                    else if (character == '|')
                    {

                        // add current dialog to list then set current dialog to empty
                        DialogList.Add(new Dialog(name, dialog));
                        dialog = "";
                        break;
                    }

                    else
                    {
                        dialog += character;
                    }
                }
                */

            }
        }
        
      


        // create and add dialog event to the event list
        eventList.Add(new DialogEvent(DialogList));

    }


    // adds audio event to event list
    private void CreateAudioEvent(StreamReader reader)
    {

        // create variables for making audio event
        string filePath = "Assets/Resources/Audio/";

        bool blocking = false;
        bool blockCheck = false;

        char character;


        character = readLine(reader, ref filePath, true);


        if (character == NEWLINE)
        {
            blockCheck = true;
        }
        
        /*
        // while there's data to read
        while(reader.Peek() > -1)
        {

            // read next character
            character = (char)reader.Read();


            // if end of line symbol
            if (character == '|')
            {

                // do a blocking check and break out loop
                blockCheck = true;
                break;
            }

            // if end of event symbol
            else if (character == '~')
            {

                // break out loop
                blockCheck = false;
                break;
            }
        }
        */


        // if checking for blocking
        if (blockCheck)
        {

            // create buffer for getting data
            string buffer = "";

            readLine(reader, ref buffer, true);

            /*
            // while there's data to read
            while (reader.Peek() > -1)
            {

                // read next character
                character = (char)reader.Read();

                // if end of event symbol
                if (character == '~')
                {

                    // exit loop
                    break;
                }
                else
                {

                    // add character to buffer
                    buffer += character;
                }
            }
            */

            // if blocking is true
            if (buffer.ToUpper() == "TRUE")
            {

                // set blocking to true
                blocking = true;
            }

            // else don't block event
            else
            {
                blocking = false;
            }
        }


        // add new Audio event to list
        eventList.Add(new AudioEvent(filePath, blocking));

    }


    // function for skipping whitespace in file
    private char skipWhiteSpace(StreamReader reader)
    {

        // set character to default value
        char character = ENDEVENT;

        // while there's data to read
        while (reader.Peek() > -1)
        {

            // read next character
            character = (char)reader.Read();

            // if there's a "whitespace" character then continue loop
            if (character == ' ' || character == '\n' || character == '\t' || character == '\r')
            {
                continue;
            }

            // if no whitespace exit loop
            break;
        }

        // return last character read
        return character;
    }


    // adds Character Change event to event list
    private void CreateCharacterChangeEvent(StreamReader reader)
    {

        // create variables for making event
        string filePath = "Assets/Resources/Characters/";
        string charNum = "";
        bool blocking = false;

        char character;

        readLine(reader, ref charNum, true);

        /*
        // while there's data to read
        while (reader.Peek() > -1)
        {

            // read next character
            character = (char)reader.Read();


            // if next line symbol
            if (character == '|')
            {
                // exit loop
                break;
            }
            else
            {

                // add character to charNum
                charNum += character;
            }
            

        }

        */

        character = readLine(reader, ref filePath, true);

        bool blockCheck = false;

        if (character == NEWLINE)
        {
            blockCheck = true;
        }

        /*
        // while there's data to read
        while (reader.Peek() > -1)
        {

            // read next character
            character = (char)reader.Read();


            // if end of line character
            if (character == '|')
            {

                blockCheck = true;
                break;
            }

            // else if end of event character
            else if(character == '~')
            {
                break;
            }
            else
            {

                // add character to filePath
                filePath += character;
            }

        }
        */

        // if checking for block
        if (blockCheck)
        {

            // create buffer
            string buffer = "";

            readLine(reader, ref buffer, true);

            /*
            // while there's data to read
            while (reader.Peek() > -1)
            {

                // read next character
                character = (char)reader.Read();

                // if end of event character
                if (character == '~')
                {

                    // exit loop
                    break;
                }
                else
                {

                    // add character to buffer
                    buffer += character;
                }
            }
            */

            // check if event is blocking
            if (buffer.ToUpper() == "TRUE")
            {

                // if event is blocking then set blocking to true
                blocking = true;
            }
            else
            {

                // event isn't blocking
                blocking = false;
            }
        }

        // add character change event to event list
        eventList.Add(new CharacterChangeEvent(filePath, charNum, blocking));

    }


    // adds event file to next events list
    private void AddEventFile(StreamReader reader)
    {

        // set file directory
        string file = "Assets/Resources/Events/";
        char character;

       
        // skip the whitespace
        character = skipWhiteSpace(reader);

        file += character;

        // while there's data to read
        while (reader.Peek() > -1)
        {

            // read next character
            character = (char)reader.Read();


            // if end of event symbol
            if (character == ENDEVENT)
            {

                // exit loop
                break;
            }
            else
            {

                // add character to file path
                file += character;
            }
        }


        // add eventfile to list
        nextEvent = file;
    }

    private void GotoNextEvent()
    {

        // increase index to next event
        eventIndex++;

        // check if event exists
        if (eventIndex < eventList.Count)
        {

            // set current event to next event
            currentEvent = eventList[eventIndex];

            // begin the event
            currentEvent.begin();
        }
        else
        {

            // if no event exists load new events
            LoadEvent();

        }

        
    }


    // load new events
    private bool LoadEvent()
    {

        Debug.Log("start load");

        // clear and reset current event list
        eventIndex = 0;
        eventList.Clear();

        // get path for next event
        string path = nextEvent;

        nextEvent = "";

        

        // read nexgt event file
        StreamReader reader = new StreamReader(path);

        //create buffer for reading
        //char[] buffer = null;
        string buffer = "";
        char character;

        // bool for checking if at end
        bool endOfFile = false;

        // while there is data to read
        while (reader.Peek() > -1 && !endOfFile)
        {

            // read first 3 characters of line to buffer
            character = (char)reader.Read();

            //Debug.Log("\nhere is read character");
            //Debug.Log(character);

            if (character == KEYSTART)
            {
                buffer = "";
                while (reader.Peek() > -1)
                {
                    character = (char)reader.Read();
                    if (character == KEYEND)
                    {
                        break;
                    }
                    else
                    {
                        buffer += character;
                    }
                }
            }
            else
            {
                continue;
            }


            // check type of event
            switch (buffer.ToUpper())
            {

                case "DIALOG":   // if event type dialog
                    Debug.Log("dialogevent");
                    CreateDialogEvent(reader);
                    break;
                case "EVENTFILE":
                    Debug.Log("eventfile");
                    AddEventFile(reader);
                    break;
                case "AUDIO":
                    Debug.Log("audioevent");
                    CreateAudioEvent(reader);
                    break;
                case "CHARACTERLOAD":
                    Debug.Log("characterLoadEvent");
                    CreateCharacterChangeEvent(reader);
                    break;
                case "END":   // if end of file
                    Debug.Log("end of file");
                    endOfFile = true;
                    break;
                default:
                    break;
            }
        }

        

        reader.Close();


        currentEvent = eventList[eventIndex];
        currentEvent.begin();

        //Debug.Log("finished loading");
        return false;
    }
}


