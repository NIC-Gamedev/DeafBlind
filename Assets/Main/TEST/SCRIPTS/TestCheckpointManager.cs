using UnityEngine;

public class TestCheckpointManager : MonoBehaviour
{
    public Transform[] points;

    public static TestCheckpointManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        points = GetComponentsInChildren<Transform>();
    }
}
