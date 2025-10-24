using UnityEngine;
using UnityEngine.UI;

public class AttackPointIconController : MonoBehaviour
{
    [SerializeField]
    private Image[] images; // 攻撃ポイントアイコンを保持する配列.

    [SerializeField]
    private Sprite existPoint, noPoint;

    private void Start()
    {

    }

    public void AttackPointSet(int point)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i + 1 <= point)
            {
                images[i].sprite = existPoint;
            }
            else
            {
                images[i].sprite = noPoint;
            }
        }
    }
}
