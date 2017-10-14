using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class eventManager : MonoBehaviour
{

    public static eventManager manager;

    // index of which is the current event
    private int eventIndex;
    private BaseEvent currentEvent;
    private List<BaseEvent> eventList = new List<BaseEvent> ();
    private eventLoader loader = new eventLoader();

    private bool skipCurrent = false;

    private string nextEvent;

    void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else if (manager != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialisation
    void Start ()
    {

        // set default values
        eventIndex = 0;
        nextEvent = "Assets/Resources/Events/dialog1.event";
        currentEvent = null;

        //UI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIhandler>();
    }

    // Update is called once per frame
    void Update()
    {

        // check if there is a current event
        if (currentEvent != null)
        {

            

            // check for inputs

            if (UIhandler.UI.dialogStaggering)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // call ui staggering
                    UIhandler.UI.OnInput();
                }
            }
            else
            {
                currentEvent.OnInput();
            }
            //Debug.Log("checking for input");

            if (skipCurrent)
            {
                LoadEvent();
            }

            // check if event has ended
            else if (currentEvent.End)
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

    public void SkipEvents()
    {
        skipCurrent = true;
    }

    public void GotoNextEvent()
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

    public BaseEvent GetCurrentEvent()
    {
        return currentEvent;
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

        skipCurrent = false;

        currentEvent = eventList[eventIndex];
        currentEvent.begin();

        //Debug.Log("finished loading");
        return false;
    }
}


