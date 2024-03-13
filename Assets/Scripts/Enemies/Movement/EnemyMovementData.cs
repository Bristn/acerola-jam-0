
using Unity.Entities;

namespace Enemies
{
    [System.Serializable]
    public struct EnemyMovementData : IComponentData
    {
        public float BaseDetectionRange;
        public float PlayerDetectionRange;
        public float PassiveMoveRadius;
        public float PassiveMovementDelay;
        public bool AllowPassiveMovement;

        public void ReducePassiveMoveDelay(float deltaTime)
        {
            float newTime = this.PassiveMovementDelay - deltaTime;
            if (newTime <= 0)
            {
                this.AllowPassiveMovement = true;
                return;
            }

            this.PassiveMovementDelay = newTime;
        }
    }
}