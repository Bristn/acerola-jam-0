using System;
using Buildings.Base;
using Buildings.Towers;
using Pathfinding;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

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
            RequireForUpdate<EnemySpawnerData>();

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