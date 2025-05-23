using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Mirror; // Добавлено для Mirror

public abstract class NetworkEnemyBase : NetworkBehaviour, IDieable // Наследуемся от NetworkBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform player;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float attackCooldown = 1f;

    public List<Transform> patrolPoints;

    private int currentPatrolIndex = 0;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Ищем игрока только на сервере
        if (isServer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;

            if (patrolPoints.Count > 0)
                SetPatrolDestination();
        }
    }

    protected virtual void Update()
    {
        if (!isServer) return; // AI логика только на сервере

        if (patrolPoints.Count > 0 && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            SetPatrolDestination();
        }
    }

    private void SetPatrolDestination()
    {
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetAttack(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
    }

    public void StopMovement()
    {
        agent.isStopped = true;
    }

    public void Die()
    {
        GetComponent<NPCLookAtPlayer>().enabled = false;
        this.enabled = false;
    }
}
