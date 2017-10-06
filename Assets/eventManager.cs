using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class eventManager : MonoBehaviour
{


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
        public virtual void OnMouseDown()
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


            file = filePath;

            blocking = block;

            Audio = GameObject.FindGameObjectWithTag("AudioHandler");

            End = false;
        }

        ~AudioEvent()
        {

        }


        // virtual methods for inheritance
        //
        // begin is called when event starts
        public override void begin()
        {
            // play audio from file

            if (!blocking)
            {
                End = true;
            }
        }

        // OnMouse Down is called when left mouse button is pressed
        public override void OnMouseDown()
        {
            End = true;
        }

        // returns next event
        public override string nextEvent()
        {
            return nextEventList[0];
        }

    }
    

    // Base event class
    public class CharacterChangeEvent : BaseEvent
    {

        private string file;

        private bool blocking;

        private string charNum;

        private GameObject UI;

        // constructor/destructor
        public CharacterChangeEvent(string filePath, string characterNum, bool block = false )
        {
            file = filePath;

            blocking = block;

            charNum = characterNum;

            UI = GameObject.FindGameObjectWithTag("Canvas");

            End = false;
        }

        ~CharacterChangeEvent()
        {

        }


        // virtual methods for inheritance
        //
        // begin is called when event starts
        public override void begin()
        {

            UI.GetComponent<UIhandler>().changeImageSprite(charNum, file);

            if (!blocking)
            {
                End = true;
            }

        }

        // OnMouse Down is called when left mouse button is pressed
        public override void OnMouseDown()
        {

            End = true;
        }

        // returns next event
        public override string nextEvent()
        {
            return nextEventList[0];
        }

    }

    public class DialogEvent : BaseEvent
    {

        // List of dialog used
        private List<string> dialog;

        // index of current dialog
        private int currentDialog;

        // pointers to UI/AUDIO objects
        private GameObject UI;
        private GameObject Audio;

        

        // constructor/destructor
        public DialogEvent(List<string> newDialog)//, string nextEvent, GameObject setUI, GameObject setAudio )
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
            // UI display first dialog

        }

        // when mouse button down
        public override void OnMouseDown()
        {
            // goto next dialog 
            currentDialog++;

            // check if at end of dialog
            if (currentDialog < dialog.Count) {

                // if there is dialog display it

                Debug.Log("mouse down displaying new poop hehe got 'em");
                UI.GetComponent<UIhandler>().changeText(dialog[currentDialog]);
                // UI display dialog[currentDialog]
            }
            else {

                // else end the event
                End = true;
            }
        }

        // returns location of next event
        public override string nextEvent()
        {
            
            return nextEventList[0];
        }

    }

    // index of which is the current event
    int eventIndex;
    BaseEvent currentEvent;
    List<BaseEvent> eventList = new List<BaseEvent> ();

    int nextEventIndex;
    List<string> nextEventList = new List<string>();

    // Use this for initialisation
    void Start ()
    {
        eventIndex = 0;
        nextEventIndex = 0;
        nextEventList.Add("Assets/Resources/Events/dialog1.event");
        currentEvent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEvent != null)
        {

            
            // check for inputs
            if (Input.GetMouseButtonDown(0))
            {

                // if mouse button is down then call mouse button down on current event
                currentEvent.OnMouseDown();
            }

            // check if event has ended
            if (currentEvent.End)
            {

                // change current event to the next one
                GotoNextEvent();
            }
        }
        else
        {
            LoadEvent();
        }
    }

    // create event types
    //
    // Dialog event
    private void CreateDialogEvent(StreamReader reader)
    {

        // create variables for creating dialog event
        List<string> DialogList = new List<string>();

        string dialog = "";
        char character;

        bool end = false;

        while (reader.Peek() > -1 && !end)
        {

            character = skipWhiteSpace(reader);

            dialog += character;

            // while not at end of file
            while (reader.Peek() > -1)
            {

                // read next character
                character = (char)reader.Read();

                // check for symbols
                //
                // if end of event symbol
                if (character == '~')
                {

                    //add current dialog to list then break out loop
                    DialogList.Add(dialog);
                    end = true;
                    break;
                }

                // if end of dialog box symbol
                else if (character == '|')
                {

                    // add current dialog to list then set current dialog to empty
                    DialogList.Add(dialog);
                    dialog = "";
                    break;
                }

                else
                {

                    // else add character to dialog string
                    dialog += character;
                }
            }

        }
        
      


        // create and add dialog event to the event list
        eventList.Add(new DialogEvent(DialogList));

    }

    private void CreateAudioEvent(StreamReader reader)
    {

        string filePath = "Assets/Resources/Audio/";

        bool blocking = false;
        bool blockCheck = false;

        char character;

        character = skipWhiteSpace(reader);

        filePath += character;

        while(reader.Peek() > -1)
        {

            character = (char)reader.Read();

            if (character == '|')
            {
                blockCheck = true;
                break;
            }
            else if (character == '~')
            {
                blockCheck = false;
                break;
            }
        }

        if (blockCheck)
        {

            string buffer = "";

            character = skipWhiteSpace(reader);

            buffer += character;

            while (reader.Peek() > -1)
            {

                character = (char)reader.Read();

                if (character == '~')
                {
                    break;
                }
                else
                {
                    buffer += character;
                }
            }

            if (buffer.ToUpper() == "TRUE")
            {
                blocking = true;
            }
            else
            {
                blocking = false;
            }
        }


        eventList.Add(new AudioEvent(filePath, blocking));

    }

    private char skipWhiteSpace(StreamReader reader)
    {
        char character = '~';

        while (reader.Peek() > -1)
        {

            character = (char)reader.Read();

            if (character == ' ' || character == '\n' || character == '\t' || character == '\r')
            {
                continue;
            }

            break;
        }
        return character;
    }

    private void CreateCharacterChangeEvent(StreamReader reader)
    {

        string filePath = "Assets/Resources/Characters/";
        string charNum = "";
        bool blocking = false;

        char character;

        character = skipWhiteSpace(reader);

        charNum += character;

        while (reader.Peek() > -1)
        {

            character = (char)reader.Read();

            if (character == '|')
            {
                break;
            }
            else
            {
                charNum += character;
            }
            

        }

        character = skipWhiteSpace(reader);

        filePath += character;

        bool blockCheck = false;

        while (reader.Peek() > -1)
        {
            character = (char)reader.Read();

            if (character == '|')
            {
                blockCheck = true;
                break;
            }
            else if(character == '~')
            {
                break;
            }
            else
            {
                filePath += character;
            }

        }

        if (blockCheck)
        {

            string buffer = "";

            character = skipWhiteSpace(reader);

            buffer += character;

            while (reader.Peek() > -1)
            {

                character = (char)reader.Read();

                if (character == '~')
                {
                    break;
                }
                else
                {
                    buffer += character;
                }
            }

            if (buffer.ToUpper() == "TRUE")
            {
                blocking = true;
            }
            else
            {
                blocking = false;
            }
        }


        eventList.Add(new CharacterChangeEvent(filePath, charNum, blocking));

    }

    private void AddEventFile(StreamReader reader)
    {

        string file = "Assets/Resources/Events/";
        char character;

       

        character = skipWhiteSpace(reader);

        file += character;

        while (reader.Peek() > -1)
        {

            character = (char)reader.Read();

            if (character == '~')
            {
                break;
            }
            else
            {
                file += character;
            }
        }

        nextEventList.Add(file);
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
        string path = nextEventList[nextEventIndex];

        nextEventList.Clear();

        

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

            if (character == '[')
            {
                buffer = "";
                while (reader.Peek() > -1)
                {
                    character = (char)reader.Read();
                    if (character == ']')
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
            switch (buffer)
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


