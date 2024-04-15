using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class PlayerController : MonoBehaviourPun
{
    public Rigidbody2D rb;
    public Animator aim;
    public SpriteRenderer sr;
    public SpriteRenderer[] srEffects;
    public Player photonPlayer;
    public static PlayerController me;
    public int id;
    public List<PlayerController> listOfPlayers;
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
    [Header("Tự động Tấn Công")]
    public TextMeshProUGUI autoattackText;
    public Button autoattackButton;
    private Transform currentTarget;
    private bool isAutoAttacking = false;
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
    private bool isFalling = false;
    public bool dead;
    [Header("Vàng và Kim Cương")]
    public int coin;
    public int diamond;
    [Header("XP và Level")]
    public int playerLevel = 1;
    public int currentExp;
    public int maxExp = 500;
    [Header("UI")]
    public TextMeshProUGUI playernametagText;
    public TextMeshProUGUI playerLevelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI messageText;
    public Slider healthBar;
    public Canvas canvasHUD;
    [Header("Hiệu Ứng")]
    public GameObject damPopUp;
    public GameObject levelUp;
    [Header("Scipts")]
    public PlayerUpGrade _playerUpgrade;
    [PunRPC]
    public void InitializePlayer(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            playerLevel = PlayerPrefs.GetInt("PlayerLevel");
        }
        if (PlayerPrefs.HasKey("Coin"))
        {
            coin = PlayerPrefs.GetInt("Coin");
        }
        else
        {
            coin = 0;
        }
        if (PlayerPrefs.HasKey("Diamond"))
        {
            diamond = PlayerPrefs.GetInt("Diamond");
        }
        else
        {
            diamond = 0;
        }
        if (PlayerPrefs.HasKey("currentHP"))
        {
            currentHP = PlayerPrefs.GetInt("currentHP");
        }
        if (PlayerPrefs.HasKey("maxHP"))
        {
            maxHP = PlayerPrefs.GetInt("maxHP");
        }
        if (PlayerPrefs.HasKey("maxMP"))
        {
            maxMP = PlayerPrefs.GetInt("maxMP");
        }
        if (PlayerPrefs.HasKey("DF"))
        {
            def = PlayerPrefs.GetInt("DF");
        }
        if (PlayerPrefs.HasKey("CurrentExp"))
        {
            currentExp = PlayerPrefs.GetInt("CurrentExp");
        }
        if (PlayerPrefs.HasKey("MaxExp"))
        {
            maxExp = PlayerPrefs.GetInt("MaxExp");
        }
        if (PlayerPrefs.HasKey("DamageMin"))
        {
            damageMin = PlayerPrefs.GetInt("DamageMin");
        }
        if (PlayerPrefs.HasKey("DamageMax"))
        {
            damageMax = PlayerPrefs.GetInt("DamageMax");
        }
        UpdateNametag(player.NickName);
        UpdatePlayerLevel(playerLevel);
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
        UpdateValue(maxHP);
        currentHP = maxHP;
        UpdateExp(currentExp, maxExp, playerLevel);
        if (player.IsLocal)
            me = this;
    }
    void Start()
    {
        attackButton.onClick.AddListener(HandleAttackButtonClick);
        autoattackButton.onClick.AddListener(ToggleAutoAttack);
        if (!photonView.IsMine)
        {
            canvasHUD.enabled = false;
        }
    }
    void Update()
    {
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
        UpdateCoin(coin);
        UpdateDiamond(diamond);
        if (!photonView.IsMine)
            return;

        if (isAutoAttacking)
        {
            if (currentTarget != null)
            {
                float distance = Vector2.Distance(transform.position, currentTarget.position);

                if (distance > attackRange)
                {
                    MoveToTarget();
                }
                else if (Time.time - lastAttackTime > attackDelay)
                {
                    Attack();
                    lastAttackTime = Time.time;
                    StartCoroutine(StartCooldown());
                }
            }
            else
            {
                FindNearestEnemy();
            }
            autoattackText.color = Color.green;
        }
        else
        {
            MoveCharacter();
            autoattackText.color = Color.red;
        }
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
        if (!dead)
        {
            rb.velocity = new Vector2(x * moveSpeed, y * moveSpeed);

            if (x != 0 || y != 0)
            {
                aim.SetBool("Move", true);

                if (x > 0 && !faceRight)
                {
                    photonView.RPC("FlipRight", RpcTarget.All);
                }
                else if (x < 0 && faceRight)
                {
                    photonView.RPC("FlipLeft", RpcTarget.All);
                }
            }
            else
            {
                aim.SetBool("Move", false);
            }
        }
        if (rb.velocity.y <0)
        {
            aim.SetBool("IsFalling", true);
        }
        else
        {
            aim.SetBool("IsFalling", false);
        }
    }
    [PunRPC]
    void FlipRight()
    {
        for (int i = 0; i < srEffects.Length; i++)
        {
            srEffects[i].flipX = false;
        }
        sr.flipX = false;
        faceRight = true;
        attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
    }
    [PunRPC]
    void FlipLeft()
    {
        for (int i = 0; i < srEffects.Length; i++)
        {
            srEffects[i].flipX = true;
        }
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
            if (hitenemy != null && photonView.IsMine)
            {
                int randomDamage = Random.Range(damageMin, damageMax);
                enemy.GetComponent<Enemy>().photonView.RPC("TakeDamage", RpcTarget.All, warriorID, randomDamage);
            }
        aim.SetTrigger("Attack");
        aim.SetBool("Move", false);
        if(currentHP<=maxHP)
        {
            currentHP += 50;
        }
    }
    void initializeAttack(int attackId, bool inMine)
    {
        this.warriorID = attackId;
        this.isMine = inMine;
    }
    #endregion
    #region Tự động Attack
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAutoAttacking && other.CompareTag("Enemy") && currentTarget == null)
        {
            currentTarget = other.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == currentTarget)
        {
            currentTarget = null;
        }
    }
    private void MoveToTarget()
    {
        if (currentTarget != null)
        {
            Vector2 targetPosition = currentTarget.position - (transform.position - currentTarget.position).normalized * attackRange;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (targetPosition.x > transform.position.x)
            {
                if (!faceRight)
                {
                    photonView.RPC("FlipRight", RpcTarget.All);
                    aim.SetBool("Move", true);
                }
            }
            else if (targetPosition.x < transform.position.x)
            {
                if (faceRight)
                {
                    photonView.RPC("FlipLeft", RpcTarget.All);
                    aim.SetBool("Move", true);
                }
            }
        }
    }
    private void FindNearestEnemy()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        float nearestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (GameObject enemyObject in enemyObjects)
        {
            Transform enemyTransform = enemyObject.transform;
            float distance = Vector2.Distance(transform.position, enemyTransform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = enemyTransform;
            }
        }

        currentTarget = nearestTarget;

        if (currentTarget != null)
        {
            aim.SetBool("Move", true);
        }
        else
        {
            aim.SetBool("Move", false);
        }
    }
    private void ToggleAutoAttack()
    {
        isAutoAttacking = !isAutoAttacking;
        if(isAutoAttacking)
        {
            aim.SetBool("Move", true);
            rb.gravityScale = 0f;
            messageText.color = Color.yellow;
            messageText.text = " You have auto attack enabled ";
            StartCoroutine(HideMessageAfterDelay(3));
        }
        if (!isAutoAttacking)
        {
            currentTarget = null;
            rb.gravityScale = 20f;
            aim.ResetTrigger("Attack");
            messageText.color = Color.red;
            messageText.text = " You have turned off auto attack ";
            StartCoroutine(HideMessageAfterDelay(3));
        }
    }
    #endregion
    #region Level và XP
    [PunRPC]
    void UpdatePlayerLevel(int name)
    {
        playerLevelText.text ="Lv."+ "" + name;
    }
    [PunRPC]
    void EarnExp(int xpAmount)
    {
        currentExp += xpAmount;
        LevelUp();
        UpdateExp(currentExp, maxExp, playerLevel);
        photonView.RPC("UpdatePlayerLevel", RpcTarget.All, playerLevel);
        PlayerPrefs.SetInt("CurrentExp", currentExp);
        PlayerPrefs.SetInt("MaxExp", maxExp);
    }
    public void LevelUp()
    {
        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            maxExp = (int)(maxExp * 1.1f);
            playerLevel++;
            damageMin += 10;
            damageMax += 10;
            currentHP += 20;
            maxHP += 20;
            _playerUpgrade.strengthPotential +=1;
            PlayerPrefs.SetInt("Potential", _playerUpgrade.strengthPotential);
            PlayerPrefs.SetInt("CurrentExp", currentExp);
            PlayerPrefs.SetInt("MaxExp", maxExp);
            PlayerPrefs.SetInt("PlayerLevel", playerLevel);
            PlayerPrefs.SetInt("DamageMin", damageMin);
            PlayerPrefs.SetInt("DamageMax", damageMax);
            PlayerPrefs.SetInt("maxHP", maxHP);
            PlayerPrefs.SetInt("currentHP", currentHP);
            UpdateExp(currentExp, maxExp, playerLevel);
            photonView.RPC("UpdatePlayerLevel", RpcTarget.All, playerLevel);
            messageText.color = Color.green;
            messageText.text = " You have leveled up LV." + playerLevel;
            StartCoroutine(HideMessageAfterDelay(3f));
        }
    }
    public void UpdateExp(int currentExp, int maxExp, int level)
    {

        float percentage = (float)currentExp / maxExp * 100f;
        string formattedPercentage = " LV." + level + "+" + percentage.ToString("0.00")+"%";
        expText.text = formattedPercentage;
    }
    public PlayerController GetPlayer(int playerID)
    {
        foreach (PlayerController player in listOfPlayers)
        {
            if (player.id == playerID)
            {
                return player;
            }
        }
        return null;
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
        if (damPopUp != null)
        {
            Vector3 popUpPosition = transform.position + new Vector3(0, 2, 0);
            GameObject instance = Instantiate(damPopUp, popUpPosition, Quaternion.identity);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = "-" + damageValue.ToString("N0") + " Hit ";
            Animator animator = instance.GetComponentInChildren<Animator>();

            if (damageValue <= 100000)
            {
                animator.Play("critical");
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
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
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
        isAutoAttacking = false;
        transform.position = new Vector3(0, 90, 0);
        Vector3 spawnPos = GameManager.gamemanager.spawnPoint[Random.Range(0, GameManager.gamemanager.spawnPoint.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.gamemanager.respawnTime));
    }
    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        dead = false;
        transform.position = spawnPos;
        currentHP = maxHP;
        currentMP = maxMP;
        rb.gravityScale = 20f;
        UpdateHealthSlider(currentHP);
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
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
    public void UpdateValue(int maxVal)
    {
        maxHealthValue = maxVal;
        healthBar.value = 1.0f;
    }
    void UpdateHealthSlider(int heal)
    {
        healthBar.value = (float)heal / maxHealthValue;
    }
    #endregion
    #region Coin và Diamond
    [PunRPC]
    void GetCoin(int goldToGive)
    {
        coin += goldToGive;
        PlayerPrefs.SetInt("Coin", coin);
        messageText.text = " You have picked up the coin " + "+" + goldToGive.ToString("N0");
        messageText.color = Color.yellow;
        StartCoroutine(HideMessageAfterDelay(2f));
    }
    [PunRPC]
    void Diamond(int diamondToGive)
    {
        diamond += diamondToGive;
        PlayerPrefs.SetInt("Diamond", diamond);
        messageText.text = " You have picked up the diamond " + diamondToGive.ToString("N0");
        messageText.color = Color.yellow;
        StartCoroutine(HideMessageAfterDelay(2f));
    }
    public void UpdateCoin(int coin)
    {
        coinText.text = "" + coin.ToString("N0");
    }
    public void UpdateDiamond(int diamond)
    {
        diamondText.text = "" + diamond.ToString("N0");
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
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
    #endregion
}
