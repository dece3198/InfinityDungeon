using UnityEngine;

[System.Serializable]
public class MonsterSpawn
{
    public GameObject monsterPrefab;
    public int index;
}


[CreateAssetMenu(fileName = "New Stage", menuName = "New Stage/Stage")]
public class Stage : ScriptableObject
{
    public MonsterSpawn[] monsterSpawns;
}
