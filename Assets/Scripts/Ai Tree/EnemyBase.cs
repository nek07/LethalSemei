using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // Для использования List

public abstract class EnemyBase : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform player;
    
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 2f;

    public List<Transform> patrolPoints; // Список точек патруля, куда NPC будет идти

    private int currentPatrolIndex = 0; // Индекс текущей точки патруля

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Если есть точки патруля, установим первую точку
        if (patrolPoints.Count > 0)
        {
            SetPatrolDestination();
        }
    }

    protected virtual void Update()
    {
        // Если NPC на патруле, проверим, дошел ли он до точки
        if (patrolPoints.Count > 0 && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Переход к следующей точке патруля
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            SetPatrolDestination();
        }
    }

    private void SetPatrolDestination()
    {
        // Устанавливаем точку назначения для NavMeshAgent
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
}