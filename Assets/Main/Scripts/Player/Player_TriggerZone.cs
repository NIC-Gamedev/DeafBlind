using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_TriggerZone : MonoBehaviour
{
    public List<GameObject> objectsInZone = new List<GameObject>();

    // Метод, вызываемый при входе объекта в триггерную зону
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, есть ли у объекта компонент "Health"
        if (other.GetComponent<ObjectHealth>() != null)
        {
            // Добавляем объект в список
            objectsInZone.Add(other.gameObject);
            
        }
    }

    // Метод, вызываемый при выходе объекта из триггерной зоны
    private void OnTriggerExit(Collider other)
    {
        // Если объект покидает зону, убираем его из списка
        if (objectsInZone.Contains(other.gameObject))
        {
            objectsInZone.Remove(other.gameObject);
            
        }
    }

    // Метод для получения всех объектов в зоне
    public List<GameObject> GetObjectsInZone()
    {
        return objectsInZone;
    }
}
