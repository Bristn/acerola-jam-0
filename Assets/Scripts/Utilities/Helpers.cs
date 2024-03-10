using Enemies;
using Players;
using Unity.Entities;
using static InterfaceBehaviour;

public static class Helpers
{
    public static void StartEnemySpawner()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<EnemySpawnerEnableData>();
        InterfaceBehaviour.Instance.SetElementVisible(Element.START_WAVE, false);
    }

    public static void EnablePlayerMoevemnt()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<PlayerMovementEnableData>();
    }
}