
using Unity.Entities;

namespace Players
{
    public struct PlayerSpawnerData : IComponentData
    {
        public Entity Prefab;
    }
}