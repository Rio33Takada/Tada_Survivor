using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Title_StartButton : MonoBehaviour
{
    public GameObject text;
    public GameObject FadeImage; //フェードイメージを入れる
    public TextType texttype;

    //ストーリー（？）文章画面表示
    public void OnTitileButton()
    {
        FadeImage.SetActive(true);
        text.SetActive(true);
        texttype.StartButton();
    }
}
