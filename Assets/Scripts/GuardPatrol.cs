
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrol : MonoBehaviour
{
    public enum PathStrategy
    {
        Shortest,
        AvoidEnemies,
        Stealth
    }

    public PathStrategy strategy = PathStrategy.Shortest;

    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private int currentPoint = 0;
    private bool isChasing = false;
    private Coroutine chaseCoroutine;

    private int walkableArea;
    private int dangerArea;
    private int coverArea;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        walkableArea = NavMesh.GetAreaFromName("Walkable");
        dangerArea = NavMesh.GetAreaFromName("EnemyDanger");
        coverArea = NavMesh.GetAreaFromName("Cover");

        ApplyStrategy();

        if (patrolPoints.Length > 0)
            GoToNextPoint();
    }

    void Update()
    {
        if (isChasing || patrolPoints.Length == 0)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPoint();
        }
    }

    void ApplyStrategy()
    {
        agent.SetAreaCost(walkableArea, 1f);
        agent.SetAreaCost(dangerArea, 1f);
        agent.SetAreaCost(coverArea, 1f);

        if (strategy == PathStrategy.AvoidEnemies)
        {
            agent.SetAreaCost(dangerArea, 50f);
        }
        else if (strategy == PathStrategy.Stealth)
        {
            agent.SetAreaCost(walkableArea, 5f);
            agent.SetAreaCost(dangerArea, 50f);
            agent.SetAreaCost(coverArea, 1f);
        }
    }

    void GoToNextPoint()
    {
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    public void StartChase(Transform target)
    {
        if (chaseCoroutine != null)
            StopCoroutine(chaseCoroutine);

        chaseCoroutine = StartCoroutine(ChasePlayer(target));
    }

    IEnumerator ChasePlayer(Transform target)
    {
        isChasing = true;
        agent.SetDestination(target.position);

        float timer = 0f;

        while (timer < 3f)
        {
            if (target != null)
                agent.SetDestination(target.position);

            timer += Time.deltaTime;
            yield return null;
        }

        isChasing = false;
        GoToNextPoint();
        chaseCoroutine = null;
    }
}
