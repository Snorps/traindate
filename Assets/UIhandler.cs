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

    public static UIhandler UI;

    void Awake()
    {
        if (UI == null)
        {
            DontDestroyOnLoad(gameObject);
            UI = this;
        }
        else if (UI != this)
        {
            Destroy(gameObject);
        }
    }

    Text dialogText;
    Text nameText;

    public bool dialogStaggering;
    public Image instantiableImage;

    // Use this for initialization
    void Start()
    {
        dialogText = this.gameObject.transform.Find("DialogText").gameObject.GetComponent<Text>();
        nameText = this.gameObject.transform.Find("NameText").gameObject.GetComponent<Text>();
        dialogStaggering = false;
    }

    //Show/Hide UI
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public bool IsHidden()
    {
        return !this.gameObject.activeSelf;
    }


    //////////////////////////////Dialog Stuff////////////////////////////////////
    public void OnInput()
    {
        dialogStaggering = false;
    }

    public void DisplayText(Dialog dialog) //to be called by the event manager
    {
        if (dialog.name == "")
        {
            nameText.text = "";
            ChangeImageSprite("NameBox", "blank64");
        }
        else
        {
            nameText.text = dialog.name;
            ChangeImageSprite("NameBox", "9tile");
        }
        string str = dialog.message;
        ClearText();
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

    public void ClearText()
    {
        dialogText.text = "";
    }

    public void DisplayDecision(Dialog dialog, List<string> options)
    {
        DisplayText(dialog);
        ChangeImageSprite("DecisionBox", "9tile");
    }

    /////////////////////Low Level Sprite Stuff//////////////////////////

    public void ChangeImageSprite(string imageName, string filePath)
    {
        this.gameObject.transform.Find(imageName).gameObject.GetComponent<Image>().sprite = Resources.Load(filePath, typeof(Sprite)) as Sprite;
    }

    public void CreateImage(string name, string spriteFilePath, Vector2 position) //Create a new image, low level, use higher level functions for characters in stead
    {
        Sprite sprite = Resources.Load(spriteFilePath, typeof(Sprite)) as Sprite;
        Image image = Instantiate(instantiableImage);
        image.gameObject.GetComponent<Image>().sprite = sprite;
        image.gameObject.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void FadeImage(string imageName, float targetAlpha = 0f, float time = 0.25f)
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

    public void MoveImageAbsolute(string imageName, Vector2 absoluteDest, float time)
    {
        Image image = this.gameObject.transform.Find(imageName).gameObject.GetComponent<Image>();
        RectTransform tf = image.GetComponent<RectTransform>();

        Vector2 anchorPos = new Vector2(tf.anchoredPosition.x, tf.anchoredPosition.y);
        Vector2 relativeDest = absoluteDest - anchorPos;
        Debug.Log("anchor " + anchorPos);
        Debug.Log("relative dest " + relativeDest);

        StartCoroutine(StaggerImagePosition(image, relativeDest, time));
    }

    public void MoveImageRelative(string imageName, Vector2 relativeDest, float time)
    {
        Image image = this.gameObject.transform.Find(imageName).gameObject.GetComponent<Image>();

        StartCoroutine(StaggerImagePosition(image, relativeDest, time));
    }

    IEnumerator StaggerImagePosition(Image image, Vector2 relativeDest, float time)
    {
        RectTransform tf = image.GetComponent<RectTransform>();
        Vector2 currentPos = new Vector2(tf.anchoredPosition.x, tf.anchoredPosition.y);

        float delay = 0.02f; //time between each iteration
        int stepCount = (int)(time / delay); //number of times to iterate
        Vector2 posStep = relativeDest / stepCount; //amount to increase/decrease alpha by each iteration

        for (int i = 0; i < stepCount; i++)
        {
            tf.position = new Vector3(tf.position.x + posStep.x, tf.position.y + posStep.y, tf.position.z);
            yield return new WaitForSeconds(delay);
        }
    }

    ////////////////////////////////////High Level Sprite Stuff////////////////////////////////////


}