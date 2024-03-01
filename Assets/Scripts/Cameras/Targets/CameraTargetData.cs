
using Unity.Entities;

namespace Cameras.Targets
{
    /// <summary>
    /// The camera is going to copy the transform of ths entity.
    /// Only one entity should have this component
    /// </summary>
    public struct CameraTargetData : IComponentData { }
}