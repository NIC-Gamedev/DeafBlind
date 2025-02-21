using System;
using FishNet.Object;
using Kartograph.Entities;
using UnityEngine;

public class LevelSpawner : NetworkBehaviour
{
    public Action OnLevelGeneratorComplete;
    public LevelGenerator3D levelGenerator;
    protected override void OnValidate()
    {
        if (!levelGenerator)
            levelGenerator = FindAnyObjectByType<LevelGenerator3D>();
    }
    public void GenerateLevel()
    {
        if(levelGenerator)
            levelGenerator.Generate(OnLevelGeneratorComplete);
    }
}
