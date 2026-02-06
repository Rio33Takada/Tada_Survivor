using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class audio : MonoBehaviour
{
    public SoundManager SoundManager;

    private void Awake()
    {
        SoundManager = GameObject.Find("BGM").GetComponent<SoundManager>();
    }
    public void OnAudio()
    {
        SoundManager.PlaySE(SoundManager.SEType.Select);
    }
}
