using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class EnemyTest : MonoBehaviourPun
{
    public Animator anim;
    public Rigidbody2D rb;
    public int currenthp;
    public int maxhp;
    public int curAttackerID;
    public int facingDirection = -1;

    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public LayerMask whatIoIgnore;
    [SerializeField] public float groundCheckDistance;
    [SerializeField] public float wallCheckDistance;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform wallCheck;
    public bool wallDetected;
    public bool groundDetected;
    public RaycastHit2D playerDetection;
    [Header("Move info")]
    [SerializeField] public float speed;
    [SerializeField] public float idleTime = 2;
    public float idleTimeCounter;


    public bool canMove = true;
    public bool aggresive;

    public void Start()
    {
        currenthp = maxhp;
        if (groundCheck == null)
            groundCheck = transform;
        if (wallCheck == null)
            wallCheck = transform;

    }
    public void Update()
    {
        WalkAround();
        CollisionChecks();
    }
    [PunRPC]
     void TakeDamage(int attackerId, int damageAmount)
    {
        if (photonView.IsMine)
        {
            currenthp -= damageAmount;
            curAttackerID = attackerId;

            if (currenthp <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void WalkAround()
    {
        if (idleTimeCounter <= 0 && canMove)
            rb.velocity = new Vector2(speed * facingDirection, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, 0);

        idleTimeCounter -= Time.deltaTime;

        if (wallDetected || !groundDetected)
        {
            idleTimeCounter = idleTime;
            Flip();
        }
    }
    public void Flip()
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);

    }
    public void CollisionChecks()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        playerDetection = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, 100, ~whatIoIgnore);

    }
    public void OnDrawGizmos()
    {
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));

        if (wallCheck != null)
            Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + playerDetection.distance * facingDirection, wallCheck.position.y));


    }
}
