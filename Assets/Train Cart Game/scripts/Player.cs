using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    bool currentKey = true;
    public float velocity = 0.0f;
    public float acceleration = 0.3f;
    public float maxVelocity = 5.0f;

    public float drag = 0.1f;
    public float deceleration = 0.2f;

    public Texture powerBar;

    // Use this for initialization
    void Start() {

        StartCoroutine(StepDownVelocity());
        
    }
	
	// Update is called once per frame
	void Update () {


        if ( (currentKey && Input.GetButtonDown("Right") ) || (!currentKey && Input.GetButtonDown("Left")) )
        {
            velocity += acceleration;
            currentKey = !currentKey;
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameEvent meh = (GameEvent)eventManager.manager.GetCurrentEvent();
            meh.ReturnToMain();
        }

        gameObject.transform.Translate(new Vector3(velocity * Time.deltaTime, 0.0f, 0.0f));

	}

    

    IEnumerator StepDownVelocity()
    {

        while (true)
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

    void OnGUI()
    {


        
        GUI.Box(new Rect(10, 10, 300, 30), "");

        GUI.DrawTexture(new Rect(10, 10, 300 * velocity / maxVelocity, 30), powerBar);


    }

}
