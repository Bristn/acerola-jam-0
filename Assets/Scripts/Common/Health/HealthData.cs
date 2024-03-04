
using Unity.Entities;

namespace Common.Health
{
    [System.Serializable]
    public struct HealthData : IComponentData
    {
        public float BaseHealth;
        public float CurrentHealth;
    }
}