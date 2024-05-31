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
    public PlayerController controller;
    private Dictionary<string, CoolDown> cooldowns;

    [Header("Đấm")]
    public Image cooldownSkill1;
    public TextMeshProUGUI skill1Text;


    [Header("Chưởng")]
    public string kameRight;
    public string kameLeft;
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
    public GameObject skill3UI;
    public TextMeshProUGUI skill3UI_Text;
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
            GameObject bulletObj = PhotonNetwork.Instantiate(kameRight, controller.attackPoint.transform.position, Quaternion.identity);
            FireKame bulletScript = bulletObj.GetComponent<FireKame>();
            bulletScript.Initialized(controller.id, controller.photonView.IsMine);
        }
        else
        {
            GameObject bulletObj = PhotonNetwork.Instantiate(kameLeft, controller.attackPoint.position, Quaternion.identity);
            FireKame bulletScript = bulletObj.GetComponent<FireKame>();
            bulletScript.Initialized(controller.id, controller.photonView.IsMine);
        }
        canMove = false;
    }
    public void Skill2()
    {
        if (IsSkillReady("skill2"))
        {
            skill2Cooldown.LastAttackTime = Time.time;
            controller.aim.SetTrigger("Skill_2");
            StartCooldown("skill2", skill2Cooldown.Delay);
        }
    }
    public void StopMove()
    {
        canMove = true;
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
            skill3UI_Text.text = remainingTime.ToString("F1");
            skill3UI_Text.gameObject.SetActive(true);
            skill3UI.SetActive(true);
            yield return null;
        }
        controller.aim.Play(baseLayerIndex, 0, 0f);
        controller.aim.SetLayerWeight(aimLayerIndex, 0f);
        skill3UI_Text.text = "0.0";
        skill3UI_Text.gameObject.SetActive(false);
        skill3UI.SetActive(false);
    }
    #region Cooldown Skill
    private void InitializeCooldowns()
    {
        cooldowns = new Dictionary<string, CoolDown>();
        cooldowns.Add("skill1", skill1Cooldown);
        cooldowns.Add("skill2", skill2Cooldown);
        cooldowns.Add("skill3", skill3Cooldown);
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
        else if (skillName == "skill3")
        {
            cooldownImage = cooldownSkill3;
            cooldownText = skill3_Text;
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
        else if (controller.playerLevel > 5 && controller.playerLevel <= 100)
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
