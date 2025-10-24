using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    private CommandController commandController;

    private List<GameObject> buttons = new List<GameObject>(); // ボタンリスト.

    public GameObject buttonPrefab; // ボタン(プレハブ).
    private GameObject normalAttack, skillAttack, trapAttack; // ボタンGameObject.

    [SerializeField]
    private Canvas mainCanvas; // UI表示キャンバス.

    [SerializeField]
    private Text remainEnemyCountText, // 残りの敵の数を表示するテキスト.
                 waveCountText;       // 現在のウェーブ数を表示するテキスト.

    [SerializeField]
    private GameObject attackPointIconsPrefab; // 攻撃ポイント表示オブジェクト(プレハブ).

    private GameObject attackPointIcons; // 攻撃ポイント表示オブジェクト(インスタンス).

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void ClassInitializer()
    {
        commandController = new CommandController();
    }

    /// <summary>
    /// UI初期化
    /// </summary>
    public void InitializeUI(GameManager gm)
    {
        // クラス初期化.
        ClassInitializer();

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

        CreateCommandButton();
    }

    public void CreateCommandButton()
    {
        normalAttack = Instantiate(buttonPrefab);
        normalAttack.transform.SetParent(mainCanvas.transform);
        buttons.Add(normalAttack);

        normalAttack.transform.position = normalAttack.transform.position + new Vector3(100, 100, 0);
        normalAttack.GetComponent<Button>().onClick.AddListener(() => commandController.OnNormalAttackSelected());

        skillAttack = Instantiate(buttonPrefab);
        skillAttack.transform.SetParent(mainCanvas.transform);
        buttons.Add(skillAttack);

        skillAttack.transform.position = skillAttack.transform.position + new Vector3(300, 100, 0);
        skillAttack.GetComponent<Button>().onClick.AddListener(() => commandController.OnSkillAttackSelected());

        trapAttack = Instantiate(buttonPrefab);
        trapAttack.transform.SetParent(mainCanvas.transform);
        buttons.Add(trapAttack);

        trapAttack.transform.position = trapAttack.transform.position + new Vector3(700, 100, 0);
        trapAttack.GetComponent<Button>().onClick.AddListener(() => commandController.OnSetTrapSelected());
    }

    public void DeleteCommandButton()
    {
        foreach (var button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
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
