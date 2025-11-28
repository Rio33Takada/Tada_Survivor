using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClearBackGround : MonoBehaviour
{
    public float fadetime = 2f;
    public Image fadeImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Fadeout());
    }

    public IEnumerator Fadeout()
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < fadetime)
        {
            if(c.a < 180)
            {
                t += Time.deltaTime;
                c.a = t / fadetime;
                fadeImage.color = c;
                yield return null;
            }
            
            
        }
    }
}
