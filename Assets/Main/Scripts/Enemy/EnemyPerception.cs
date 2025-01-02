using UnityEngine;
using System.Collections.Generic;

public class EnemyPerception : MonoBehaviour
{
    [Header("Настройки зрения")]
    [SerializeField] private float visionRange = 10f; // Радиус зрения
    [SerializeField] private float visionAngle = 90f; // Угол зрения (половина поля зрения)
    [SerializeField] private LayerMask detectableLayers; // Слои для обнаружения объектов

    [Header("Отладка")]
    [SerializeField] private List<GameObject> visibleObjects = new List<GameObject>(); // Список объектов в поле зрения (видимый в инспекторе)

    private StateController stateController; // Ссылка на контроллер состояний

    public GameObject lastSeenTarget; // Последняя видимая цель

    // Метод для получения всех объектов в поле зрения
    public List<GameObject> GetVisibleObjects()
    {
        visibleObjects.Clear(); // Очищаем список перед каждым обновлением
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, detectableLayers); // Проверяем объекты в радиусе зрения

        GameObject currentTarget = null; // Текущая цель

        foreach (Collider hit in hits)
        {
            if (hit != null && IsInVisionAngle(hit.transform) && HasLineOfSight(hit))
            {
                visibleObjects.Add(hit.gameObject); // Добавляем объект в список видимых

                // Если объект видим, устанавливаем его как текущую цель
                currentTarget = hit.gameObject;

                // Если цели нет, меняем состояние на Chase
                if (stateController != null && lastSeenTarget != currentTarget)
                {
                    lastSeenTarget = currentTarget;
                    stateController.SetState<ChaseState>(); // Переключаем состояние на Chase
                }
            }
        }

        // Если текущая цель исчезла, возвращаемся к патрулированию
        if (currentTarget == null && lastSeenTarget != null)
        {
            stateController.SetState<PatrolState>(); // Переключаем состояние на Patrol
            lastSeenTarget = null; // Сбрасываем последнюю цель
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
        if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, visionRange, detectableLayers))
        {
            return hit.collider == target; // Проверяем, нет ли преграды на пути
        }
        return false;
    }

    private void Awake()
    {
        // Получаем ссылку на StateController
        stateController = GetComponent<StateController>();
    }

    private void OnDrawGizmosSelected()
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

    private void Update()
    {
        // Обновляем список объектов в поле зрения
        GetVisibleObjects();
    }
}
