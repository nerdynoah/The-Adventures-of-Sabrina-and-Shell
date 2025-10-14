using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BaseCharacter.Items;
using static Enums;
using static AllLibary;

public class GiveQuestinator : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string QuestName;
    [SerializeField] RawImage QuestMaterial;
    [SerializeField] Texture inActive;
    [SerializeField] Texture Active;
    [SerializeField] Texture Finished;
    [SerializeField] Texture Reward;
    [SerializeField] Texture Failed;
    private Quest adventure = new Quest(-1);
    public bool isHidden { get; private set; } = true;
    private bool isClickable = false;
    public bool isSelected { get; private set;} = false;
    public bool isRewarding { get; private set; } = false;
    public bool isHover { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        adventure = ItemLibary.SearchLibaryForQuest(QuestName);
        HideMenu();
    }
    /// <summary>
    /// Make the menu inviisble
    /// </summary>
    public void HideMenu()
    {
        isClickable = false;
        isHidden = true;
        QuestMaterial.color = new Color(0, 0, 0, 0);
    }
    /// <summary>
    /// Make the menu visble
    /// </summary>
    public void ShowMenu()
    {
        QuestMaterial.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// Set the quest to compelted
    /// </summary>
    public void SetCompleted()
    {
        adventure.SetQuestStage(QuestStage.Rewarded);
    }
    /// <summary>
    /// Get the Quest.
    /// </summary>
    /// <returns><see cref="Quest"/> or <see cref="null"/></returns>
    public Quest GetQuest()
    {
        isSelected = false;
        adventure.SetQuestStage(QuestStage.Active);
        if (adventure != null)
        {
            return adventure;
        }
        return null;
    }
    /// <summary>
    /// Is the quest visible
    /// </summary>
    /// <param name="isSee"></param>
    public void SetIsVisible(bool isSee)
    {
        adventure.SetIsVisible(isSee);
        if (isSee)
        {
            ShowMenu();
        }
    }
    /// <summary>
    /// Is the quest clickable
    /// </summary>
    /// <param name="isclickable"></param>
    public void SetIsClickable(bool isclickable)
    {
        adventure.SetIsClickable(isclickable);
        isClickable = isclickable;
    }
    /// <summary>
    /// Get when the quest activates its level.
    /// </summary>
    /// <returns>level</returns>
    public int GetLevelActivation()
    {
        return adventure.GetLevelActivation();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isClickable)
        {
            if (adventure.GetQuestStage() == QuestStage.Inactive)
            {
                isSelected = true;
                isClickable = false;
            }
            if (adventure.GetQuestStage() == QuestStage.Rewarded)
            {
                isRewarding = true;
                isSelected = false;
                isClickable = false;
            }
        }
    }
    /// <summary>
    /// Sets the quest stage.
    /// </summary>
    /// <param name="stage">see <see cref="QuestStage"/> for details</param>
    public void SetQuestStage(QuestStage stage)
    {
        adventure.SetQuestStage(stage);
    }
    /// <summary>
    /// Returns the Hover desc if the object is not hidden.
    /// </summary>
    /// <returns></returns>
    public string GetisHoverDesc()
    {
        if (isHover && !isHidden)
        {
            Debug.Log($"{adventure.GetDesc()}");
            return adventure.GetDesc();
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// Get's if the mouse is currently hovering
    /// </summary>
    /// <returns></returns>
    public bool GetIsHover()
    {
        return isHover;
    }
   
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }
    /// <summary>
    /// Apply the visual, clickable, and material via the <see cref="QuestStage"/>
    /// </summary>
    /// <param name="isOnQuest"></param>
    public void ApplyQuestStage(QuestStage isOnQuest)
    {
        QuestStage stage = adventure.GetQuestStage();
        if (stage == QuestStage.Unavailable)
        {
            isHidden = true;
            isClickable = false;
            QuestMaterial.color = new Color(0, 0, 0, 0);
        }
        if (stage == QuestStage.Completed || stage == QuestStage.Failed)
        {
            isHidden = false;
            isClickable = false;
            QuestMaterial.color = new Color(0.9f, 0.9f, 0.9f, 0.99f);
            if (stage == QuestStage.Completed)
            {
                QuestMaterial.texture = Reward;
            }
            else
            {
                QuestMaterial.texture = Failed;
            }
        }
        if (stage == QuestStage.Rewarded)
        {
            isHidden = false;
            isClickable = true;
            QuestMaterial.texture = Finished;
        }
        if (stage == QuestStage.Inactive)
        {
            isHidden = false;
            QuestMaterial.texture = inActive;
            isClickable = true;
            if (isOnQuest == QuestStage.Inactive)
            {
                isClickable = true;
            }
        }
        if (stage == QuestStage.Active)
        {
            isHidden = false;
            isClickable = false;
            QuestMaterial.color = new Color(0.8f, 0.8f, 1, 1);
            QuestMaterial.texture = Active;
        }
    }
    /// <summary>
    /// Gets if the Quest is clickable
    /// </summary>
    /// <returns><see cref="isClickable"/></returns>
    public bool GetIsClickable()
    {
        return isClickable;
    }
}