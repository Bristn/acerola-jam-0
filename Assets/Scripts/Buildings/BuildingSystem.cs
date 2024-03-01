using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Buildings
{
    public partial struct BuildingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("BuildingSystem: OnCreate");
            state.RequireForUpdate<BuildingData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }
    }
}