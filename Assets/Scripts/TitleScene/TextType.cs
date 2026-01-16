using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextType : MonoBehaviour
{
    public Text messageText;
    public float TypeSpeed = 0.04f; //表示速度
    //public float fadetime = 1f;
    //public Image fadeimage;
    private bool isTyping;
    private bool isTyped;
    [SerializeField]
    private int currentIndex = 0;
    private string[] message =
    {
        //\nで改行
        "かつて、この大陸の中央には \r\n強大な国家――バシコン帝国が君臨していた。",
        "帝国を治めるのは、\r\nバシ・コシコーン三世。\r\n彼は己の欲望こそが正義と信じる、\r\n独善的かつ冷酷な帝王であった。",
        "些細な失言すら「不敬」として切り捨てられ、\r\n気に入らぬ家臣や民は即刻処刑。\r\nさらに、生活を顧みぬ重税が課され、\r\n帝国の政治は完全な独裁へと堕ちていく。",
        "街は活気を失い、畑は枯れ、\r\n飢えに倒れる民が後を絶たなかった。\r\nかつて繁栄を誇った帝国は、\r\n今や廃墟と絶望に覆われていた。",
        "しかし―― \r\n闇が深まるとき、必ず影から立ち上がる者がいる。",
        "国を救うため、\r\nたったひとり……いや、一匹の男が動き出した。",
        "その名は――タダン。",
        "悪徳貴族の屋敷に忍び込み、\r\n不正に蓄えられた財宝を奪い、\r\nそれを苦しむ民へと還す義賊だった。",
        "帝国に仇なす反逆者か。\r\nそれとも、救国の英雄か。",
        "バシコン帝国の闇を切り裂く\r\n一匹の義賊から始まる――。",
        ""
    };

    //タイトルのボタンが押されたら
    public void StartButton()
    {
        StartCoroutine(TypeText(message[currentIndex]));
        currentIndex++;
    }

    //テキスト表示処理
    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        isTyped = false;

        messageText.text = "";
        foreach (char c in fullText)
        {
            messageText.text += c;
            yield return new WaitForSeconds(TypeSpeed);
        }

        isTyped = true;
        isTyping = false;
    }

    //フェードアウト
    //public IEnumerator FadeOut()
    //{
    //    float t = 0;
    //    Color fadeC = fadeimage.color;

    //    while (t < fadetime) 
    //    {
    //        t += Time.deltaTime;
    //        fadeC.a = Mathf.Lerp(1f, 0f, t / fadetime);
    //        fadeimage.color = fadeC;
    //        yield return null;
    //    }
    //}

    void Update()
    {
        if ((currentIndex <= message.Length - 1))
        {
            //早送り
            if (isTyping == true && isTyped == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    TypeSpeed = 0f;
                }
            }
            //次の文へ
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    TypeSpeed = 0.04f;
                    StartCoroutine(TypeText(message[currentIndex]));
                    currentIndex++;
                }
            }
        }
        else
        {
            StartCoroutine(delay());
        }

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("HowToPlayScene");
        }
    }
}
