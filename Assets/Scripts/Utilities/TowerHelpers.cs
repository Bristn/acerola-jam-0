using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TowerHelpers
{
    public static TowerInformation[] Towers = new TowerInformation[] {
        new() // Default Tower
        {
            Cost = 25,
            Radius = 5f,
            BulletVelocity = 25f,
            BulletCountPerShot = 1,
            BulletRandomness = 0.1f,
            FireCooldown = 1,
        },
        new() // Sniper Tower
        {
            Cost = 50,
            Radius = 10f,
            BulletVelocity = 50f,
            BulletCountPerShot = 1,
            BulletRandomness = 0.0f,
            FireCooldown = 2,
        },
        new() // Shotgun Tower 
        {
            Cost = 75,
            Radius = 3.5f,
            BulletVelocity = 15f,
            BulletCountPerShot = 5,
            BulletRandomness = 0.5f,
            FireCooldown = 2,
        },
        new() // Rifle Tower
        {
            Cost = 100,
            Radius = 7f,
            BulletVelocity = 25f,
            BulletCountPerShot = 1,
            BulletRandomness = 0.25f,
            FireCooldown = 0.25f,
        },
    };

    public struct TowerInformation
    {
        public int Cost;
        public float Radius;
        public float BulletVelocity;
        public int BulletCountPerShot;
        public float BulletRandomness;
        public float FireCooldown;
        public float TargettingTime;
    }
}