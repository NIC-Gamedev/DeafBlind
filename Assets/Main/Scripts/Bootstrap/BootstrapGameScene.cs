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
    
    public void Awake()
    {
        base.OnStartServer();
        serviceLocator.Initialize();
        levelSpawner.OnLevelGeneratorComplete += InitAfterGenerateComplete;
        levelSpawner.GenerateLevel();
    }

    public override void OnStartServer()
    {
        base.OnStartServer(); 
        enemySpawner.Initialize(mapManager.mapData);
    }
    private void InitAfterGenerateComplete()
    {
        StartCoroutine(InitAfterGenerateCompleteIE());
    }

    private IEnumerator InitAfterGenerateCompleteIE()
    {
        yield return new WaitForEndOfFrame(); 
        navBaker.Initialize();
        mapManager.Initialize(levelSpawner.levelGenerator);

        RegisterServices();
    }


    public void RegisterServices()
    {
        serviceLocator.Register(mapManager);
    }
}
