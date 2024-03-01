using Unity.Entities;
using UnityEngine;

namespace Buildings
{
    public class BuildingAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        public class Baker : Baker<BuildingAuthoring>
        {
            public override void Bake(BuildingAuthoring authoring)
            {
                Debug.Log("BuildingAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new BuildingData { });
            }
        }
    }
}