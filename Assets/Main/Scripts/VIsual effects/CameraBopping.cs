using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBopping : MonoBehaviour
{
    //private MainController inputActions;   // ��� ������ � input
    //private Vector3 movementInput;         // ��� ������������ ��������


    //void Start()
    //{
    //    animator = GetComponent<Animator>();
    //    // ������������� inputActions
    //    inputActions = InputManager.inputActions;
    //    inputActions.Player.Movement.performed += OnMovementPerformed;
    //    inputActions.Player.Movement.canceled += OnMovementCanceled;
    //}

    //// ���������� ������� ������� ������� �������� (����� ����� �������� ���������)
    //private void OnMovementPerformed(InputAction.CallbackContext context)
    //{
    //    movementInput = context.ReadValue<Vector3>();

    //    // ���� �������� ����������, �������� ������ ������
    //    if (movementInput != Vector3.zero)
    //    {
    //        Debug.Log("Button pressed");
    //        animator.Play("CameraShake");
    //    }
    //}

    //// ���������� ������� ������ �������� (����� ����� ��������� ���������)
    //private void OnMovementCanceled(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Buttom is released");
    //    movementInput = Vector3.zero;
    //    animator.Play("Stop");
    //}

    //// ������� �� ������� ��� ����������� �������
    //void OnDestroy()
    //{
    //    inputActions.Player.Movement.performed -= OnMovementPerformed;
    //    inputActions.Player.Movement.canceled -= OnMovementCanceled;
    //}

    public Transform parentObject; // ������ �� ������������ ������
    public Animator animator;      // ������ �� Animator ��� ������
    public float speedThreshold = 0.1f; // ����� �������� ��� ������� ��������

    private Vector3 lastPosition; // ��������� ������� �������
    private float currentSpeed;   // ������� �������� �������

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

        // ��������� ��������� �������
        lastPosition = parentObject.position;
    }

    void Update()
    {
        // ��������� ������� �������� �� ������ ��������� �������
        currentSpeed = (parentObject.position - lastPosition).magnitude / Time.deltaTime;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        // ���� �������� ���� ������ � "CameraShake" �� ������, ��� ��� �����������
        if (currentSpeed > speedThreshold)
        {
            if (animator.GetBool("IsMoving") == false)
            {
                animator.SetBool("IsMoving", true);
            }
        }
        // ���� �������� ���� ������ � �������� "Stop" ��� �� ��������, ������������� ������
        else if (currentSpeed <= speedThreshold )
        {
            animator.SetBool("IsMoving", false);
        }

        // ��������� ��������� �������
        lastPosition = parentObject.position;
    }
}
