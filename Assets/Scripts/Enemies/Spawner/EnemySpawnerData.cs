
using Unity.Entities;

namespace Enemies
{
    [System.Serializable]
    public struct EnemySpawnerData : IComponentData
    {
        public Entity Prefab;
        public uint WaveCount;
        public float TotalWaveCooldown;
        public float CurrentWaveCooldown;


        public bool ReduceWaveCooldown(float deltaTime)
        {
            float newTime = this.CurrentWaveCooldown + deltaTime;
            if (newTime >= this.TotalWaveCooldown)
            {
                this.CurrentWaveCooldown = 0;
                this.WaveCount++;
                return true;
            }

            this.CurrentWaveCooldown = newTime;
            return false;
        }
    }
}