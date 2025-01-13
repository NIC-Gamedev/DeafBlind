using Kartograph.Entities;
using UnityEngine;

public class MapManager : MonoBehaviour,IService
{
   [SerializeField] public MapData mapData;

    public void Initialize(LevelGenerator3D levelGenerator)
    {
        mapData = new MapData(levelGenerator);
    }

    private void OnDestroy()
    {
        mapData = null;
    }
}
