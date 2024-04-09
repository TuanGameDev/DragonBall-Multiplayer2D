using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviourPun
{
    public Rigidbody2D rb;
    public Animator aim;
    public SpriteRenderer sr;
    public Player photonPlayer;
    public static PlayerController me;
    public int id;
    [Header("Tấn Công")]
    public Transform attackPoint;
    public int damageMin;
    public int damageMax;
    public float attackRange;
    public float attackDelay;
    private float lastAttackTime;
    public int warriorID;
    private bool isMine;
    public LayerMask playermask;
    [Header("Tấn Công và SkillCooldown")]
    public Button attackButton;
    public Image cooldownAttack;
    public TextMeshProUGUI attackcooldownText;
    [Header("Máu và giáp")]
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    public int def;
    private float maxHealthValue;
    [Header("Di chuyển và Nhảy")]
    public Joystick joystick;
    public int moveSpeed;
    public bool faceRight = false;
    public bool dead;
    [Header("Vàng và Kim Cương")]
    public int coin;
    public int diamond;
    [Header("UI")]
    public TextMeshProUGUI playernametagText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public Slider healthSlider;
    public Canvas canvasHUD;
    [PunRPC]
    public void InitializePlayer(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        if (GameManager.gamemanager.spawnPositions != null && id <= GameManager.gamemanager.spawnPositions.Length)
        {
            transform.position = GameManager.gamemanager.spawnPositions[id - 1].position;
            transform.rotation = GameManager.gamemanager.spawnPositions[id - 1].rotation;
        }
        UpdateNametag(player.NickName);
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
        UpdateHealthSlider(maxHP);
    }
    void Start()
    {
        attackButton.onClick.AddListener(HandleAttackButtonClick);
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
        if (!photonView.IsMine)
        {
            canvasHUD.enabled = false;
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        if (!photonView.IsMine)
            return;
        MoveCharacter();
    }
    #region Di chuyển
    void MoveCharacter()
    {
        float x, y;
        x = joystick.Horizontal;
        y = joystick.Vertical;
        MoveJoystick(x, y);
    }
    void MoveJoystick(float x, float y)
    {
        rb.velocity = new Vector2(x, y) * moveSpeed;
        if (x != 0 || y != 0)
        {
            aim.SetBool("Move", true);
            if (x > 0)
            {
                photonView.RPC("FlipRight", RpcTarget.All);
            }
            else
            {
                photonView.RPC("FlipLeft", RpcTarget.All);

            }
        }
        else
        {
            aim.SetBool("Move", false);
        }
    }
    [PunRPC]
    void FlipRight()
    {
        sr.flipX = false;
        faceRight = true;
        attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
    }
    [PunRPC]
    void FlipLeft()
    {
        sr.flipX = true;
        faceRight = false;
        attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
    }
    #endregion
    #region Tấn Công và Skill
    public void HandleAttackButtonClick()
    {
        if (Time.time - lastAttackTime > attackDelay)
        {
            Attack();
            lastAttackTime = Time.time;
            StartCoroutine(StartCooldown());
        }
    }
    void Attack()
    {
        lastAttackTime = Time.time;
        Collider2D[] hitenemy = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playermask);
        initializeAttack(id, photonView.IsMine);
        foreach (Collider2D enemy in hitenemy)
        {
            int randomDamage = Random.Range(damageMin, damageMax);
            enemy.GetComponent<Enemy>().photonView.RPC("TakeDamage", RpcTarget.All, warriorID, randomDamage);
        }
        aim.SetTrigger("Attack");
    }
    void initializeAttack(int attackId, bool inMine)
    {
        this.warriorID = attackId;
        this.isMine = inMine;
    }
    #endregion
    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
    #region Health + Name
    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        int damageValue = damageAmount - def;
        if (damageValue < 1)
        {
            damageValue = 1;
        }
        currentHP -= damageValue;
        UpdateHealthSlider(currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            photonView.RPC("FlasDamage", RpcTarget.All);
        }
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
    }
    [PunRPC]
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
       
    }
    [PunRPC]
    public void UpdateNametag(string name)
    {
        playernametagText.text = ""+name;
    }
    void UpdateHpText(int curHP, int maxHP, int curMP, int maxMP)
    {
        hpText.text = curHP + "/" + maxHP;
        mpText.text = curMP + "/" + maxMP;

    }
    void UpdateHealthSlider(int heal)
    {
        healthSlider.value = (float)heal / maxHealthValue;
        healthSlider.value = 1.0f;
    }
    #endregion
    #region IEnumerator Attack và Skill
    IEnumerator StartCooldown()
    {
        cooldownAttack.fillAmount = 1f;
        float startTime = Time.time;
        float endTime = startTime + attackDelay;

        while (Time.time < endTime)
        {
            float remainingTime = endTime - Time.time;
            float fillAmount = remainingTime / attackDelay;
            cooldownAttack.fillAmount = fillAmount;
            attackcooldownText.text = remainingTime.ToString("F1");
            attackcooldownText.gameObject.SetActive(true);
            yield return null;
        }
        cooldownAttack.fillAmount = 0f;
        attackcooldownText.text = "0.0";
        attackcooldownText.gameObject.SetActive(false);
    }
    #endregion
}
