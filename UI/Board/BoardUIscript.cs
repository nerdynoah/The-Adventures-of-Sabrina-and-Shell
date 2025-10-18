using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using BaseCharacter.Items;

public class BoardUIscript : MonoBehaviour
{
    [SerializeField] GiveQuestinator[] quests;
    [SerializeField] MsgBox desc;
    [SerializeField] RawImage material;
    private Quest quest { get; set; }
    private bool IsShown { get; set; } = false;
    private bool HasSelected { get; set; } = false;
    /// <summary>
    /// Returns the quest being selected if one is being selected.
    /// </summary>
    /// <returns><see cref="Quest"/> or <see cref="null"/></returns>
    public Quest GetQuest()
    {
        if (quest != null)
        {
            HasSelected = true;
            return quest;
        }
        return null;
    }
    /// <summary>
    /// Reload the Quest
    /// </summary>
    /// <param name="level">player level</param>
    public void ReloadQuests(int level)
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].GetLevelActivation() <= level)
            {
                quests[i].SetIsVisible(true);
            }
            else
            {
                quests[i].SetIsVisible(false);
            }
        }
    }
    /// <summary>
    /// Show the quest board
    /// </summary>
    /// <param name="level">How many levels you've completed</param>
    /// <param name="quest">Are you on a quest right now?</param>
    public void ShowBoard(int level, QuestStage quest)
    {
        IsShown = true;
        ReloadQuests(level);
        material.color = new Color(1, 1, 1, 1);
        for (int i = 0; i < quests.Length; i++)
        {
            quests[i].ApplyQuestStage(quest);
        }
    }
    /// <summary>
    /// Update the stage of the quest on the board.
    /// </summary>
    /// <param name="questName">The name of the quest</param>
    /// <param name="stage">The current stage</param>
    public void UpdateQuests(string questName, QuestStage stage)
    {
        foreach (var quest in quests)
        {
            if (quest.name == questName)
            {
                quest.SetQuestStage(stage);
            }
        }
    }
    /// <summary>
    /// On Completion of a quest, after collecting rewards, this will set the paper to be clear and you will receive your rewards.
    /// </summary>
    /// <param name="questName">The name of the quest</param>
    public void SetQuestCompleted(string questName)
    {
        foreach (var quest in quests)
        {
            if (quest.name == questName)
            {
                quest.SetQuestStage(QuestStage.Rewarded);
                quest.ApplyQuestStage(QuestStage.Rewarded);
            }
        }
    }
    /// <summary>
    /// Hide Board
    /// </summary>
    public void HideBoard()
    {
        material.color = new Color(0,0,0,0);
        for (int i = 0; i < quests.Length; i++)
        {
            quests[i].HideMenu();
        }
        IsShown = false;
        desc.ClearMsg();
    }
    private void Start()
    {
        HideBoard();
    }
    private void Update()
    {
        if (IsShown)
        {
            if (HasSelected)
            {
                quest = null;
                HasSelected = false;
            }
            for (int i = 0; i < quests.Length; i++)
            {
                if (quests[i].isSelected)
                {
                    quest = quests[i].GetQuest();
                }
                if (quests[i].GetIsHover() && quests[i].GetIsClickable())
                {
                    desc.SetText(quests[i].GetisHoverDesc(),true);
                }
            }
        }
    }
}
