
using Unity.Entities;

namespace Cameras
{
    [System.Serializable]
    public struct CameraRecenterData : IComponentData
    {
        public float Speed;
    }
}