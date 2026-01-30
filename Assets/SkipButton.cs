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


    public void OnSkip()
    {
        textType.currentIndex = 11;
        fadeimage.SetActive(true);
    }

    
}
