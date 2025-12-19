using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabHolder", menuName = "Game/Enemy Prefab Holder")]
public class EnemyPrefabHolder : ScriptableObject
{
    [Serializable]
    public class EnemyPrefabPair
    {
        public EnemyType type;
        public GameObject prefab;
    }

    [SerializeField]
    private List<EnemyPrefabPair> enemyPrefabs = new List<EnemyPrefabPair>();

    private Dictionary<EnemyType, GameObject> prefabDictionary;

    public GameObject GetPrefab(EnemyType type)
    {
        if (prefabDictionary == null)
        {
            prefabDictionary = new Dictionary<EnemyType, GameObject>();
            foreach (var pair in enemyPrefabs)
            {
                if (!prefabDictionary.ContainsKey(pair.type))
                {
                    prefabDictionary.Add(pair.type, pair.prefab);
                }
            }
        }

        if (prefabDictionary.TryGetValue(type, out var prefab))
        {
            return prefab;
        }

        Debug.LogError($"EnemyType {type} ‚Ì Prefab ‚ª“o˜^‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
        return null;
    }
}
