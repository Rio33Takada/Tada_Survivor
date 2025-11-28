using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearTurnText : MonoBehaviour
{
    string textname;
    public float fadetime;
    public GameObject next_text;

    public Text numberText; // Text コンポーネントをアサイン
    public int targetNumber; // 最終的に表示する目標の数字(ターン数)
    public float duration = 1.2f; // 目標に到達するまでの時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textname = numberText.text;
        StartCoroutine(Fadeout());
    }

    public IEnumerator Fadeout()
    {
        float t = 0;
        float targetAlpha = 1f;
        Color c = numberText.color;
        c.a = 0f;

        while (t < fadetime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, t / fadetime);
            numberText.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        numberText.color = c;
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(DisplayRandomNumbers());
    }

    IEnumerator DisplayRandomNumbers()
    {
        float timeElapsed = 0f;
        int startNumber = 0; // 開始時に表示する数字
        string targetNumberStr = targetNumber.ToString(); // 目標の数字を文字列に変換

        // ランダムに数字を表示しながら、最終的に目標の数字を表示
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            // ランダムな数字を生成して表示
            string randomNumber = GenerateRandomNumber(targetNumberStr.Length);
            numberText.text = textname + randomNumber;

            // 少し待つ
            yield return null;
        }

        // 最後に目標の数字を表示
        numberText.text = textname + targetNumberStr;

        //次の文章表示
        yield return new WaitForSeconds(1f);
        next_text.SetActive(true);
    }

    // ランダムな数字を生成する関数
    string GenerateRandomNumber(int length)
    {
        string randomNumber = "";
        for (int i = 0; i < length; i++)
        {
            randomNumber += Random.Range(0, 10).ToString();
        }
        return randomNumber;
    }
}
