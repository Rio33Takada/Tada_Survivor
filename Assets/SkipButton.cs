using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class SkipButton : MonoBehaviour
{
    public TextType textType;
    [SerializeField]
    private GameObject fadeimage;
    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = GameObject.Find("BGM").GetComponent<SoundManager>();
    }

    public void OnSkip()
    {
        soundManager.PlaySE(SoundManager.SEType.Select2);
        textType.currentIndex = 11;
        fadeimage.SetActive(true);
    }

    
}
