using System;
using Unity.Entities;

namespace Buildings.Base
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BaseSystem : SystemBase
    {
        public static Action<int, int> BuildingResourcesUpdated;
        public static Action<int, int> AmmoResourcesUpdated;
        public static Action<int, int> PlayerLifesUpdated;

        public static BaseData currentData { get; private set; }

        protected override void OnCreate()
        {
            this.RequireForUpdate<BaseData>();
        }

        protected override void OnUpdate()
        {
            BaseData baseData = SystemAPI.GetSingleton<BaseData>();

            if (baseData.AmmoResoruces != currentData.AmmoResoruces)
            {
                AmmoResourcesUpdated?.Invoke(currentData.AmmoResoruces, baseData.AmmoResoruces);
            }

            if (baseData.BuildingResoruces != currentData.BuildingResoruces)
            {
                BuildingResourcesUpdated?.Invoke(currentData.BuildingResoruces, baseData.BuildingResoruces);
            }

            if (baseData.PlayerLifes != currentData.PlayerLifes)
            {
                PlayerLifesUpdated?.Invoke(currentData.PlayerLifes, baseData.PlayerLifes);
            }

            currentData = baseData;
        }
    }
}