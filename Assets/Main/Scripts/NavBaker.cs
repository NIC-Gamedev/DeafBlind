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

        StartCoroutine(NavMeshUpdate());

    }

    IEnumerator NavMeshUpdate()
    {
        yield return new WaitForSeconds(1f);

        NavisMesh.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
