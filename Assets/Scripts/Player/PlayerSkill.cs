using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerSkill : MonoBehaviourPun
{
    [System.Serializable]
    public class CoolDown
    {
        public float Delay;
        public float LastAttackTime;
    }

    [Header("Kỹ Năng")]
    public CoolDown skill1Cooldown;
    public CoolDown skill2Cooldown;
    public CoolDown skill3Cooldown;
    public CoolDown skill4Cooldown;
    public PlayerController controller;
    private Dictionary<string, CoolDown> cooldowns;

    [Header("Đấm")]
    public Image cooldownSkill1;
    public TextMeshProUGUI skill1Text;


    [Header("Chưởng")]
    public GameObject kameRight;
    public GameObject kameLeft;
    public Transform fireKameRight;
    public Transform fireKameLeft;
    public Image cooldownSkill2;
    public TextMeshProUGUI skill2Text;
    public bool canMove;

    [Header("Biến Hình")]
    public int aimLayerIndex;
    public int baseLayerIndex;
    public Image cooldownSkill3;
    public TextMeshProUGUI skill3_Text;
    [Header("Kỹ năng Ultimate")]
    public Image cooldownSkill4;
    public TextMeshProUGUI skill4_Text;
    public GameObject ultiObject;
    public Transform utilTranform;
    private void Start()
    {
        InitializeCooldowns();
        canMove = true;
    }
    #region Skill Player
    #region Skill 1
    public void Skill1()
    {
        if (IsSkillReady("skill1"))
        {
            controller.Attack();
            StartCooldown("skill1", skill1Cooldown.Delay);
        }
    }
    #endregion
    #region Skill 2
    public void Skill2Object()
    {
        if (controller.faceRight)
        {
            GameObject bulletObj = Instantiate(kameRight, controller.attackPoint.transform.position, Quaternion.identity);
            SkillObject bulletScript = bulletObj.GetComponent<SkillObject>();
            bulletScript.Initialized(controller.id, controller.photonView.IsMine);
        }
        else
        {
            GameObject bulletObj = Instantiate(kameLeft, controller.attackPoint.position, Quaternion.identity);
            SkillObject bulletScript = bulletObj.GetComponent<SkillObject>();
            bulletScript.Initialized(controller.id, controller.photonView.IsMine);
        }
    }
    public void Skill2()
    {
        if (IsSkillReady("skill2"))
        {
            skill2Cooldown.LastAttackTime = Time.time;
            controller.aim.SetTrigger("Skill_2");
            StartCooldown("skill2", skill2Cooldown.Delay);
        }
        controller.isAutoAttacking = false;
    }
    public void StopMove()
    {
        canMove = false;
        controller.rb.gravityScale = 0;
    }
    public void StartMove()
    {
        canMove = true;
        controller.rb.gravityScale = 20;
    }
    #endregion
    #region Skill 3
    public void Skill3()
    {
        if (IsSkillReady("skill3"))
        {
            if (aimLayerIndex != -1)
            {
                StartCoroutine(PlayIntroAndIdle());
                ReturnToBaseLayerAfterDelay(30f);
                ChangeSkin();
            }
            StartCooldown("skill3", skill3Cooldown.Delay);
        }
        controller.isAutoAttacking = false;
    }
    #endregion
    #region Skill4
    public void Skill4Goku()
    {
        if (IsSkillReady("skill4"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
            GameObject bulletObj = PhotonNetwork.Instantiate(ultiObject.name, utilTranform.transform.position, Quaternion.identity);
            SkillObject bulletScript = bulletObj.GetComponent<SkillObject>();
            bulletScript.GetComponent<CircleCollider2D>().enabled = false;
            bulletScript.Initialized(controller.id, controller.photonView.IsMine);
            controller.aim.SetTrigger("Ultimate");
            controller.aim.Play(baseLayerIndex, 0, 0f);
            controller.aim.SetLayerWeight(aimLayerIndex, 0f);
            StartCooldown("skill4", skill4Cooldown.Delay);
        }
        controller.isAutoAttacking = false;
    }
    public void Skill4Vegeta()
    {
        if (IsSkillReady("skill4"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            controller.aim.SetTrigger("Ultimate");
            controller.aim.Play(baseLayerIndex, 0, 0f);
            controller.aim.SetLayerWeight(aimLayerIndex, 0f);
            StartCooldown("skill4", skill4Cooldown.Delay);
        }
        controller.isAutoAttacking = false;
    }
    public void Skill4ObjectVegeta()
    {
        GameObject bulletObj = Instantiate(ultiObject, utilTranform.transform.position, Quaternion.identity);
        SkillObject bulletScript = bulletObj.GetComponent<SkillObject>();

        bulletScript.GetComponent<CircleCollider2D>().enabled = true;
        bulletScript.Initialized(controller.id, controller.photonView.IsMine);
        CameraFollow.instance.ShakeCamera(5f, 1f);
        controller.currentHP = 0;
        controller.UpdateHealthSlider(controller.currentHP);
        controller.Die();
    }
    public void Skill4ObjectGoku()
    {
        GameObject bulletObj = Instantiate(ultiObject, utilTranform.transform.position, Quaternion.identity);
        SkillObject bulletScript = bulletObj.GetComponent<SkillObject>();

        bulletScript.GetComponent<CircleCollider2D>().enabled = true;
        bulletScript.speed = 20;
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            bulletScript.target = nearestEnemy.transform;
        }
        bulletScript.Initialized(controller.id, controller.photonView.IsMine);
        CameraFollow.instance.ShakeCamera(5f, 1f);
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = enemies[0];
            float nearestDistance = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            for (int i = 1; i < enemies.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, enemies[i].transform.position);
                if (distance < nearestDistance)
                {
                    nearestEnemy = enemies[i];
                    nearestDistance = distance;
                }
            }

            return nearestEnemy;
        }
        else
        {
            return null;
        }
    }
    public void Skill4Reset()
    {
        SkillObject bulletScript = ultiObject.GetComponent<SkillObject>();
        bulletScript.speed = 0;
    }
    #endregion
    #endregion
    private IEnumerator PlayIntroAndIdle()
    {
        bool isPlayingIntro = true;

        while (true)
        {
            if (isPlayingIntro)
            {
                photonView.RPC("AimIntroPun", RpcTarget.All);
                yield return new WaitForSeconds(3f);
                isPlayingIntro = false;
            }
            else
            {
                photonView.RPC("AimIdlePun", RpcTarget.All);
                yield return new WaitForSeconds(3f);
                break;
            }
        }
    }
    [PunRPC]
    void AimIdlePun()
    {
        controller.aim.Play("SSJ2_Idle");
        controller.aim.Play("SSJ3_Idle");
    }
    [PunRPC]
    void AimIntroPun()
    {
        controller.aim.Play("SSJ2_Intro");
        controller.aim.Play("SSJ3_Intro");
    }
    private void ReturnToBaseLayerAfterDelay(float delay)
    {
        float startTime = Time.time;
        float endTime = startTime+ delay;
        StartCoroutine(ReturnToBaseLayerCoroutine(endTime));
    }

    private System.Collections.IEnumerator ReturnToBaseLayerCoroutine(float endTime)
    {
        while (Time.time < endTime)
        {
            float remainingTime = endTime - Time.time;
            yield return null;
        }
        controller.aim.Play(baseLayerIndex, 0, 0f);
        controller.aim.SetLayerWeight(aimLayerIndex, 0f);
    }
    #region Cooldown Skill
    private void InitializeCooldowns()
    {
        cooldowns = new Dictionary<string, CoolDown>();
        cooldowns.Add("skill1", skill1Cooldown);
        cooldowns.Add("skill2", skill2Cooldown);
        cooldowns.Add("skill3", skill3Cooldown);
        cooldowns.Add("skill4", skill4Cooldown);
    }

    private bool IsSkillReady(string skillName)
    {
        if (cooldowns.ContainsKey(skillName))
        {
            CoolDown cooldown = cooldowns[skillName];
            float elapsedTime = Time.time - cooldown.LastAttackTime;
            return elapsedTime >= cooldown.Delay;
        }
        return false;
    }

    private void StartCooldown(string skillName, float delay)
    {
        if (cooldowns.ContainsKey(skillName))
        {
            CoolDown cooldown = cooldowns[skillName];
            cooldown.LastAttackTime = Time.time;
            StartCoroutine(StartCooldownCoroutine(skillName, delay));
        }
    }

    private IEnumerator StartCooldownCoroutine(string skillName, float delay)
    {
        Image cooldownImage = null;
        TextMeshProUGUI cooldownText = null;
        if (skillName == "skill1")
        {
            cooldownImage = cooldownSkill1;
            cooldownText = skill1Text;
        }else
        if (skillName == "skill2")
        {
            cooldownImage = cooldownSkill2;
            cooldownText = skill2Text;
        }
        else 
        if (skillName == "skill3")
        {
            cooldownImage = cooldownSkill3;
            cooldownText = skill3_Text;
        }
        else 
        if (skillName == "skill4")
        {
            cooldownImage = cooldownSkill4;
            cooldownText = skill4_Text;
        }

        if (cooldownImage != null && cooldownText != null)
        {
            cooldownImage.fillAmount = 1f;
            float startTime = Time.time;
            float endTime = startTime + delay;

            while (Time.time < endTime)
            {
                float remainingTime = endTime - Time.time;
                float fillAmount = remainingTime / delay;
                cooldownImage.fillAmount = fillAmount;
                cooldownText.text = remainingTime.ToString("F1");
                cooldownText.gameObject.SetActive(true);
                yield return null;
            }
            cooldownImage.fillAmount = 0f;
            cooldownText.text = "0.0";
            cooldownText.gameObject.SetActive(false);
        }
    }
    #endregion
    #region Thay đổi hình dạng biến hình và cộng chỉ số
    void ChangeSkin()
    {
        if (controller.playerLevel >= 1 && controller.playerLevel <= 5)
        {
            aimLayerIndex = 1;
            controller.aim.SetLayerWeight(aimLayerIndex, 1f);
        }
        else if (controller.playerLevel > 5 && controller.playerLevel <= 1000)
        {
            aimLayerIndex = 2;
            controller.aim.SetLayerWeight(aimLayerIndex, 2f);
        }
        else
        {
            // Xử lý cho trường hợp khác (nếu cần)
        }
    }
    #endregion
}
