using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavBaker : MonoBehaviour
{
    public NavMeshSurface NavisMesh;
    // Start is called before the first frame update
    void Start()
    {
        NavisMesh = GetComponent<NavMeshSurface>();
        NavisMesh.BuildNavMesh();
        StartCoroutine(NavMeshUpdate());

    }

    IEnumerator NavMeshUpdate()
    {
        var data = NavisMesh.navMeshData;
        yield return new WaitForSeconds(3f);

        NavisMesh.UpdateNavMesh(data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
