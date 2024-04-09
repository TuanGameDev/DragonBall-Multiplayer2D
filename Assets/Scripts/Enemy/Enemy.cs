using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class Enemy : MonoBehaviourPun
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public Animator aim;
    [Header("Quản lí")]
    public string enemyName;
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
    public int xpToGive;
    [Header("UI")]
    public TextMeshProUGUI playernametagText;
    public TextMeshProUGUI hpText;
    public Slider healthBar;
    private void Start()
    {
        UpdateValue(maxHP);
        UpdateNametag(enemyName);
        UpdateHealthBar(currentHP);
        UpdateHpText(currentHP);
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
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
            }
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                rb.velocity = dir.normalized * moveSpeed;
                aim.SetBool("Move", true);
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
                        targetPlayer = null;
                        aim.SetBool("Move", false);
                        rb.velocity = Vector2.zero;
                    }
                }
                else if (dist < chaseRange)
                {
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
        UpdateHealthBar(currentHP);
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
            yield return new WaitForSeconds(0.05f);
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
        }
    }

    public void UpdateValue(int maxVal)
    {
        maxHealthValue = maxVal;
        healthBar.value = 1.0f;
    }
    [PunRPC]
    void UpdateNametag(string nametag)
    {
        playernametagText.text = "" + nametag;
    }
    [PunRPC]
    void UpdateHpText(int curHP)
    {
        hpText.text = curHP.ToString();
    }
    [PunRPC]
    void UpdateHealthBar(int value)
    {
        healthBar.value = (float)value / maxHealthValue;
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