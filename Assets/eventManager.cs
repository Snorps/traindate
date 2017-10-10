using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class eventManager : MonoBehaviour
{

    static char NEWLINE = ';';
    static char ENDEVENT = '~';
    static char KEYSTART = '[';
    static char KEYEND = ']';
    static char DECISIONSTART = '{';
    static char DECISIONEND = '}';


    UIhandler UI;

    // index of which is the current event
    private int eventIndex;
    private BaseEvent currentEvent;
    private List<BaseEvent> eventList = new List<BaseEvent> ();
    private eventLoader loader = new eventLoader();

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

        UI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIhandler>();
    }

    // Update is called once per frame
    void Update()
    {

        // check if there is a current event
        if (currentEvent != null)
        {


            // check for inputs

            if (UI.dialogStaggering)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // call ui staggering
                    UI.onInput();
                }
            }
            else
            {
                currentEvent.OnInput();
            }
            //Debug.Log("checking for input");

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
            Debug.Log("beginning an event");
            currentEvent.begin();
        }
        else
        {

            // if no event exists load new events
            LoadEvent();

        }

        
    }

    public void InsertEvents(List<BaseEvent> newEvents)
    {

        eventList.InsertRange(eventIndex + 1, newEvents);
        
    }

    public void SetNextEvent(string path)
    {

        nextEvent = path;
    }

    // load new events
    private bool LoadEvent()
    {

        // clear and reset current event list
        eventIndex = 0;
        eventList.Clear();

        // get path for next event
        eventList = loader.load(nextEvent);
        //string path = nextEvent;

        nextEvent = "";



        currentEvent = eventList[eventIndex];
        currentEvent.begin();

        //Debug.Log("finished loading");
        return false;
    }
}


