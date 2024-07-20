using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Photon.Pun;
public class HealthBarUI : MonoBehaviourPun
{
    [SerializeField]
    private Image _healthBar;
    [SerializeField]
    private TextMeshProUGUI _healthTxt;
    public void SetHealthBar(float value, float maxValue, int health)
    {
        _healthBar.fillAmount = value / maxValue;
        _healthTxt.text = health.ToString("N0");
    }
}