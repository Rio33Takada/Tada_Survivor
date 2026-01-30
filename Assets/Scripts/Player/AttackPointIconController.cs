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
        Debug.Log($"[AttackPointIcon] point = {point}");

        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = (i + 1 <= point) ? existPoint : noPoint;
        }
    }

}
