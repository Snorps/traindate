using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCartManager : MonoBehaviour {

    private List<string> dialog;

    private string count = "";

    public bool playing = false;
    private bool started = false;

    public float endPos;

    public static TrainCartManager manager;


    void Awake()
    {

        if (manager == null)
        {
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (playing)
        {

            Debug.Log(Player.self.transform.position.x);
            if (Player.self.transform.position.x >= endPos)
            {

                End(true);
                playing = false;
            }
            else if (enemy.self.transform.position.x >= endPos)
            {

                End(false);
                playing = false;
            }

        }
        else
        {
            
            if (Input.GetButtonDown("Start") && !started)
            {

                Debug.Log("it's starting");
                
                StartCoroutine(CountDown());
                started = true;
                
            }

        }

        
	}

    void OnGUI()
    {

        GUIStyle style = new GUIStyle();

        style.alignment = TextAnchor.MiddleCenter;

        count = GUI.TextField(new Rect(Screen.width/2 - 15.0f, Screen.height/2 - 15.0f, 30.0f, 30.0f), count, style);


    }

    IEnumerator CountDown()
    {
        count = "5";
        for (int i = 5; i > 0; i--)
        {
            count = i.ToString();
            Debug.Log(count);

            yield return new WaitForSeconds(1.0f);


        }
        count = "GO!!!";
        StartGame();

        yield return new WaitForSeconds(1.0f);

        count = "";
        
    }

    void StartGame()
    {

        playing = true;

        Player.self.StartGame();
        enemy.self.StartGame();

    }

    void End(bool winner)
    {

        GameEvent meh = (GameEvent)eventManager.manager.GetCurrentEvent();
        meh.ReturnToMain();

    }

}
