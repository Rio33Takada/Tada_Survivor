using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeText : MonoBehaviour
{
    [SerializeField]
    private float fadetime;
    [SerializeField]
    private Image FadeImage;

    void Start()
    {
        StartCoroutine(Fadeout());
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
    }
}
