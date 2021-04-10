using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    Button button = null;
    AudioSource audioSource = null;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        button.onClick.AddListener(() =>
        {
            audioSource.Play();
            SceneManager.LoadScene(1);
        });


    }
}
