using Common;
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
        InterfaceBehaviour.Instance.SetElementVisible(Element.TOWER_CARDS, false);
        InterfaceBehaviour.Instance.SetElementVisible(Element.BUILDING_RESOURCES, false);
    }

    public static void EnablePlayerMovement()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<PlayerMovementEnableData>();
    }

    public static void SpawnNewPlayer()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<PlayerSpawnerEnableData>();
    }

    public static void SetGamePaused(bool paused)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (paused)
        {
            entityManager.CreateSingleton<PauseTimeData>();
        }
        else
        {
            entityManager.CreateSingleton<ResumeTimeData>();
        }
    }

    public static void StartTimer()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<RemainingTimeData>();
    }
}