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
public class Enemy : MonoBehaviourPun
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public Animator aim;
    [Header("Quản lí")]
    public string enemyName;
    public string dead = "Death";
    private PlayerController[] playerInScene;
    private PlayerController targetPlayer;
    [Header("Tấn Công")]
    public int damage;
    public float chaseRange;
    public float attackRange;
    public float playerdetectRate;
    private float lastPlayerDetectTime;
    public float attackrate;
    private float lastattackTime;
    [Header("Di chuyển")]
    public float moveSpeed;
    [Header("Máu")]
    public int currentHP;
    public int maxHP;
    private float maxHealthValue;
    [Header("Kinh nghiệm")]
    public int curAttackerID;
    private bool isMine;
    public int xpToGive;
    [Header("ObjectToSpawnOnDeath")]
    public string objectTospawnOnDeath;
    [Header("Patrol")]
    public bool isAttacking = false;
    [Header("UI")]
    public GameObject damPopUp;
    public TextMeshProUGUI playernametagText;
    public TextMeshProUGUI hpText;
    public GameObject canvasHealh;
    private void Start()
    {
        UpdateHpText(currentHP);
        playernametagText.text = "" + enemyName;
        currentHP = maxHP;
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (!isAttacking)
        {
            photonView.RPC("Patrol", RpcTarget.All);
        }
        if (targetPlayer != null)
        {
            float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            float face = targetPlayer.transform.position.x - transform.position.x;

            if (face > 0)
            {
                photonView.RPC("FlipRight", RpcTarget.All);
            }
            else
            {
                photonView.RPC("FlipLeft", RpcTarget.All);
            }

            if (dist < attackRange && Time.time - lastattackTime >= attackrate)
            {
                Attack();
                photonView.RPC("ResetlocalScale", RpcTarget.All);
            }
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                //rb.velocity = dir.normalized * moveSpeed;
                photonView.RPC("FlipRight", RpcTarget.All);
                //aim.SetBool("Move", true);
            }
            else
            {
                rb.velocity = Vector2.zero;
                aim.SetBool("Move", false);
            }
        }
        DetectPlayer();
    }
    #region Di chuyển và Tấn Công
    [PunRPC]
    void FlipRight()
    {
        sr.flipX = false;
    }
    [PunRPC]
    void FlipLeft()
    {
        sr.flipX = true;
    }
    void Attack()
    {
        aim.SetTrigger("Attack");
        lastattackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", RpcTarget.All, damage);
    }

    void DetectPlayer()
    {
        if (Time.time - lastPlayerDetectTime > playerdetectRate)
        {
            lastPlayerDetectTime = Time.time;
            playerInScene = FindObjectsOfType<PlayerController>();
            foreach (PlayerController player in playerInScene)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                    {
                        isAttacking = false;
                        targetPlayer = null;
                        aim.SetBool("Move", false);
                        rb.velocity = Vector2.zero;
                    }
                }
                else if (dist < chaseRange)
                {
                    isAttacking = true;
                    if (targetPlayer == null)
                    {
                        targetPlayer = player;
                    }
                }
            }
        }
    }
    #endregion
    #region Health + Name
    [PunRPC]
    public void TakeDamage(int attackerId, int damageAmount)
    {
        currentHP -= damageAmount;
        curAttackerID = attackerId;
        UpdateHpText(currentHP);
        if (damPopUp != null)
        {
            Vector3 popUpPosition = transform.position + new Vector3(0, 2, 0);
            GameObject instance = Instantiate(damPopUp, popUpPosition, Quaternion.identity);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damageAmount.ToString("N0") + " Hit ";
            Animator animator = instance.GetComponentInChildren<Animator>();

            if (damageAmount <= 100000)
            {
                animator.Play("normal");
            }
        }
        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            FlasDamage();
        }
    }
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
    void Die()
    {
        if (PlayerController.me != null)
        {
            PlayerController player = PlayerController.me.GetPlayer(curAttackerID);
            if (player != null && player.photonView != null)
            {
                player.photonView.RPC("EarnExp", player.photonPlayer, xpToGive);
            }
        }
        if (photonView != null && (photonView.IsMine || PhotonNetwork.IsMasterClient))
        {
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Instantiate(dead, transform.position, Quaternion.identity);
            if (objectTospawnOnDeath != string.Empty)
                PhotonNetwork.Instantiate(objectTospawnOnDeath, transform.position, Quaternion.identity);
        }
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
                canvasHealh.SetActive(true);
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
                canvasHealh.SetActive(false);
            }
        }
        transform.localScale = new Vector2(-Mathf.Sign(rb.velocity.x), transform.localScale.y);
    }
    #endregion
    #region Patrol Enemy
    [PunRPC]
    void Patrol()
    {
        if (isAttacking)
        {
            return;
        }
        if(targetPlayer)
        {
            return;
        }
        if (IsFacingRight())
        {
            aim.SetBool("Move", true);
            rb.velocity = new Vector2(moveSpeed, 0f);
        }
        else
        {
            aim.SetBool("Move", true);
            rb.velocity = new Vector2(-moveSpeed, 0f);
        }
    }
    private bool IsFacingRight()
    {
        return transform.localScale.x > 0f;
    }
    [PunRPC]
    void ResetlocalScale()
    {
        transform.localScale = new Vector2(1f, transform.localScale.y);
    }
    #endregion
    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    #endregion
}