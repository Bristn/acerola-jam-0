
using Unity.Entities;

namespace Enemies
{
    [System.Serializable]
    public struct EnemyMovementData : IComponentData
    {
        public float BaseDetectionRange;
        public float PlayerDetectionRange;
    }
}