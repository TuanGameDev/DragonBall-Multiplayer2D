using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Quest
{
    public string Name { get; set; }
    public string Rewards { get; set; }
    public bool IsCompleted { get; set; }
    public int CurrentProgress { get; set; }
    public int MaxProgress { get; set; }
}

public class Mission : MonoBehaviour
{
    public List<Quest> Quests = new List<Quest>();
    public TextMeshProUGUI QuestListText;

    void Start()
    {
        CreateKillEnemiesQuest();
        UpdateQuestList();
    }

    public void CreateKillEnemiesQuest()
    {
        var quest = new Quest
        {
            Name = "Tiêu diệt 10 con khỉ đen",
            Rewards = "100 coin",
            IsCompleted = false,
            CurrentProgress = 0,
            MaxProgress = 10
        };
        AddQuest(quest);
    }

    public void UpdateEnemyProgress(int enemiesKilled)
    {
        if (Quests.Count > 0)
        {
            Quests[0].CurrentProgress += enemiesKilled;
            if (Quests[0].CurrentProgress >= Quests[0].MaxProgress)
            {
                Quests[0].IsCompleted = true;
            }
            UpdateQuestList();
        }
    }

    private void AddQuest(Quest quest)
    {
        Quests.Add(quest);
    }

    private void UpdateQuestList()
    {
        string questListText = "";
        foreach (var quest in Quests)
        {
            string status = quest.IsCompleted ? "Hoàn thành" : $"Chưa hoàn thành ({quest.CurrentProgress}/{quest.MaxProgress})";
            questListText += $"- {quest.Name} ({status})";
        }
        QuestListText.text = questListText;
    }
}