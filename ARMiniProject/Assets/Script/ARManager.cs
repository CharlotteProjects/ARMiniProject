using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ARManager : MonoBehaviour
{
    enum Step
    {
        Non,
        unscrew,
        removeCoverGround,
        removePower,
    }
    Step myStep = Step.Non;

    Color transparent = new Color(1, 1, 1, 0);
    Color transparent50 = new Color(1, 1, 1, 0.5f);

    Coroutine CoroutineBack = null;
    Coroutine CoroutineBody = null;
    Coroutine CoroutineMessage = null;
    Coroutine CoroutineCloseAction = null;

    AudioSource audioSource = null;

    [Header("--- Back Group ---")]
    [SerializeField] GameObject backGroup = null;
    [SerializeField] GameObject ringGround = null;
    [SerializeField] GameObject ScrewdriversGround = null;
    [SerializeField] GameObject BackCoverGround = null;
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
    [Header("--- Button Group ---")]
    [SerializeField] Button exitButton = null;
    [SerializeField] Button frontButton = null;
    [SerializeField] Button nextButton = null;
    [Header("--- Other Group ---")]
    [SerializeField] AudioClip onClickSound = null;
    float backTime = 0.2f;
    string debugString = "Debug:\n";

    List<string> message = new List<string>
    {
        "Detected the back cover of MacBook.",
        "Detected the inside of MacBook.",
    };

    List<string> actionMessage = new List<string>
    {
        "Step 1 : Unscrew the screws ...",
        "Step 2 : Remove the back cover ..."
    };

    void Start()
    {
        Initizalition();
        InitizalitionButton();
    }

    void Initizalition()
    {
        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < ring.Count; i++)
            ring[i].SetActive(false);

        messageBG = messageBlock.transform.Find("BG").transform.GetComponent<RectTransform>();
        messageText = messageBlock.transform.Find("Text").transform.GetComponent<TMP_Text>();
        ActionText = ActionBlock.transform.Find("Text").transform.GetComponent<TMP_Text>();

        ScrewdriversGround.SetActive(false);
        BackCoverGround.SetActive(false);
        LeanTween.color(messageBG, transparent, 0);
        ActionBlock.SetActive(false);
    }

    void InitizalitionButton()
    {
        exitButton.onClick.AddListener(() => { Application.Quit(); });

        frontButton.onClick.AddListener(() =>
        {
            Debug.Log("OnClick Front");
            audioSource.PlayOneShot(onClickSound);

            switch (myStep)
            {
                case Step.removeCoverGround:
                    BackCoverGround.SetActive(false);
                    frontButton.gameObject.SetActive(false);
                    nextButton.gameObject.SetActive(true);

                    if (CoroutineBack != null)
                    {
                        StopCoroutine(CoroutineBack);
                        CoroutineBack = null;
                    }

                    ringGround.SetActive(true);
                    ScrewdriversGround.SetActive(true);
                    ActionText.text = actionMessage[0];
                    myStep = Step.unscrew;
                    break;
            }

            if (CoroutineMessage != null)
            {
                StopCoroutine(CoroutineMessage);
                CoroutineMessage = null;
                messageText.color = transparent;
                LeanTween.color(messageBG, transparent, 0.5f);
            }
        });

        nextButton.onClick.AddListener(() =>
        {
            Debug.Log("OnClick Next");
            audioSource.PlayOneShot(onClickSound);

            switch (myStep)
            {
                case Step.unscrew:
                    ringGround.SetActive(false);
                    ScrewdriversGround.SetActive(false);

                    if (CoroutineBack != null)
                    {
                        StopCoroutine(CoroutineBack);
                        CoroutineBack = null;
                    }

                    ActionText.text = actionMessage[1];
                    BackCoverGround.SetActive(true);
                    frontButton.gameObject.SetActive(true);
                    nextButton.gameObject.SetActive(false);
                    myStep = Step.removeCoverGround;
                    break;
            }

            if (CoroutineMessage != null)
            {
                StopCoroutine(CoroutineMessage);
                CoroutineMessage = null;
                messageText.color = transparent;
                LeanTween.color(messageBG, transparent, 0.5f);
            }
        });

        frontButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }

    public void DetectedBack()
    {
        nextButton.gameObject.SetActive(true);

        switch (myStep)
        {
            case Step.Non:
                myStep = Step.unscrew;
                break;
            case Step.removeCoverGround:
                frontButton.gameObject.SetActive(true);
                break;
        }

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
        frontButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        if (CoroutineBack != null)
        {
            StopCoroutine(CoroutineBack);
            CoroutineBack = null;
            ScrewdriversGround.SetActive(false);

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

            messageText.color = transparent;
            LeanTween.color(messageBG, transparent, 0.5f);
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
        switch (myStep)
        {
            case Step.unscrew:
                ActionBlock.SetActive(true);
                ActionText.text = actionMessage[0];

                for (int i = 0; i < ring.Count; i++)
                {
                    ring[i].SetActive(true);
                    yield return new WaitForSeconds(backTime);
                }

                backTime = 0.05f;
                ScrewdriversGround.SetActive(true);
                break;

            case Step.removeCoverGround:
                ActionBlock.SetActive(true);
                ActionText.text = actionMessage[1];

                ringGround.SetActive(false);
                ScrewdriversGround.SetActive(false);
                BackCoverGround.SetActive(true);
                break;
        }
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
        LeanTween.color(messageBG, transparent, 0.5f);
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

            GUI.Box(new Rect(10, 10, 200, 20), debugString, myStyle);
        }
    }
}
