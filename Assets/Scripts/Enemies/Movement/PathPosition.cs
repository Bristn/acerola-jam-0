using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(20)]
public partial struct PathPosition : IBufferElementData
{
    public int2 Position;
}