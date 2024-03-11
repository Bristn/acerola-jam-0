using Unity.Entities;
using UnityEngine;

namespace Players
{
    public class PlayerMovementAuthoring : MonoBehaviour
    {
        public class Baker : Baker<PlayerMovementAuthoring>
        {
            public override void Bake(PlayerMovementAuthoring authoring)
            {
                Debug.Log("PlayerMovementAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PlayerMovementData { });
            }
        }
    }
}