using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Title_StartButton : MonoBehaviour
{
    public GameObject text;
    public Image FadeImage; //フェードイメージを入れる
    public GameObject img;
    public float fadetime;
    public TextType texttype;

    //ストーリー（？）文章画面表示
    public void OnTitileButton()
    {
        StartCoroutine(Fadeout());
        img.SetActive(true);
    }

    public IEnumerator Fadeout()
    {
        float t = 0;
        float targetAlpha = 1f;
        Color c = FadeImage.color;
        c.a = 0f;

        while (t < fadetime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, t / fadetime);
            FadeImage.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        FadeImage.color = c;
        yield return new WaitForSeconds(0.5f);

        text.SetActive(true);
        texttype.StartButton();
    }
}
