using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBopping : MonoBehaviour
{
    //private MainController inputActions;   // Для работы с input
    //private Vector3 movementInput;         // Для отслеживания движения


    //void Start()
    //{
    //    animator = GetComponent<Animator>();
    //    // Инициализация inputActions
    //    inputActions = InputManager.inputActions;
    //    inputActions.Player.Movement.performed += OnMovementPerformed;
    //    inputActions.Player.Movement.canceled += OnMovementCanceled;
    //}

    //// Обработчик события нажатия клавиши движения (когда игрок начинает двигаться)
    //private void OnMovementPerformed(InputAction.CallbackContext context)
    //{
    //    movementInput = context.ReadValue<Vector3>();

    //    // Если движение происходит, начинаем тряску камеры
    //    if (movementInput != Vector3.zero)
    //    {
    //        Debug.Log("Button pressed");
    //        animator.Play("CameraShake");
    //    }
    //}

    //// Обработчик события отмены движения (когда игрок перестает двигаться)
    //private void OnMovementCanceled(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Buttom is released");
    //    movementInput = Vector3.zero;
    //    animator.Play("Stop");
    //}

    //// Отписка от событий при уничтожении объекта
    //void OnDestroy()
    //{
    //    inputActions.Player.Movement.performed -= OnMovementPerformed;
    //    inputActions.Player.Movement.canceled -= OnMovementCanceled;
    //}

    public Transform parentObject; // Ссылка на родительский объект
    public Animator animator;      // Ссылка на Animator для камеры
    public float speedThreshold = 0.1f; // Порог скорости для запуска анимации

    private Vector3 lastPosition; // Последняя позиция объекта
    private float currentSpeed;   // Текущая скорость объекта

    void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object not assigned!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not assigned!");
        }

        // Сохраняем начальную позицию
        lastPosition = parentObject.position;
    }

    void Update()
    {
        // Вычисляем текущую скорость на основе изменения позиции
        currentSpeed = (parentObject.position - lastPosition).magnitude / Time.deltaTime;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        // Если скорость выше порога и "CameraShake" не играет, или она завершилась
        if (currentSpeed > speedThreshold)
        {
            if (animator.GetBool("IsMoving") == false)
            {
                animator.SetBool("IsMoving", true);
            }
        }
        // Если скорость ниже порога и анимация "Stop" еще не запущена, останавливаем тряску
        else if (currentSpeed <= speedThreshold )
        {
            animator.SetBool("IsMoving", false);
        }

        // Обновляем последнюю позицию
        lastPosition = parentObject.position;
    }
}
