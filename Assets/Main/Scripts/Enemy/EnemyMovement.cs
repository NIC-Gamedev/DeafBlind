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

    [SerializeField] private GameObject target;
    private Vector3 targetPosition; // Текущая цель движения
    private bool hasTarget = false; // Флаг наличия цели

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
        targetPosition = target.GetComponent<Transform>().position;
        // Если есть цель, передать её NavMeshAgent
        if (hasTarget)
        {
            agent.SetDestination(targetPosition);

            // Получаем желаемое направление от NavMeshAgent
            Vector3 direction = agent.desiredVelocity;

            // Передаем направление в метод движения
            MoveUsingRigidbody(direction);
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
    public void SetTarget(GameObject targetObject)
    {
        target = targetObject;
        targetPosition = target.GetComponent<Transform>().position;
        hasTarget = true; // Устанавливаем флаг наличия цели
    }


}
