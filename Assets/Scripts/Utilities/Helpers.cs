using Cameras;
using Cameras.Targets;
using Common;
using Enemies;
using Players;
using Unity.Entities;
using static InterfaceBehaviour;

public static class Helpers
{
    public static float EnemySpawnRandomness = 5;
    public static float EnemySpawnDistance = 20;

    public static void StartEnemyWaveSpawner()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<EnemyWaveSpawnerEnableData>();
        InterfaceBehaviour.Instance.SetElementVisible(Element.START_WAVE, false);
        InterfaceBehaviour.Instance.SetElementVisible(Element.TOWER_CARDS, false);
        InterfaceBehaviour.Instance.SetElementVisible(Element.BUILDING_RESOURCES, false);
        InterfaceBehaviour.Instance.SetElementVisible(Element.GENERIC_HINT, false);
        InterfaceBehaviour.ChangedGenericHint.Invoke(string.Empty);
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
            EntityQuery query = entityManager.CreateEntityQuery(new ComponentType[] { typeof(PauseTimeData) });
            if (query.TryGetSingletonEntity<PauseTimeData>(out Entity _))
            {
                return;
            }

            entityManager.CreateSingleton<PauseTimeData>();
        }
        else
        {
            EntityQuery query = entityManager.CreateEntityQuery(new ComponentType[] { typeof(ResumeTimeData) });
            if (query.TryGetSingletonEntity<ResumeTimeData>(out Entity _))
            {
                return;
            }

            entityManager.CreateSingleton<ResumeTimeData>();
        }
    }

    public static void RecenterCamera()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = entityManager.CreateEntityQuery(new ComponentType[] { typeof(CameraRecenterData) });
        if (query.TryGetSingletonEntity<CameraRecenterData>(out Entity _))
        {
            return;
        }

        entityManager.CreateSingleton(new CameraRecenterData()
        {
            Speed = 4f
        });
    }

    public static void StartTimer()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.CreateSingleton<RemainingTimeData>();
    }
}