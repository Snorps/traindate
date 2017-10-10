using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Dialog
{
    public string name, message;
    public float textDelay;
    
    public Dialog(string newName, string newMessage, float delay = 0.07f)
    {
        name = newName;
        message = newMessage;
        textDelay = delay;
    }
}

public class UIhandler : MonoBehaviour {

   // public GameObject testNtext;
    Text dialogText;
    Text nameText;
    Sprite nameBoxSprite;
    public bool dialogStaggering;

    // Use this for initialization
    void Start () {
        dialogText = this.gameObject.transform.Find("DialogText").gameObject.GetComponent<Text>();
        nameText = this.gameObject.transform.Find("NameText").gameObject.GetComponent<Text>();
        dialogStaggering = false;
    }

    public void changeText(Dialog dialog) //to be called by the event manager
    {
        if (dialog.name == "")
        {
            nameText.text = "";
            changeImageSprite("NameBox", "blank64");
        } else
        {
            nameText.text = dialog.name;
            nameBoxSprite = Resources.Load("9tile", typeof(Sprite)) as Sprite;
        }
        string str = dialog.message;
        if (dialogStaggering)
        {
            dialogStaggering = false;
        }
        else
        {
            clearText();
            dialogStaggering = true;
            StartCoroutine(StaggerText(str, dialog.textDelay));
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
        this.gameObject.transform.Find(imageName).gameObject.GetComponent<Image>().sprite = Resources.Load(filePath, typeof(Sprite)) as Sprite;
    }

    public void createImage(string name, string spriteFilePath, Vector3 position)
    {

    }

    // Update is called once per frame
    void Update () {
		
	}
}
