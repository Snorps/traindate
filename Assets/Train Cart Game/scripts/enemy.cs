using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {

    public float velocity = 0.0f;
    public float acceleration = 0.3f;
    public float maxVelocity = 5.0f;

    public float drag = 0.1f;
    public float deceleration = 0.2f;

    public Texture powerBar;


    public static enemy self;

    void Awake()
    {
        
        if (self == null)
        {
            self = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }


    // Use this for initialization
    void Start () {

        
	}

    public void StartGame()
    {

        Random.InitState(Time.frameCount);

        StartCoroutine(StepDownVelocity());
        StartCoroutine(accelerate());
    }
	
	// Update is called once per frame
	void Update () {

        if (TrainCartManager.manager.playing)
        {
            gameObject.transform.Translate(new Vector3(velocity * Time.deltaTime, 0.0f, 0.0f));
        }
	}

    IEnumerator StepDownVelocity()
    {

        while (TrainCartManager.manager.playing)
        {

            yield return new WaitForSeconds(0.01f);
            if (velocity > 0)
            {

                velocity = velocity - ((deceleration + velocity * drag) * 0.01f);
                if (velocity < 0)
                {
                    velocity = 0;
                }
                else if (velocity > maxVelocity)
                {
                    velocity = maxVelocity;
                }

            }
        }

    }

    IEnumerator accelerate()
    {

        while (TrainCartManager.manager.playing)
        {

            yield return new WaitForSeconds(0.1f);

            velocity += acceleration * Random.Range(0.6f, 1.4f);

        }

    }

    void OnGUI()
    {


        GUI.Box(new Rect(Screen.width - 310, 10, 300, 30), "");

        //GUI.DrawTexture(new Rect(Screen.width - 310 + 300 * velocity / maxVelocity, 10, 300 * velocity / maxVelocity, 30), powerBar);
        GUI.DrawTexture(new Rect(Screen.width - 310 + (300 - 300 * velocity / maxVelocity), 10, 300 * velocity / maxVelocity, 30), powerBar);

    }

}


