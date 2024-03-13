using Unity.Entities;
using UnityEngine;

namespace Enemies
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                Debug.Log("EnemyAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new EnemyData
                {
                });
            }
        }
    }
}