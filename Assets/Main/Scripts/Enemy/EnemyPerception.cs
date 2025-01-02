using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10f; // Радиус обнаружения
    [SerializeField] private float fieldOfViewAngle = 360f; // Угол поля зрения
    [SerializeField] private LayerMask targetLayer; // Маска слоя для целей (например, игрок)

    private StateController stateController; // Ссылка на контроллер состояний
    private Transform player; // Ссылка на игрока
    private bool playerInSight = false; // Флаг, показывающий, что игрок в поле зрения

    private void Awake()
    {
        stateController = GetComponent<StateController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        // Проверяем, видит ли враг игрока
        CheckPlayerSight();

        // Если игрок в поле зрения, переключаемся на ChaseState
        if (playerInSight)
        {
            if (stateController.GetCurrentState() is not ChaseState)
            {
                stateController.SetState(new ChaseState()); // Переход в состояние преследования
            }
        }
        else
        {
            if (stateController.GetCurrentState() is not PatrolState)
            {
                stateController.SetState(new PatrolState()); // Переход в состояние патрулирования
            }
        }

        stateController.UpdateController(); // Обновляем контроллер состояний
    }

    // Проверка, видит ли враг игрока
    private void CheckPlayerSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Если игрок слишком далеко, не видим его
        if (distanceToPlayer > detectionRadius)
        {
            playerInSight = false;
            return;
        }

        // Проверяем, попадает ли игрок в угол поля зрения
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < fieldOfViewAngle / 2f)
        {
            // Проверяем, не блокирует ли что-то между врагом и игроком (лучевая проверка)
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRadius, targetLayer))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    playerInSight = true; // Игрок видим
                    return;
                }
            }
        }

        playerInSight = false; // Игрок не в поле зрения
    }
}
