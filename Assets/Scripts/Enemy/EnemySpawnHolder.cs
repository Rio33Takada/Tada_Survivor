using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "EnemySpawnHolder", menuName = "Game/EnemySpawnHolder")]
public class EnemySpawnHolder : ScriptableObject
{
    public List<EnemySpawnData> spawnList;
}

[Serializable]
public struct EnemySpawnData
{
    public Vector2Int position;
    public EnemyType enemyType;
}