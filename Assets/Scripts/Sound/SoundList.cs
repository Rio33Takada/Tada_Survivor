using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "Game/SoundList")]
public class SoundList : ScriptableObject
{
    public AudioClip title;
    public AudioClip grassLand;
    public AudioClip volcano;
    public AudioClip gameClear;
    public AudioClip gameOver;
}
