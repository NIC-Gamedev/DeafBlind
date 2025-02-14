using System.Collections;
using FishNet.Object;
using UnityEngine;

public class BootstrapGameScene : NetworkBehaviour
{
    public ServiceLocator serviceLocator;
    public NavBaker navBaker;
    public EnemySpawner enemySpawner;
    public MapManager mapManager;
    public LevelSpawner levelSpawner;

    protected override void OnValidate()
    {
        if(!serviceLocator)
            serviceLocator = GetComponent<ServiceLocator>();
        if(!navBaker)
            navBaker = FindAnyObjectByType<NavBaker>();
        if(!enemySpawner)
            enemySpawner = FindAnyObjectByType<EnemySpawner>();
        if(!mapManager)
            mapManager = FindAnyObjectByType<MapManager>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer(); 
        serviceLocator.Initialize();
    }
    private void InitAfterGenerateComplete()
    {
        StartCoroutine(InitAfterGenerateCompleteIE());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        levelSpawner.OnLevelGeneratorComplete += InitAfterGenerateComplete;
        levelSpawner.GenerateLevel();
    }

    private IEnumerator InitAfterGenerateCompleteIE()
    {
        yield return new WaitForEndOfFrame(); 
        navBaker.Initialize();
        mapManager.Initialize(levelSpawner.levelGenerator);
        if(IsServerInitialized)
            enemySpawner.Spawn(mapManager.mapData.allWayPoints[Random.Range(0, mapManager.mapData.allWayPoints.Count)].transform.position);
        RegisterServices();
    }


    public void RegisterServices()
    {
        serviceLocator.Register(mapManager);
    }
}
