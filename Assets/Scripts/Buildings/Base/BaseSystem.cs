using System;
using Unity.Entities;

namespace Buildings.Base
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BaseSystem : SystemBase
    {
        public static Action<int> BuildingResourcesUpdated;
        public static Action<int> AmmoResourcesUpdated;

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
                AmmoResourcesUpdated?.Invoke(baseData.AmmoResoruces);
            }

            if (baseData.BuildingResoruces != currentData.BuildingResoruces)
            {
                BuildingResourcesUpdated?.Invoke(baseData.BuildingResoruces);
            }

            currentData = baseData;
        }
    }
}