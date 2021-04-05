using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ARManager : MonoBehaviour
{
    Color transparent = new Color(1, 1, 1, 0);
    Color transparent50 = new Color(1, 1, 1, 0.5f);
    Coroutine CoroutineBack = null;
    Coroutine CoroutineBody = null;
    Coroutine CoroutineMessage = null;
    Coroutine CoroutineCloseAction = null;

    [Header("--- Back Group ---")]
    [SerializeField] GameObject backGroup = null;
    [SerializeField] GameObject ringGround = null;
    [SerializeField] List<GameObject> ring;

    [Header("--- Body Group ---")]
    [SerializeField] GameObject bodyGroup = null;

    [Header("--- Message Group ---")]
    [SerializeField] GameObject messageBlock = null;
    RectTransform messageBG = null;
    TMP_Text messageText = null;
    [Header("--- Action Group ---")]
    [SerializeField] GameObject ActionBlock = null;
    TMP_Text ActionText = null;
    float backTime = 0.2f;
    string debugString = "Debug:\n";

    List<string> message = new List<string>
    {
        "Detected the MacBook.",
        "This is the Component of MacBook.",
    };

    List<string> actionMessage = new List<string>
    {
        "Step 1 : Unscrew the screws ...",
    };

    void Start()
    {
        Initizalition();
    }

    void Initizalition()
    {
        for (int i = 0; i < ring.Count; i++)
            ring[i].SetActive(false);

        messageBG = messageBlock.transform.Find("BG").transform.GetComponent<RectTransform>();
        messageText = messageBlock.transform.Find("Text").transform.GetComponent<TMP_Text>();
        ActionText = ActionBlock.transform.Find("Text").transform.GetComponent<TMP_Text>();

        LeanTween.color(messageBG, transparent, 0);
        ActionBlock.SetActive(false);
    }

    public void DetectedBack()
    {
        if (CoroutineBack == null)
        {
            CoroutineBack = StartCoroutine(_detectedBack());
        }

        if (CoroutineMessage == null)
        {
            CoroutineMessage = StartCoroutine(ShowMessage(0));
        }

        if (CoroutineCloseAction != null)
        {
            StopCoroutine(CoroutineCloseAction);
            CoroutineCloseAction = null;
        }
    }

    public void LostBack()
    {
        if (CoroutineBack != null)
        {
            StopCoroutine(CoroutineBack);
            CoroutineBack = null;

            for (int i = 0; i < ring.Count; i++)
                ring[i].SetActive(false);
        }

        if (CoroutineCloseAction == null)
        {
            CoroutineCloseAction = StartCoroutine(CloseAction());
        }

        if (CoroutineMessage != null)
        {
            StopCoroutine(CoroutineMessage);
            CoroutineMessage = null;
        }
    }

    public void DetectedBody()
    {
        if (CoroutineBody == null)
        {
            CoroutineBody = StartCoroutine(_detectedBody());
        }

        if (CoroutineMessage == null)
        {
            CoroutineMessage = StartCoroutine(ShowMessage(1));
        }

        if (CoroutineCloseAction != null)
        {
            StopCoroutine(CoroutineCloseAction);
        }
    }

    IEnumerator _detectedBack()
    {
        ActionBlock.SetActive(true);
        ActionText.text = actionMessage[0];

        for (int i = 0; i < ring.Count; i++)
        {
            ring[i].SetActive(true);
            yield return new WaitForSeconds(backTime);
        }

        backTime = 0.05f;
    }

    IEnumerator _detectedBody()
    {
        yield return new WaitForSeconds(0.2f);
    }


    IEnumerator ShowMessage(int stringNumber, float displayTime = 3)
    {
        messageText.color = Color.black;
        messageText.text = message[stringNumber];
        LeanTween.color(messageBG, transparent50, 0.5f);

        yield return new WaitForSeconds(displayTime);

        messageText.color = transparent;
        LeanTween.color(messageBG, transparent, 2);
    }

    // After Lost Target 3 second will close Action Block
    IEnumerator CloseAction()
    {
        yield return new WaitForSeconds(3);
        backTime = 0.2f;
        ActionBlock.SetActive(false);
    }

    void OnGUI()
    {
        if (Application.isEditor || Debug.isDebugBuild)
        {
            GUIStyle myStyle = new GUIStyle(GUI.skin.button);
            myStyle.fontSize = 20;
            myStyle.normal.textColor = Color.green;
            myStyle.hover.textColor = Color.red;

            GUI.Box(new Rect(10, 10, 200, 100), debugString, myStyle);
        }
    }
}
