using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f; // Настройка скорости движения врага
    [SerializeField] private float rotationSpeed = 10f; // Настройка скорости поворота врага

    private NavMeshAgent agent; // NavMeshAgent для управления движением
    private Rigidbody rb;

    [SerializeField] private Transform target;
    public Vector3 targetPosition { private set; get; } // Текущая цель движения

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        // Настроить параметры NavMeshAgent
        agent.speed = movementSpeed;
        agent.angularSpeed = rotationSpeed * 10f; // NavMeshAgent использует градусы/сек
        agent.acceleration = movementSpeed * 2f;
        agent.updatePosition = false; // Позволяет нам самим двигать Rigidbody
        agent.updateRotation = false; // Управляем поворотом вручную
    }

    void Update()
    {
        if (target)
        {
            targetPosition = target.transform.position;
            agent.SetDestination(targetPosition);

            // Получаем желаемое направление от NavMeshAgent
            Vector3 direction = agent.desiredVelocity;

            // Передаем направление в метод движения
            MoveUsingRigidbody(direction);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    public void MoveInDirection(Vector3 direction)
    {
        if (agent.enabled)
        {
            agent.SetDestination(transform.position + direction.normalized);
            Vector3 directionAgent = agent.desiredVelocity;
            MoveUsingRigidbody(directionAgent);
        }
    }

    private void MoveUsingRigidbody(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // Добавление силы для движения
            rb.AddForce(direction.normalized * movementSpeed * 10f, ForceMode.Force);

            // Ограничение скорости
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

            // Плавный поворот в направлении движения
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void FixedUpdate()
    {
        // Синхронизируем позицию NavMeshAgent с Rigidbody
        if (agent.enabled)
        {
            agent.nextPosition = rb.position;
        }
    }

    // Публичный метод для установки цели извне
    public void SetTarget(Transform targetObject)
    {
        target = targetObject;
    }
}
