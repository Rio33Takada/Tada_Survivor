using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ClearText : MonoBehaviour
{
    public float fadetime;
    public Text text;
    public GameObject next_text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Fadeout());
    }

    public IEnumerator Fadeout()
    {
        float t = 0;
        float targetAlpha = 1f;
        Color c = text.color;
        c.a = 0f;

        while (t < fadetime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, t / fadetime);
            text.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        text.color = c;
        yield return new WaitForSeconds(0.4f);
        next_text.SetActive(true);
    }
}
