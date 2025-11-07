using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Title_StartButton : MonoBehaviour
{
    public List<Text> text = new List<Text>();
    public float delay { get; private set; } //文章の表示スピード
    public GameObject Textobj;   //
    public GameObject FadeImage; //フェードイメージを入れる
    private string Text;
    private bool IsAccela = false;


    private void Start()
    {
        delay = 0.05f;
        Text = text[0].text;
        text[0].text = "";
    }
    //ストーリー（？）文章画面表示
    public void OnTitileButton()
    {
        Debug.Log("Start");
        StartCoroutine(ShowText());
        IsAccela = true;
        Textobj.SetActive(true);
        FadeImage.SetActive(true);
    }

    IEnumerator ShowText()
    {
        foreach(char c in Text)
        {
            text[0].text += c;
            yield return new WaitForSeconds(delay);
        }
    }

    private void Update()
    {
        if(IsAccela)
        {
            if(Input.GetMouseButton(0))
            {
                delay = 0f;
                IsAccela = false;
            }
        }
    }
}
