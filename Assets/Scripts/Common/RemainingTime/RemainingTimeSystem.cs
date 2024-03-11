using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Common.Time
{
    public partial class RemainingTimeSystem : SystemBase
    {
        // Time in seconds
        private float RemainingTime;
        public static Action<float> RemainingTimeChanged;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("RemainingTimeSystem: OnCreate");
            RemainingTime = 10 * 60;
            RequireForUpdate<RemainingTimeData>();
            RequireForUpdate<ResumeTimeData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            RemainingTime -= SystemAPI.Time.DeltaTime;
            RemainingTimeChanged.Invoke(RemainingTime);
        }
    }
}