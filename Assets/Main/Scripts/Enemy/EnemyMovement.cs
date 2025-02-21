using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] public float movementSpeedMultiplier = 1;
    [SerializeField] private float rotationSpeed = 10f;

    private NavMeshAgent agent;
    public Rigidbody rb { get; private set; }

    [SerializeField] public Transform target;
    public Vector3 targetPosition { private set; get; }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.speed = movementSpeed;
        agent.angularSpeed = rotationSpeed * 10f;
        agent.acceleration = movementSpeed * 2f;
        agent.updatePosition = false; 
        agent.updateRotation = false; 
    }

    void Update()
    {
        if (target)
        {
            targetPosition = target.transform.position;
            agent.SetDestination(targetPosition);
            
            Vector3 direction = agent.desiredVelocity;
            MoveAndWatchToDirection(direction);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
    public void MoveInDirection(Vector3 direction)
    {
        if (agent.enabled)
        {
            agent.SetDestination(transform.position + direction.normalized);
            Vector3 directionAgent = agent.desiredVelocity;
            MoveTo(directionAgent);
        }
    }

    public void KeepDistance(GameObject player, float distance)
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
            direction -= new Vector3(0,direction.y,0);
            rb.AddForce(direction.normalized * movementSpeed * 10f * movementSpeedMultiplier, ForceMode.Force);

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }

            RotateToDirection(direction);
        }
    }
    private void MoveTo(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            rb.AddForce(direction.normalized * movementSpeed * 10f * movementSpeedMultiplier, ForceMode.Force);

            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
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
        // Ñèíõðîíèçèðóåì ïîçèöèþ NavMeshAgent ñ Rigidbody
        if (agent.enabled)
        {
            agent.nextPosition = rb.position;
        }
    }

    // Ïóáëè÷íûé ìåòîä äëÿ óñòàíîâêè öåëè èçâíå
    public void SetTarget(Transform targetObject)
    {
        target = targetObject;
    }
}
