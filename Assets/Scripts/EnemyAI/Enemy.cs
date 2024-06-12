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
    public bool isAttacking = false;
    [Header("UI")]
    public GameObject damPopUp;
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
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (PhotonNetwork.InRoom && !isAttacking)
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
                photonView.RPC("ResetlocalScale", RpcTarget.All);
            }
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                photonView.RPC("FlipRight", RpcTarget.All);
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
    #endregion
    #region Health + Name
    [PunRPC]
    public void TakeDamage(int attackerId, int damageAmount)
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
        PhotonNetwork.Destroy(gameObject);
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