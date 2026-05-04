using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChargerEnemy : AIEnemyBase
{
    private NavMeshAgent agent;
    private bool hasHitPlayer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }
    }

    private void Update()
    {
        Act();
    }

    public override void Act()
    {
        if (agent == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHitPlayer(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other);
    }

    private void TryHitPlayer(Collider other)
    {
        if (hasHitPlayer || !other.CompareTag("Player")) return;

        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null) return;

        hasHitPlayer = true;
        playerController.playerHit();
        Invoke(nameof(ResetHit), 0.5f);
    }

    private void ResetHit()
    {
        hasHitPlayer = false;
    }
}
