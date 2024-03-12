using Unity.Entities;
using UnityEngine;

namespace Enemies
{
    public class EnemyWaveSpawnerAuthoring : MonoBehaviour
    {
        public class Baker : Baker<EnemyWaveSpawnerAuthoring>
        {
            public override void Bake(EnemyWaveSpawnerAuthoring authoring)
            {
                Debug.Log("EnemyWaveSpawnerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new EnemyWaveSpawnerData
                {
                    // Instantly spawn first wave
                    TotalWaveCooldown = 5,
                    CurrentWaveCooldown = 5,

                    EnemiesPerWave = 3,
                    MaxEnemySpawnDelay = 2,
                });
            }
        }
    }
}