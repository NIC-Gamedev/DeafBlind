using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_TriggerZone : MonoBehaviour
{
    public List<GameObject> objectsInZone = new List<GameObject>();

    // �����, ���������� ��� ����� ������� � ���������� ����
    private void OnTriggerEnter(Collider other)
    {
        // ���������, ���� �� � ������� ��������� "Health"
        if (other.GetComponent<ObjectHealth>() != null)
        {
            // ��������� ������ � ������
            objectsInZone.Add(other.gameObject);
            
        }
    }

    // �����, ���������� ��� ������ ������� �� ���������� ����
    private void OnTriggerExit(Collider other)
    {
        // ���� ������ �������� ����, ������� ��� �� ������
        if (objectsInZone.Contains(other.gameObject))
        {
            objectsInZone.Remove(other.gameObject);
            
        }
    }

    // ����� ��� ��������� ���� �������� � ����
    public List<GameObject> GetObjectsInZone()
    {
        return objectsInZone;
    }
}
