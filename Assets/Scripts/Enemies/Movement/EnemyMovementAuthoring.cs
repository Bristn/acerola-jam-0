using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Enemies
{
    public class EnemyMovementAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        [SerializeField][BoxGroup("Settings")] private float playerDetectionRange;
        [SerializeField][BoxGroup("Settings")] private float passiveMovementRadius;

        public class Baker : Baker<EnemyMovementAuthoring>
        {
            public override void Bake(EnemyMovementAuthoring authoring)
            {
                Debug.Log("EnemyMovementAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new EnemyMovementData
                {
                    PlayerDetectionRange = authoring.playerDetectionRange,
                    BaseDetectionRange = Helpers.EnemySpawnDistance + Helpers.EnemySpawnRandomness * 1.5f,
                    PassiveMoveRadius = authoring.passiveMovementRadius,
                });
            }
        }
    }
}