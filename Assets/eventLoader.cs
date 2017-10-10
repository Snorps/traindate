using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class eventLoader {

    static char NEWLINE = ';';
    static char ENDEVENT = '~';
    static char KEYSTART = '[';
    static char KEYEND = ']';
    static char DECISIONSTART = '{';
    static char DECISIONEND = '}';

    public List<BaseEvent> load(string path)
    {
        Debug.Log("start load");

        List<BaseEvent> eventList = new List<BaseEvent>();

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

            //BaseEvent newEvent = null;

            // check type of event
            switch (buffer.ToUpper())
            {
                case "DECISION":
                    Debug.Log("decisionEvent");
                    eventList.Add(CreateDecisionEvent(reader));
                    //newEvent = new DecisionEvent(reader);
                    break;
                case "DIALOG":   // if event type dialog
                    Debug.Log("dialogevent");
                    eventList.Add( CreateDialogEvent(reader) );
                    //newEvent = new DialogEvent(reader);
                    break;
                case "EVENTFILE":
                    Debug.Log("eventfile");
                    eventList.Add( AddEventFile(reader) );
                    //newEvent = new SetNextFileEvent(reader);
                    break;
                case "AUDIO":
                    Debug.Log("audioevent");
                    eventList.Add( CreateAudioEvent(reader) );
                    //newEvent = new AudioEvent(reader);
                    break;
                case "CHARACTERLOAD":
                    Debug.Log("characterLoadEvent");
                    eventList.Add( CreateCharacterChangeEvent(reader) );
                    //newEvent = new CharacterChangeEvent(reader);
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

        return eventList;
    }

    private char readLine(StreamReader reader, ref string line, bool skipWhite = false)
    {

        char character = ENDEVENT;

        if (skipWhite)
        {

            character = skipWhiteSpace(reader);
            if (character == ENDEVENT || character == NEWLINE)
            {

                return character;
            }

            line += character;
        }



        do
        {

            character = (char)reader.Read();

            if (character == NEWLINE || character == ENDEVENT)
            {
                break;
            }
            else
            {
                line += character;
            }


        } while (reader.Peek() > -1);


        return character;
    }

    // create event types
    //
    // adds Dialog event to event list
    private DialogEvent CreateDialogEvent(StreamReader reader)
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



        return new DialogEvent(DialogList);
        // create and add dialog event to the event list


    }


    // adds decision event to event list
    private DecisionEvent CreateDecisionEvent(StreamReader reader)
    {

        List<Decision> DecisionList = new List<Decision>();
        List<BaseEvent> eventList = new List<BaseEvent>();

        char character;
        string message = "";
        string buffer = "";

        while (reader.Peek() > -1)
        {

            buffer = "";
            message = "";

            character = skipWhiteSpace(reader);

            //Debug.Log(character);

            if (character == DECISIONSTART)
            {

                Debug.Log("decision started");

                while (reader.Peek() > -1)
                {

                    buffer = "";

                    character = skipWhiteSpace(reader);

                    //Debug.Log(character);

                    if (character == DECISIONEND)
                    {
                        //Debug.Log("decision has ended");
                        break;
                    }

                    else if (character == KEYSTART)
                    {

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
                            case "MESSAGE":
                                Debug.Log("adding message");
                                readLine(reader, ref message, true);
                                break;
                            case "DIALOG":   // if event type dialog
                                Debug.Log("dialogeventpoo");
                                eventList.Add(CreateDialogEvent(reader));
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
                                //Debug.Log("end of file");
                                //endOfFile = true;
                                break;
                            default:
                                break;
                        }
                        //Debug.Log("loaded event in decision");
                    }

                }

                //Debug.Log("decision being added");
                DecisionList.Add(new Decision(message, eventList));

                eventList = new List<BaseEvent>();
                message = "";
            }
            else if (character == ENDEVENT)
            {
                break;

            }
            else
            {
                //  parsing error
            }



        }

        Debug.Log("decision count");
        Debug.Log(DecisionList.Count);
        return new DecisionEvent(DecisionList);

    }

    // adds audio event to event list
    private AudioEvent CreateAudioEvent(StreamReader reader)
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
        return new AudioEvent(filePath, blocking);

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
    private CharacterChangeEvent CreateCharacterChangeEvent(StreamReader reader)
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
        return new CharacterChangeEvent(filePath, charNum, blocking);

    }



    // adds event file to next events list
    private SetNextFileEvent AddEventFile(StreamReader reader)
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
        return new SetNextFileEvent(file);
    }



}
