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

public class UIhandler : MonoBehaviour
{

    // public GameObject testNtext;
    Text dialogText;
    Text nameText;
    Sprite nameBoxSprite;
    public bool dialogStaggering;
    public Image instantiableImage;

    // Use this for initialization
    void Start()
    {
        dialogText = this.gameObject.transform.Find("DialogText").gameObject.GetComponent<Text>();
        nameText = this.gameObject.transform.Find("NameText").gameObject.GetComponent<Text>();
        dialogStaggering = false;

    }

    //////////////////////////////Dialog Stuff////////////////////////////////////
    public void onInput()
    {
        dialogStaggering = false;
    }

    public void changeText(Dialog dialog) //to be called by the event manager
    {
        if (dialog.name == "")
        {
            nameText.text = "";
            changeImageSprite("NameBox", "blank64");
        }
        else
        {
            nameText.text = dialog.name;
            nameBoxSprite = Resources.Load("9tile", typeof(Sprite)) as Sprite;
        }
        string str = dialog.message;
        clearText();
        dialogStaggering = true;
        StartCoroutine(StaggerText(str, dialog.textDelay));
    }


    IEnumerator StaggerText(string str, float textDelay) //don't know why this is how you wait in unity
    {
        for (int i = 0; i < str.Length; i++) //for every letter in the string to be displayed
        {
            Debug.Log(dialogStaggering);
            if (dialogStaggering && (i + 1) < str.Length)
            {
                dialogText.text = dialogText.text.Insert(dialogText.text.Length, str.Substring(i, 1)); //append curent char text in dialog box
                yield return new WaitForSeconds(textDelay);
            }
            else
            {
                dialogText.text = dialogText.text.Insert(dialogText.text.Length, str.Substring(i, str.Length - i)); //append rest of text in dialog box
                dialogStaggering = false;
                break; //cancel for if told to skip staggering
            }
        }
    }

    public void clearText()
    {
        dialogText.text = "";
    }

    /// //////////////////////////////////////Sprite Stuff//////////////////////////

    public void changeImageSprite(string imageName, string filePath)
    {
        this.gameObject.transform.Find(imageName).gameObject.GetComponent<Image>().sprite = Resources.Load(filePath, typeof(Sprite)) as Sprite;
    }

    public void createImage(string name, string spriteFilePath, Vector2 position) //create a new image, low level, use higher level functions for characters in stead
    {
        Sprite sprite = Resources.Load(spriteFilePath, typeof(Sprite)) as Sprite;
        Image image = Instantiate(instantiableImage);
        image.gameObject.GetComponent<Image>().sprite = sprite;
        image.gameObject.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void fadeImage(string imageName, float targetAlpha = 0f, float time = 0.25f)
    {
        Image image = this.gameObject.transform.Find(imageName).gameObject.GetComponent<Image>();
        Color color = image.color;

        StartCoroutine(StaggerImageAlpha(image, targetAlpha, time));
    }

    IEnumerator StaggerImageAlpha(Image image, float targetAlpha, float time)
    {
        Color color = image.color;
        float currentAlpha = color.a;

        float delay = 0.02f; //time between each iteration
        int stepCount = (int)(time / delay); //number of times to iterate
        float alphaStep = (-(currentAlpha - targetAlpha)) / stepCount; //amount to increase/decrease alpha by each iteration

        for (int i = 0; i < stepCount; i++)
        {
            color.a = color.a + alphaStep;
            image.color = color;
            yield return new WaitForSeconds(delay);
        }
    }
}
