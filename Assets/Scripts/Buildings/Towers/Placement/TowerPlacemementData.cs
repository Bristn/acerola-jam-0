
using Unity.Entities;
using Unity.Mathematics;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct TowerPlacemementData : IComponentData
    {
        public bool ShowPlacement;
        public int TowerType;
        public int2 CellIndex;
    }
}