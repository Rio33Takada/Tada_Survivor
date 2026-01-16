using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource source;

    public SoundList soundList;

    public enum BGMType
    {
        Title,
        GrassLand,
        Volcano,
        GameClear,
        GameOver,
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
}
