using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    public string playerPrefabName;
    public GameObject[] playerModel;
    public int currentPlayerIndex = 0;
    [Header("Popup")]
    public GameObject playerPopup;
    public static PlayerSelection playerselection;

    private void Awake()
    {
        playerselection = this;
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("SelectedCharacter"))
        {
            currentPlayerIndex = PlayerPrefs.GetInt("SelectedCharacter");
            playerPrefabName = playerModel[currentPlayerIndex].GetComponent<PlayerModelName>().playerName;
            playerPopup.SetActive(false);
        }
        else
        {
            currentPlayerIndex = 0;
            playerPrefabName = playerModel[currentPlayerIndex].GetComponent<PlayerModelName>().playerName;
        }

        foreach (GameObject player in playerModel)
        {
            player.SetActive(false);
        }
        playerModel[currentPlayerIndex].SetActive(true);
    }

    public void ChangeNext()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= playerModel.Length)
            currentPlayerIndex = 0;
        UpdateSelectedCharacter();
    }

    public void ChangeBack()
    {
        currentPlayerIndex--;
        if (currentPlayerIndex < 0)
            currentPlayerIndex = playerModel.Length - 1;
        UpdateSelectedCharacter();
    }

    private void UpdateSelectedCharacter()
    {
        PlayerPrefs.SetInt("SelectedCharacter", currentPlayerIndex);
        foreach (GameObject player in playerModel)
        {
            player.SetActive(false);
        }
        playerModel[currentPlayerIndex].SetActive(true);
        playerPrefabName = playerModel[currentPlayerIndex].GetComponent<PlayerModelName>().playerName;
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("SelectedCharacter");
        PlayerPrefs.DeleteKey("Coin");
        PlayerPrefs.DeleteKey("Diamond");
        PlayerPrefs.DeleteKey("PlayerLevel");
        PlayerPrefs.DeleteKey("DamageMin");
        PlayerPrefs.DeleteKey("DamageMax");
        PlayerPrefs.DeleteKey("CurrentExp");
        PlayerPrefs.DeleteKey("MaxExp");
        PlayerPrefs.DeleteKey("currentHP");
        PlayerPrefs.DeleteKey("maxHP");
        PlayerPrefs.DeleteKey("DF");
        PlayerPrefs.DeleteKey("Name");
        PlayerPrefs.DeleteKey("RememberMe");
    }
}