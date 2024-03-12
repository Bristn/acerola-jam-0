using Unity.Entities;
using Unity.Mathematics;

namespace Enemies
{
    public partial struct EnemiesToSpawnBuffer : IBufferElementData
    {
        public float2 Center;
        public float Delay;
    }
}