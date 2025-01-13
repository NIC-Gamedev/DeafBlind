using Kartograph.Entities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class BootstrapGameScene : MonoBehaviour
{
    public ServiceLocator serviceLocator;
    public NavBaker navBaker;
    public LevelGenerator3D levelGenerator;

    public Action OnLevelGeneratorComplete;
    public EnemySpawner enemySpawner;
    public MapManager mapManager;

    private void OnValidate()
    {
        serviceLocator = GetComponent<ServiceLocator>();
        navBaker = FindAnyObjectByType<NavBaker>();
        levelGenerator = FindAnyObjectByType<LevelGenerator3D>();
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
        mapManager = FindAnyObjectByType<MapManager>();
    }
    private void Awake()
    {
        serviceLocator.Initialize();
        OnLevelGeneratorComplete += InitAfterGenerateComplete;
        levelGenerator.Generate(OnLevelGeneratorComplete);
    }
    private void InitAfterGenerateComplete()
    {
        StartCoroutine(InitAfterGenerateCompleteIE());
    }

    private IEnumerator InitAfterGenerateCompleteIE()
    {
        yield return new WaitForEndOfFrame(); 
        navBaker.Initialize();
        mapManager.Initialize(levelGenerator);
        enemySpawner.Initialize(mapManager.mapData);

        RegisterServices();
    }


    public void RegisterServices()
    {
        serviceLocator.Register(mapManager);
    }
}
