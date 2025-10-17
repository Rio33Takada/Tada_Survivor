using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas mainCanvas; // UI表示キャンバス.

    [SerializeField]
    private Text remainEnemyCountText, // 残りの敵の数を表示するテキスト.
                waveCountText;       // 現在のウェーブ数を表示するテキスト.

    [SerializeField]
    private GameObject attackPointIconsPrefab; // 攻撃ポイント表示オブジェクト(プレハブ).

    private GameObject attackPointIcons; // 攻撃ポイント表示オブジェクト.

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// UI初期化
    /// </summary>
    public void InitializeUI(GameManager gm)
    {
        // 攻撃ポイント表示オブジェクト生成.
        if (attackPointIcons != null)
        {
            Destroy(attackPointIcons);
        }
        attackPointIcons = Instantiate(attackPointIconsPrefab);
        attackPointIcons.transform.SetParent(mainCanvas.transform);

        // 攻撃ポイント表示更新.
        SetSkillPoint(gm.AttackPoint);

        // 残り敵数表示更新.


        // 現在のウェーブ数表示更新.
        SetWaveCountText(gm.WaveCount);
    }

    public void SetSkillPoint(int point)
    {
        attackPointIcons.GetComponent<AttackPointIconController>().AttackPointSet(point);
    }

    public void SetRemainEnemyCountText(int count)
    {
        remainEnemyCountText.text = "残り" + count.ToString() + "体";
    }

    public void SetWaveCountText(int count)
    {
        waveCountText.text = count.ToString() + "ウェーブ目";
    }
}
