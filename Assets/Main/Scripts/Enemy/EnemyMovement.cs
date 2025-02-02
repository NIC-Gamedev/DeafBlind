using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
<<<<<<< HEAD
    [SerializeField] private float movementSpeed = 5f; // ��������� �������� �������� �����
    [SerializeField] private float rotationSpeed = 10f; // ��������� �������� �������� �����
=======
    [SerializeField] private float movementSpeed = 5f; 
    [SerializeField] public float movementSpeedMultiplier = 1;
    [SerializeField] private float rotationSpeed = 10f; // ��������� �������� �������� �����
>>>>>>> dev/enemyAi

    private NavMeshAgent agent; // NavMeshAgent ��� ���������� ���������
    private Rigidbody rb;

<<<<<<< HEAD
    [SerializeField] private GameObject target;
    private Vector3 targetPosition; // ������� ���� ��������
    private bool hasTarget = false; // ���� ������� ����

=======
    [SerializeField] public Transform target;
    public Vector3 targetPosition { private set; get; } // ������� ���� ��������
>>>>>>> dev/enemyAi
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
<<<<<<< HEAD
       
        // ��������� ��������� NavMeshAgent
=======
        // ��������� ��������� NavMeshAgent
>>>>>>> dev/enemyAi
        agent.speed = movementSpeed;
        agent.angularSpeed = rotationSpeed * 10f; // NavMeshAgent ���������� �������/���
        agent.acceleration = movementSpeed * 2f;
        agent.updatePosition = false; // ��������� ��� ����� ������� Rigidbody
        agent.updateRotation = false; // ��������� ��������� �������
    }

    void Update()
    {
<<<<<<< HEAD
        targetPosition = target.GetComponent<Transform>().position;
        // ���� ���� ����, �������� � NavMeshAgent
        if (hasTarget)
=======
        if (target)
>>>>>>> dev/enemyAi
        {
            targetPosition = target.transform.position;
            agent.SetDestination(targetPosition);

            // �������� �������� ����������� �� NavMeshAgent
            Vector3 direction = agent.desiredVelocity;

<<<<<<< HEAD
            // �������� ����������� � ����� ��������
            MoveUsingRigidbody(direction);
=======
            // �������� ����������� � ����� ��������
            MoveAndWatchToDirection(direction);
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
            MoveTo(directionAgent);
>>>>>>> dev/enemyAi
        }
    }

    public void KeepDistance(GameObject player,float distance)
    {
        if (Vector3.Distance(player.transform.position, transform.position) > distance)
        {
            MoveInDirection(player.transform.position - transform.position);
        }
        else
        {
            MoveInDirection((player.transform.position - transform.position) * -1);
        }
    }

    private void MoveAndWatchToDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
<<<<<<< HEAD
            // ���������� ���� ��� ��������
            rb.AddForce(direction.normalized * movementSpeed * 10f, ForceMode.Force);

            // ����������� ��������
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
=======
            rb.AddForce(direction.normalized * movementSpeed * 10f * movementSpeedMultiplier, ForceMode.Force);

            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
>>>>>>> dev/enemyAi
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }

<<<<<<< HEAD
            // ������� ������� � ����������� ��������
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
=======
            RotateToDirection(direction);
>>>>>>> dev/enemyAi
        }
    }
    private void MoveTo(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            rb.AddForce(direction.normalized * movementSpeed * 10f * movementSpeedMultiplier, ForceMode.Force);

            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public void RotateToObject(GameObject gameObject)
    {
        Vector3 direction = gameObject.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    public void RotateToDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void FixedUpdate()
    {
        // �������������� ������� NavMeshAgent � Rigidbody
        if (agent.enabled)
        {
            agent.nextPosition = rb.position;
        }
    }

<<<<<<< HEAD
    // ��������� ����� ��� ��������� ���� �����
    public void SetTarget(GameObject targetObject)
    {
        target = targetObject;
        targetPosition = target.GetComponent<Transform>().position;
        hasTarget = true; // ������������� ���� ������� ����
=======
    // ��������� ����� ��� ��������� ���� �����
    public void SetTarget(Transform targetObject)
    {
        target = targetObject;
>>>>>>> dev/enemyAi
    }
}
