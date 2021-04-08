using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ARManager : MonoBehaviour
{
    enum Step
    {
        Non,
        unscrew,
        removeBackCoverGroup,
        removePower,
        component,
        Ram,
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
    [SerializeField] GameObject ringGroup = null;
    [SerializeField] GameObject screwdriversGroup = null;
    [SerializeField] GameObject backCoverGroup = null;
    [SerializeField] List<GameObject> ring = new List<GameObject>();

    [Header("--- Body Group ---")]
    [SerializeField] GameObject bodyGroup = null;
    [SerializeField] GameObject removerPowerGroup = null;
    [SerializeField] GameObject componentGroup = null;

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
    [SerializeField] List<Button> componentButton = new List<Button>();

    [Header("--- Other Group ---")]
    [SerializeField] GameObject screwdriversType = null;
    [SerializeField] AudioClip onClickSound = null;
    float backTime = 0.2f;
    bool detected = false;

    List<string> message = new List<string>
    {
        "Detected the back cover of MacBook.",
        "Detected the inside of MacBook.",
    };

    List<string> actionMessage = new List<string>
    {
        "Step 1 : Unscrew the screws ...",
        "Step 2 : Remove the back cover ...",
        "Step 3 : Remove the power ...",
        "Please choose an option ...",
        "You choose to remove RAM ..."
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

        screwdriversGroup.SetActive(false);
        screwdriversType.SetActive(false);
        backCoverGroup.SetActive(false);
        removerPowerGroup.SetActive(false);
        componentGroup.SetActive(false);
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
                case Step.removeBackCoverGroup:
                    ringGroup.SetActive(true);
                    screwdriversGroup.SetActive(true);
                    screwdriversType.SetActive(true);
                    ActionBlock.SetActive(true);
                    ActionText.text = actionMessage[0];

                    if (CoroutineBack != null)
                    {
                        StopCoroutine(CoroutineBack);
                        CoroutineBack = null;
                    }

                    backCoverGroup.SetActive(false);
                    frontButton.gameObject.SetActive(false);
                    nextButton.gameObject.SetActive(true);
                    myStep = Step.unscrew;
                    break;
                case Step.Ram:
                    componentButton.ForEach(button => button.gameObject.transform.parent.parent.gameObject.SetActive(true));

                    frontButton.gameObject.SetActive(false);
                    myStep = Step.component;
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
                    backCoverGroup.SetActive(true);
                    ActionBlock.SetActive(true);
                    ActionText.text = actionMessage[1];

                    if (CoroutineBack != null)
                    {
                        StopCoroutine(CoroutineBack);
                        CoroutineBack = null;
                    }

                    ringGroup.SetActive(false);
                    screwdriversGroup.SetActive(false);
                    screwdriversType.SetActive(false);
                    frontButton.gameObject.SetActive(true);
                    nextButton.gameObject.SetActive(false);
                    myStep = Step.removeBackCoverGroup;
                    break;

                case Step.removePower:
                    componentGroup.SetActive(true);
                    ActionBlock.SetActive(true);
                    ActionText.text = actionMessage[3];

                    if (CoroutineBack != null)
                    {
                        StopCoroutine(CoroutineBack);
                        CoroutineBack = null;
                    }

                    removerPowerGroup.SetActive(false);
                    frontButton.gameObject.SetActive(false);
                    nextButton.gameObject.SetActive(false);
                    myStep = Step.component;
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

        for (int i = 0; i < componentButton.Count; i++)
        {
            componentButton[i].onClick.AddListener(() =>
            {
                audioSource.PlayOneShot(onClickSound);
                componentButton.ForEach(button => button.gameObject.transform.parent.parent.gameObject.SetActive(false));
                componentButton[i].gameObject.SetActive(true);
                frontButton.gameObject.SetActive(true);
                myStep = Step.Ram;
            });
        }

        frontButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }

    public void DetectedBack()
    {
        detected = true;
        nextButton.gameObject.SetActive(true);

        switch (myStep)
        {
            case Step.Non:
                myStep = Step.unscrew;
                break;

            case Step.removeBackCoverGroup:
                frontButton.gameObject.SetActive(true);
                break;

            case Step.removePower:
            case Step.component:
                myStep = Step.unscrew;
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
        detected = false;
        frontButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        if (CoroutineBack != null)
        {
            StopCoroutine(CoroutineBack);
            CoroutineBack = null;
            screwdriversGroup.SetActive(false);
            screwdriversType.SetActive(false);
            backCoverGroup.SetActive(false);

            for (int i = 0; i < ring.Count; i++)
                ring[i].SetActive(false);
        }

        if (CoroutineBody != null)
        {
            StopCoroutine(CoroutineBody);
            CoroutineBody = null;

            removerPowerGroup.SetActive(false);
            componentGroup.SetActive(false);
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
        detected = true;
        switch (myStep)
        {
            case Step.Non:
                myStep = Step.component;
                break;
            case Step.unscrew:
            case Step.removeBackCoverGroup:
            case Step.removePower:
                myStep = Step.removePower;
                break;
        }


        if (CoroutineBack != null)
        {
            StopCoroutine(CoroutineBack);
            CoroutineBack = null;
        }

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
                backCoverGroup.SetActive(false);
                ringGroup.SetActive(true);
                ActionBlock.SetActive(true);
                ActionText.text = actionMessage[0];

                for (int i = 0; i < ring.Count; i++)
                {
                    ring[i].SetActive(true);
                    yield return new WaitForSeconds(backTime);
                }

                backTime = 0.05f;
                screwdriversGroup.SetActive(true);
                screwdriversType.SetActive(true);
                break;

            case Step.removeBackCoverGroup:
                ActionBlock.SetActive(true);
                ActionText.text = actionMessage[1];
                ringGroup.SetActive(false);
                screwdriversGroup.SetActive(false);
                screwdriversType.SetActive(false);
                backCoverGroup.SetActive(true);
                break;
        }
    }

    IEnumerator _detectedBody()
    {
        yield return null;

        switch (myStep)
        {
            case Step.removePower:
                removerPowerGroup.SetActive(true);
                ActionBlock.SetActive(true);
                ActionText.text = actionMessage[2];
                nextButton.gameObject.SetActive(true);

                ringGroup.SetActive(false);
                screwdriversGroup.SetActive(false);
                screwdriversType.SetActive(false);
                backCoverGroup.SetActive(false);
                componentGroup.SetActive(false);
                break;

            case Step.component:
                componentGroup.SetActive(true);
                ActionBlock.SetActive(true);
                ActionText.text = actionMessage[3];

                removerPowerGroup.SetActive(false);
                break;
            case Step.Ram:
                componentGroup.SetActive(true);
                ActionBlock.SetActive(true);
                ActionText.text = actionMessage[4];

                removerPowerGroup.SetActive(false);
                break;
        }
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

            GUI.Box(new Rect(100, 0, 200, 40), "" + detected, myStyle);
        }
    }
}
