using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Title_StartButton : MonoBehaviour
{
    public List<Text> text = new List<Text>();
    public float delay;
    private string Text;


    private void Start()
    {
        Text = text[0].text;
        text[0].text = "";
    }
    //ƒƒCƒ“ƒV[ƒ“‚Ö‚Ì‘JˆÚ
    public void OnTitileButton()
    {
        Debug.Log("Start");
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        foreach(char c in Text)
        {
            text[0].text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}
