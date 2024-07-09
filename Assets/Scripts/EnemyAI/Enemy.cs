using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using System.Linq;
using System.Reflection;
public class Enemy : MonoBehaviourPun
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public Animator aim;
    [Header("Quản lí")]
    public string enemyName;
    public string enemyLevel;
    public string objectcoinDead;
    private PlayerController[] playerInScene;
    private PlayerController targetPlayer;
    [Header("Tấn Công")]
    public int damage;
    public float chaseRange;
    public float attackRange;
    public float attackRate;
    private float lastattackTime;
    [Header("Di chuyển")]
    public float moveSpeed;
    [Header("Máu")]
    public int currentHP;
    public int maxHP;
    private float maxHealthValue;
    public Slider healthBar;
    [Header("Kinh nghiệm")]
    public int curAttackerID;
    private bool isMine;
    public int xpToGive;
    [Header("Patrol")]
    public int facingDirection = -1;
    public LayerMask whatIsGround;
    public float groundCheckDistance;
    public float wallCheckDistance;
    public Transform groundCheck;
    public Transform wallCheck;
    public bool wallDetected;
    public bool groundDetected;
    public float idleTime = 2;
    public float idleTimeCounter;
    public bool canMove = true;
    public bool stopMove = false;
    public bool aggresive;
    [Header("UI")]
    public GameObject damPopUp;
    public GameObject itemObject;
    public TextMeshProUGUI enemynametagText;
    public TextMeshProUGUI enemylevelText;
    public TextMeshProUGUI hpText;
    public GameObject InfoPopup;
    public bool attackstop = false;
    private void Start()
    {
        EnemyStatusInfo(maxHP);
        photonView.RPC("UpdateHpText", RpcTarget.All, currentHP);
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHP);
        enemynametagText.text = "" + enemyName;
        enemylevelText.text = "" + enemyLevel;
        if (groundCheck == null)
            groundCheck = transform;
        if (wallCheck == null)
            wallCheck = transform;
    }
    private void Update()
    {
        if (targetPlayer != null)
        {
            float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            if (photonView.IsMine)
            {
                if (dist < attackRange && Time.time - lastattackTime >= attackRate)
                {
                    if (targetPlayer.currentHP <= 0)
                    {
                        attackstop = true;
                        return;
                    }
                    else
                    {
                        Attack();
                    }
                }
            }
        }
        WalkAround();
        CollisionChecks();
        DetectPlayer();
    }
    #region Di chuyển và Tấn Công
    void Attack()
    {
        attackstop = false;
        aim.SetTrigger("Attack");
        lastattackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", RpcTarget.All, damage);
    }

    void DetectPlayer()
    {
        playerInScene = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in playerInScene)
        {
            float dist = Vector2.Distance(player.transform.position,transform.position);
            if (player == targetPlayer)
            {
                if (dist > chaseRange)
                {
                    targetPlayer = null;
                    aim.SetBool("Move", false);
                    rb.velocity = Vector2.zero;
                    canMove = true;
                }
            }
            else if (dist < chaseRange)
            {
                if (targetPlayer == null)
                {
                    targetPlayer = player;
                    canMove = false;
                }
            }
        }
    }
    #endregion
    #region Health + Name
    [PunRPC]
    public void TakeDamage(int attackerId, int damageAmount)
    {
        if (photonView.IsMine)
        {
            currentHP -= damageAmount;
            curAttackerID = attackerId;
            photonView.RPC("UpdateHpText", RpcTarget.All, currentHP);
            photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHP);
            if (damPopUp != null)
            {
                Vector3 popUpPosition = transform.position + new Vector3(0, 2, 0);
                GameObject instance = Instantiate(damPopUp, popUpPosition, Quaternion.identity);
                instance.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damageAmount.ToString("N0") + " Hit ";
                Animator animator = instance.GetComponentInChildren<Animator>();

                if (damageAmount <= 1000000)
                {
                    animator.Play("normal");
                }
                photonView.RPC("ShowDamPopUp", RpcTarget.Others, popUpPosition, damageAmount);
            }
            if (currentHP <= 0)
            {
                Die();
            }
            else
            {
                photonView.RPC("FlasDamage", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    void FlasDamage()
    {
        StartCoroutine(DamageFlash());
        IEnumerator DamageFlash()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.09f);
            sr.color = Color.white;
        }
    }
    [PunRPC]
    private void ShowDamPopUp(Vector3 position, int damageAmount)
    {
        if (damPopUp != null)
        {
            Vector3 popUpPosition = transform.position + new Vector3(0, 2, 0);
            GameObject instance = Instantiate(damPopUp, popUpPosition, Quaternion.identity);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damageAmount.ToString("N0") + " Hit ";
            Animator animator = instance.GetComponentInChildren<Animator>();

            if (damageAmount <= 1000000)
            {
                animator.Play("normal");
            }
        }
    }
    void Die()
    {
        PlayerController player = GetPlayer(curAttackerID);
        player.photonView.RPC("EarnExp", player.photonPlayer, xpToGive);
        player.KillKhiDen();
        PhotonNetwork.Instantiate(itemObject.name, transform.position, Quaternion.identity);
        photonView.RPC("RPCDie", RpcTarget.All);
    }
    [PunRPC]
    private void RPCDie()
    {
        Destroy(gameObject);
    }
    public PlayerController GetPlayer(int playerId)
    {
        return playerInScene.FirstOrDefault(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        return playerInScene.FirstOrDefault(x => x.gameObject == playerObject);
    }
    public void EnemyStatusInfo(int maxVal)
    {
        maxHealthValue = maxVal;
        healthBar.value = 1.0f;
    }
    [PunRPC]
    void UpdateHealthBar(int value)
    {
        healthBar.value = (float)value / maxHealthValue;
    }
    [PunRPC]
    void UpdateHpText(int curHP)
    {
        hpText.text = curHP.ToString("N0");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                InfoPopup.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                InfoPopup.SetActive(false);
            }
        }
    }
    #endregion
    #region Patrol
    public void WalkAround()
    {
        if (idleTimeCounter <= 0 && canMove)
        {
            rb.velocity = new Vector2(moveSpeed * facingDirection, rb.velocity.y);
            aim.SetBool("Move", true);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            aim.SetBool("Move", false);
        }

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
    }
    #endregion
    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));
        /*    if (groundCheck != null)
                Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
            Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));*/
    }
    #endregion
}