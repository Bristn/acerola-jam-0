using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(TowerSystem))]
    public partial struct TowerTargetVisualiserSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TowerSystem: OnCreate");
            state.RequireForUpdate<TowerTargetVisualiserData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (targetTransform, target) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<TowerTargetVisualiserData>>())
            {
                targetTransform.ValueRW.Position = target.ValueRO.Position;
            }
        }
    }
}