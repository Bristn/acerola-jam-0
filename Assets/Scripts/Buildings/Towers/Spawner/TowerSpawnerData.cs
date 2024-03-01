
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct TowerSpawnerData : IComponentData
    {
        public Entity Prefab;
    }
}