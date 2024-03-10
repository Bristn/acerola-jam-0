
using System;
using Unity.Entities;

namespace Buildings.Base
{
    [System.Serializable]
    public struct BaseData : IComponentData
    {
        public int BuildingResoruces;

        public int AmmoResoruces;
        public int PlayerLifes;
    }
}