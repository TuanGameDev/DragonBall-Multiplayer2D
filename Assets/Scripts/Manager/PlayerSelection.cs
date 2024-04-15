using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    [Header("PlayerSelection")]
    public string playerPrefabName;
    public GameObject[] playerModel;
    public int selectedCharacter=0;
    public Button[] characterIcons;
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
            selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
            playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
            playerPopup.SetActive(false);
        }
        else
        {
            selectedCharacter = 0;
            playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
        }
        foreach (GameObject player in playerModel)
        {
            player.SetActive(false);
        }
        playerModel[selectedCharacter].SetActive(true);
    }
    public void IconClick(int characterIndex)
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        foreach (GameObject player in playerModel)
        {
            player.SetActive(false);
        }
        selectedCharacter = characterIndex;
        playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
        playerModel[selectedCharacter].SetActive(true);
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