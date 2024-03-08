
using Unity.Entities;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct TowerData : IComponentData
    {
        public float Radius;
        public float BulletVelocity;
        public int BulletCountPerShot;

        /// <summary>
        /// Defines a radnom spread of the bullets. This value is used as the absolute max 
        /// of a random value which gets added to the position (Gets added to the target position)
        /// </summary>
        public float BulletRandomness;

        public float TotalFireCooldown;
        public float CurrentFireCooldown;
        public bool CanFire;

        public float TotalTargettingTime;
        public float CurrentTargettingTime;
        public bool HasTarget;


        public void ReduceFireCooldown(float deltaTime)
        {
            float newTime = this.CurrentFireCooldown - deltaTime;
            if (newTime <= 0)
            {
                this.CurrentFireCooldown = this.TotalFireCooldown;
                this.CanFire = true;
                return;
            }

            this.CurrentFireCooldown = newTime;
        }

        public void ReduceTargettingTime(float deltaTime)
        {
            float newTime = this.CurrentTargettingTime - deltaTime;
            if (newTime <= 0)
            {
                this.CurrentTargettingTime = this.TotalTargettingTime;
                this.HasTarget = true;
                return;
            }

            this.CurrentTargettingTime = newTime;
        }
    }
}