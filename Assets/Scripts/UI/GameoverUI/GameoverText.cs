using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameoverText : MonoBehaviour
{
    public float fadetime;
    public Text gameoverText;
    public GameObject obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Fadeout());
    }

    public IEnumerator Fadeout()
    {
        float t = 0;
        float targetAlpha = 1f;
        Color c = gameoverText.color;
        c.a = 0f;

        while (t < fadetime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, t / fadetime);
            gameoverText.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        gameoverText.color = c;
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(true);
    }
}
