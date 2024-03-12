using System;
using Buildings.Towers;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Enemies
{
    public partial class EnemyWaveSystem : SystemBase
    {
        public static Action BeatFirstWave;
        private bool isFirstWave;
        private bool hadEnemies;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("EnemyWaveSystem: OnCreate");
            RequireForUpdate<EnemyWaveSpawnerData>();

            this.isFirstWave = true;
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            if (!this.isFirstWave)
            {
                return;
            }

            bool hasEnemies = this.HasEnemies();
            if (this.hadEnemies && !hasEnemies)
            {
                BeatFirstWave?.Invoke();
                this.isFirstWave = false;
                return;
            }

            this.hadEnemies = hasEnemies;
        }

        private bool HasEnemies()
        {
            foreach (var _ in SystemAPI.Query<RefRW<EnemyData>>())
            {
                return true;
            }

            return false;
        }
    }
}