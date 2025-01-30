using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f; // ��������� �������� �������� �����
    [SerializeField] private float rotationSpeed = 10f; // ��������� �������� �������� �����

    private NavMeshAgent agent; // NavMeshAgent ��� ���������� ���������
    private Rigidbody rb;

    [SerializeField] private GameObject target;
    private Vector3 targetPosition; // ������� ���� ��������
    private bool hasTarget = false; // ���� ������� ����

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
       
        // ��������� ��������� NavMeshAgent
        agent.speed = movementSpeed;
        agent.angularSpeed = rotationSpeed * 10f; // NavMeshAgent ���������� �������/���
        agent.acceleration = movementSpeed * 2f;
        agent.updatePosition = false; // ��������� ��� ����� ������� Rigidbody
        agent.updateRotation = false; // ��������� ��������� �������
    }

    void Update()
    {
        targetPosition = target.GetComponent<Transform>().position;
        // ���� ���� ����, �������� � NavMeshAgent
        if (hasTarget)
        {
            agent.SetDestination(targetPosition);

            // �������� �������� ����������� �� NavMeshAgent
            Vector3 direction = agent.desiredVelocity;

            // �������� ����������� � ����� ��������
            MoveUsingRigidbody(direction);
        }
    }

    private void MoveUsingRigidbody(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // ���������� ���� ��� ��������
            rb.AddForce(direction.normalized * movementSpeed * 10f, ForceMode.Force);

            // ����������� ��������
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }

            // ������� ������� � ����������� ��������
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void FixedUpdate()
    {
        // �������������� ������� NavMeshAgent � Rigidbody
        if (agent.enabled)
        {
            agent.nextPosition = rb.position;
        }
    }

    // ��������� ����� ��� ��������� ���� �����
    public void SetTarget(GameObject targetObject)
    {
        target = targetObject;
        targetPosition = target.GetComponent<Transform>().position;
        hasTarget = true; // ������������� ���� ������� ����
    }


}
