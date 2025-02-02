using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class NavBaker : MonoBehaviour
{
    public NavMeshSurface navisMesh;

    private void OnValidate()
    {
        navisMesh = GetComponent<NavMeshSurface>();
    }
    public void Initialize()
    {
        navisMesh.BuildNavMesh();
    }
}
