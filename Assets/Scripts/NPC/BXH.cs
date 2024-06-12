using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BXH : MonoBehaviourPun
{
    [Header("LeaderBoard")]
    public float refreshRate = 0.5f;
    public GameObject[] slots;
    public TextMeshProUGUI[] namePlayerText;
    public TextMeshProUGUI[] levelPlayerText;
    public TextMeshProUGUI[] coinPlayerText;

    private void Update()
    {
        InvokeRepeating(nameof(Refresh), 0.5f, refreshRate);
    }

    private void Refresh()
    {
        Player[] players = PhotonNetwork.PlayerList;
        List<(Player, int, int)> sortedPlayers = players
      .Select(p => (p, p.CustomProperties.TryGetValue("Level", out object level) ? (int)level : 0, p.CustomProperties.TryGetValue("Coin", out object coin) ? (int)coin : 0))
      .OrderByDescending(t => t.Item2)
      .ToList();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < sortedPlayers.Count)
            {
                namePlayerText[i].text = "Tên Nhân Vật: " + sortedPlayers[i].Item1.NickName;
                levelPlayerText[i].text = "Cấp độ: " + sortedPlayers[i].Item2.ToString();
                coinPlayerText[i].text = "Vàng: " + sortedPlayers[i].Item3.ToString("N0");
            }
            else
            {
                namePlayerText[i].text = "";
                levelPlayerText[i].text = "";
                coinPlayerText[i].text = "";
            }
        }
    }
}