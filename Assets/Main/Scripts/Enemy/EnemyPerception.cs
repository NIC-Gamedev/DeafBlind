using System.Collections.Generic;
using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("Настройки зрения")]
    [SerializeField] private float visionRange = 10f; // Радиус зрения
    [SerializeField] private float visionAngle = 90f; // Угол зрения (половина поля зрения)
    [SerializeField] private LayerMask detectableLayers; // Слои для обнаружения объектов

    [Header("Отладка")]
    [SerializeField] private List<GameObject> visibleObjects = new List<GameObject>(); // Список объектов в поле зрения (видимый в инспекторе)
    // Ссылка на StateController для смены состояний
    private StateController stateController;

    private float viewAngle => Camera.main.fieldOfView * 1.5f;

    public Vector3 playerLastSeenPos;

    private void Start()
    {
        // Получаем ссылку на StateController
        stateController = GetComponent<StateController>();
    }

    // Метод для получения всех объектов в поле зрения
    public List<GameObject> GetVisibleObjects()
    {
        visibleObjects.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, detectableLayers); // Проверяем объекты в радиусе зрения

        foreach (Collider hit in hits)
        {
            if (hit != null && IsInVisionAngle(hit.transform) && HasLineOfSight(hit))
            {
                visibleObjects.Add(hit.gameObject);
            }  
        }
        return visibleObjects;
    }

    private bool IsInVisionAngle(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        return angleToTarget <= visionAngle; // Проверяем, в поле зрения ли объект
    }

    private bool HasLineOfSight(Collider target)
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position + Vector3.up/3, directionToTarget, out RaycastHit hit, visionRange, ~(1 << gameObject.layer))) //Побитовое вычесление маски, нужнл что бы она брала все слои кроме одно
        {
            return hit.collider == target;
        }
        return false;
    }
    public bool IsPlayerLookingAtMe(Transform player)
    {
        Vector3 directionToEnemy = (transform.position - player.position).normalized;

        float angle = Vector3.Angle(player.forward, directionToEnemy);
        if (angle > viewAngle / 2f)
            return false;

        float distanceToEnemy = Vector3.Distance(player.position, transform.position);
        if (distanceToEnemy > visionRange)
            return false;

        if (Physics.Raycast(player.position, directionToEnemy, distanceToEnemy, ~(1 << gameObject.layer)))
        {
            return false;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        // Рисуем радиус и угол зрения для визуализации
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * transform.forward * visionRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
  