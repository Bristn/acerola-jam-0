
using Unity.Entities;
using Unity.Mathematics;

namespace Enemies
{
    [System.Serializable]
    public struct EnemyWaveSpawnerData : IComponentData
    {
        public uint CurrentWaveCount;
        public uint EnemiesPerWave;
        public float MaxEnemySpawnDelay;
        public float TotalWaveCooldown;
        public float CurrentWaveCooldown;

        public bool ReduceWaveCooldown(float deltaTime)
        {
            float newTime = this.CurrentWaveCooldown + deltaTime;
            if (newTime >= this.TotalWaveCooldown)
            {
                this.CurrentWaveCooldown = 0;
                this.CurrentWaveCount++;
                this.AdjustValuesForNewWave();
                return true;
            }

            this.CurrentWaveCooldown = newTime;
            return false;
        }

        private void AdjustValuesForNewWave()
        {
            // Increase the enemy count of the waves gradually
            if (this.CurrentWaveCount % 3 == 0)
            {
                this.EnemiesPerWave++;
                this.EnemiesPerWave = math.min(15, this.EnemiesPerWave);
            }

            // Clamp spawn rate (With 5s start and 0.05s per wave: 50 waves to min)
            this.TotalWaveCooldown -= 0.05f;
            this.TotalWaveCooldown = math.max(2f, this.TotalWaveCooldown);
        }
    }
}