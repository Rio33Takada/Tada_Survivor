using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClearBackGround : MonoBehaviour
{
    public float fadetime;
    public Image fadeImage;
    public GameObject obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Fadeout());
    }

    public IEnumerator Fadeout()
    {
        float t = 0;
        float targetAlpha = 0.9f;
        Color c = fadeImage.color;
        c.a = 0f;

        while (t < fadetime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, t / fadetime);
            fadeImage.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        fadeImage.color = c;
        obj.SetActive(true);
    }
}
