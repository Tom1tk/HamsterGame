using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [Header("Player reference")]
    [SerializeField]
    Transform player;
    public LayerMask playerLayer;

    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Animator spriteAnim;
    [SerializeField]
    ParticleSystem ChargeFX;
    [SerializeField]
    NavMeshAgent enemyAgent;
    [SerializeField]
    Collider wanderArea;

    public float maxVelocity = 15f;
    public float inputForce = 100f;
    public float detectionRange;
    public float boostAttackRange;
    public float boostChargeTime;
    public float boostCD = 3f;
    public float boostForce = 100f;
    public bool canSeePlayer, canAttackPlayer;
    public bool Attacked, boostStarted, wanderPointCreated;
    public float wanderTime;
    public Vector3 wanderPoint;
    public float enemyMagnitudeBeforePhysicsUpdate;

    void Awake()
    {
        ChargeFX.Stop();
        rb = GetComponent<Rigidbody>();
        spriteAnim = GetComponentInChildren<Animator>();
        player = GameObject.Find("PlayerBall").GetComponent<Transform>();

        Attacked = false;
        boostStarted = false;
        wanderPointCreated = false;

        enemyAgent.updatePosition = false;
        enemyAgent.updateRotation = false;
        enemyAgent.updateUpAxis = false;
    }
    void FixedUpdate() 
    {
        enemyMagnitudeBeforePhysicsUpdate = rb.velocity.magnitude;

        //enemyAgent.SetDestination(player.position);

        //since the NavMeshAgent doesnt update its own position, we have to do it manually as the first corner [0]
        enemyAgent.nextPosition = rb.position;
        //to avoid desync where the agent and enemy would be in different places, causing the enemy to fly in some cases
        enemyAgent.Warp(rb.position);

        //spriteAnim.SetFloat("speed", rb.velocity.magnitude);

        canSeePlayer = Physics.CheckSphere(transform.position, detectionRange, playerLayer);
        canAttackPlayer = Physics.CheckSphere(transform.position, boostAttackRange, playerLayer);

        if (!canSeePlayer && !canAttackPlayer && !boostStarted) 
        {
            if(wanderPointCreated == false)
            {
                StartCoroutine(createWanderPoint());
            }
            Wander();
        }
        if (canSeePlayer && !canAttackPlayer && !boostStarted) 
        {
            ChasePlayer();
        }
        
        if (canSeePlayer && canAttackPlayer && !boostStarted) 
        {
            boostStarted = true;
            StartCoroutine(BoostAtPlayer());
            player.GetComponent<HealthScript>().combatStarted();
            this.gameObject.GetComponent<HealthScript>().combatStarted();
        }
        
        DrawPath();
    }
    IEnumerator createWanderPoint()
    {
        wanderPointCreated = true;
        wanderPoint = RandomPointInBounds(wanderArea.bounds);
        wanderTime = Random.Range(1f, 5f);
        yield return new WaitForSeconds(wanderTime);
        wanderPointCreated = false;
    }

    void Wander()
    {
        NavMeshPath wanderPath = new NavMeshPath();
        enemyAgent.CalculatePath(wanderPoint, wanderPath);
        enemyAgent.SetPath(wanderPath);

        //finds direction from agent to destination as vector3
        Vector3 wanderDirection = (wanderPath.corners[1] - this.transform.position).normalized;
        //adds force in that direction
        rb.AddForce(wanderDirection * inputForce/2f);

        player.GetComponent<HealthScript>().combatEnded();
        this.gameObject.GetComponent<HealthScript>().combatEnded();
    }

    void ChasePlayer()
    {
        NavMeshPath pathToPlayer = new NavMeshPath();
        enemyAgent.CalculatePath(player.position, pathToPlayer);
        enemyAgent.SetPath(pathToPlayer);
        
        //since the first corner will always be the enemy location, we use the second corner [1] to follow the player
        Vector3 ChaseDirection = (pathToPlayer.corners[1] - this.transform.position).normalized;
        //adds force in that direction
        rb.AddForce(ChaseDirection * inputForce);

        player.GetComponent<HealthScript>().combatStarted();
        this.gameObject.GetComponent<HealthScript>().combatStarted();
    }

    IEnumerator BoostAtPlayer()
    {
        if (!Attacked)
        {
            Attacked = true;
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

            ChargeFX.Play();

            yield return new WaitForSeconds(boostChargeTime);

            ChargeFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.transform.LookAt(player);
            rb.AddForce(transform.forward * (boostForce), ForceMode.Impulse);
            
            Invoke(nameof(ResetAttack), boostCD);

        }
    }

    private void ResetAttack()
    {
        Attacked = false;
        boostStarted = false;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds) 
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
            );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, boostAttackRange);
    }

    void DrawPath()
    {
        var nav = GetComponent<NavMeshAgent>();
        if(nav == null || nav.path == null)
            return;
    
        var line = this.GetComponent<LineRenderer>();
        if(line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default")) {color = Color.yellow};
            line.startWidth =  0.25f;
            line.startColor = Color.red;
        }
    
        var path = nav.path;
    
        line.positionCount = path.corners.Length;
    
        for( int i = 0; i < path.corners.Length; i++ )
        {
            line.SetPosition( i, path.corners[ i ] );
        }

        /*
        Draws a yellow line from the center of the actor to the clicked location with the code in the Update() below
        */
    }

}
