using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TowerHelpers
{
    public static TowerInformation[] Towers = new TowerInformation[] {
        new()
        {
            Cost = 50,
            BulletVelocity = 5,
            FireCooldown = 2,
            Radius = 3,
            TargettingTime = 2,
        },
        new()
        {
            Cost = 50,
            BulletVelocity = 5,
            FireCooldown = 2,
            Radius = 5,
            TargettingTime = 2,
        },
        new()
        {
            Cost = 100,
            BulletVelocity = 5,
            FireCooldown = 2,
            Radius = 2,
            TargettingTime = 2,
        },
        new()
        {
            Cost = 100,
            BulletVelocity = 5,
            FireCooldown = 2,
            Radius = 3,
            TargettingTime = 2,
        },
    };

    public struct TowerInformation
    {
        public int Cost;
        public float FireCooldown;
        public float TargettingTime;
        public float BulletVelocity;
        public float Radius;
    }
}