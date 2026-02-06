using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource source;

    public SoundList soundList;

    public SEList seList;

    public enum BGMType
    {
        Title,
        GrassLand,
        Volcano,
        GameClear,
        GameOver,
    }

    public enum SEType
    {
        ArcherAttack,
        BomberAttack,
        KnightAttack,
        PlayerAttack,
        Select,
        Select2,
    }

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayBGM(BGMType type)
    {
        switch (type)
        {
            case BGMType.Title:
                source.resource = soundList.title;
                source.Play();
                break;
            case BGMType.GrassLand:
                source.resource = soundList.grassLand;
                source.Play();
                break;
            case BGMType.Volcano:
                source.resource = soundList.volcano;
                source.Play();
                break;
            case BGMType.GameClear:
                source.resource = soundList.gameClear;
                source.Play();
                break;
            case BGMType.GameOver:
                source.resource = soundList.gameOver;
                source.Play();
                break;
            default:
                break;
        }
    }

    public void PlaySE(SEType type)
    {
        switch (type)
        {
            case SEType.ArcherAttack:
                source.PlayOneShot(seList.ArcherAttack);
                break;
            case SEType.BomberAttack:
                source.PlayOneShot(seList.BomberAttack);
                break;
            case SEType.KnightAttack:
                source.PlayOneShot(seList.KnightAttack);
                break;
            case SEType.PlayerAttack:
                source.PlayOneShot(seList.PlayerAttack);
                break;
            case SEType.Select:
                source.PlayOneShot(seList.Select);
                break;
            case SEType.Select2:
                source.PlayOneShot(seList.Select2);
                break;
            default :
                break;
        }
    }
}
