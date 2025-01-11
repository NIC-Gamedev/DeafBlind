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

    [SerializeField] private Transform target;
    public Vector3 targetPosition { private set; get; } // ������� ���� ��������

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
        if (target)
        {
            targetPosition = target.transform.position;
            agent.SetDestination(targetPosition);

            // �������� �������� ����������� �� NavMeshAgent
            Vector3 direction = agent.desiredVelocity;

            // �������� ����������� � ����� ��������
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
            // ���������� ���� ��� ��������
            rb.AddForce(direction.normalized * movementSpeed * 10f, ForceMode.Force);

            // ����������� ��������
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
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
    public void SetTarget(Transform targetObject)
    {
        target = targetObject;
    }
}
