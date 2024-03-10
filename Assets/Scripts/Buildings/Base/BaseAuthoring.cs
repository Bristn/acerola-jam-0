using Buildings.Towers;
using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Buildings.Base
{
    public class BaseAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        [SerializeField][BoxGroup("Settings")] private int buildingResoruce;
        [SerializeField][BoxGroup("Settings")] private int ammoResoruce;
        [SerializeField][BoxGroup("Settings")] private int playerLifes;

        public class Baker : Baker<BaseAuthoring>
        {
            public override void Bake(BaseAuthoring authoring)
            {
                Debug.Log("BaseAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                this.AddComponent(entity, new BaseData
                {
                    BuildingResoruces = authoring.buildingResoruce,
                    AmmoResoruces = authoring.ammoResoruce,
                    PlayerLifes = authoring.playerLifes,
                });
            }
        }
    }
}