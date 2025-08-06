using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Scriptable Objects/LevelConfiguration")]
public class LevelConfiguration : ScriptableObject
{
    [System.Serializable]
    public class MapData
    {
        public string mapName;
        public GameObject mapPrefab;
        public Vector3 spawnPosition = Vector3.zero;
    }

    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public List<MapData> maps = new List<MapData>();
    }

    [Header("Game Levels Configuration")] public List<LevelData> levels = new List<LevelData>();
}