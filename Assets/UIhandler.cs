using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIhandler : MonoBehaviour {

    Text dialogText;
    public bool dialogStaggering;

    // Use this for initialization
    void Start () {
		dialogText = this.gameObject.transform.Find("DialogText").gameObject.GetComponent<Text>();
        //changeImageSprite("Character", "traumatrain");
        dialogStaggering = false;

    }

    public void changeText(string str, float textDelay = 0.07f) //to be called by the event manager
    {
        if (dialogStaggering)
        {
            dialogStaggering = false;
        }
        else
        {
            clearText();
            dialogStaggering = true;
            StartCoroutine(StaggerText(str, textDelay));
        }
        
    }


    IEnumerator StaggerText(string str, float textDelay) //don't know why this is how you wait in unity
    {
        for (int i = 0; i < str.Length; i++) //for every letter in the string to be displayed
        {
            Debug.Log(dialogStaggering);
            if (dialogStaggering && (i+1) < str.Length)
            {
                dialogText.text = dialogText.text.Insert(dialogText.text.Length, str.Substring(i, 1)); //append curent char text in dialog box
                yield return new WaitForSeconds(textDelay);
            } else
            {
                dialogText.text = dialogText.text.Insert(dialogText.text.Length, str.Substring(i, str.Length-i)); //append rest of text in dialog box
                dialogStaggering = false;
                break; //cancel for if told to skip staggering
            }
        }
    }

    public void clearText()
    {
        dialogText.text = "";
    }

    public void changeImageSprite(string imageName, string filePath)
    {
        this.gameObject.transform.Find(imageName).gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load(filePath);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
