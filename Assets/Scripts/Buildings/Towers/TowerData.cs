
using Unity.Entities;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct TowerData : IComponentData
    {
        public float Radius;
        public float TotalFireCooldown;
        public float CurrentFireCooldown;
        public bool CanFire;

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
    }
}