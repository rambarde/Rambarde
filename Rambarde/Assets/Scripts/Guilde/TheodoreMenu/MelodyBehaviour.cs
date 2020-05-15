﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MelodyBehaviour: 
    MonoBehaviour,
    IPointerEnterHandler, 
    IPointerExitHandler,
    IPointerClickHandler
{
    public Melodies.Melody melody;
    public int maxSelected;
    [SerializeField]
    private bool _isClickable;
    public bool IsClickable { get { return _isClickable; } set { _isClickable = value; } }

    RectTransform canvasRectTransform;
    RectTransform tooltipRectTransform;
    Tooltip tooltip;
    GameObject[] slottedSkills;
    GameObject slot;
    GameObject warning;
    //GameObject counter;
    //Button resetTier;
    int currentSelected;
    
    void Awake()
    {
        warning = transform.parent.GetComponentInParent<TheodoreMenuManager>().warning.gameObject;
        if (IsClickable)
        {
            GetComponent<Image>().sprite = melody.sprite;
            currentSelected = 0;
        }
    }

    void Start()
    {
        if(GameObject.FindWithTag("Tooltip")!=null)
        {
            tooltip = GameObject.FindWithTag("Tooltip").GetComponent<Tooltip>();
            tooltipRectTransform = GameObject.FindWithTag("Tooltip").GetComponent<RectTransform>() as RectTransform;
            canvasRectTransform = tooltipRectTransform.parent.GetComponent<RectTransform>() as RectTransform;
        }

        GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot");
        slottedSkills = new GameObject[4];
        int j = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            GameObject slot = slots[i];
            if (slot.GetComponent<SlotBehaviour>() != null && slot.GetComponent<SlotBehaviour>().InnateSkillSlot)
            {
                slottedSkills[j] = slot;
                j += 1;
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (tooltip != null && melody != null)
        {
            tooltip.setObject(gameObject);
            tooltip.Activate(true);

            if (canvasRectTransform == null)
                return;

            Vector3[] worldCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(worldCorners);

            Vector2 localRectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform,
                                                                    new Vector2((worldCorners[3].x + worldCorners[0].x) / 2.0f, worldCorners[0].y),
                                                                    pointerEventData.enterEventCamera,
                                                                    out localRectTransform);

            Vector2 newTooltipPos = new Vector2(localRectTransform.x, localRectTransform.y);
            if (localRectTransform.y - tooltipRectTransform.sizeDelta.y < -canvasRectTransform.sizeDelta.y / 2)
                newTooltipPos.y += tooltipRectTransform.sizeDelta.y + GetComponent<RectTransform>().sizeDelta.y;

            if (localRectTransform.x - tooltipRectTransform.sizeDelta.x / 2 < -canvasRectTransform.sizeDelta.x / 2)
                newTooltipPos.x = tooltipRectTransform.sizeDelta.x / 2 - canvasRectTransform.sizeDelta.x / 2;

            if (localRectTransform.x + tooltipRectTransform.sizeDelta.x / 2 > canvasRectTransform.sizeDelta.x / 2)
                newTooltipPos.x = canvasRectTransform.sizeDelta.x / 2 - tooltipRectTransform.sizeDelta.x / 2;

            tooltipRectTransform.localPosition = newTooltipPos;
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (tooltip != null)
            tooltip.Activate(false);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!IsClickable)
            return;

        if (transform.parent.GetComponent<SlotBehaviour>() != null)
        {
            this.melody = null;
            this.IsClickable = false;
            this.enabled = false;
            gameObject.GetComponent<Image>().sprite = null;
            gameObject.GetComponent<Image>().enabled = false;

            transform.parent.GetComponent<SlotBehaviour>().Slotted = false;

            transform.parent.GetComponentInParent<TheodoreMenuManager>().SelectedSkill -= 1;
        }
        else
        {
            if (findSlot(slottedSkills) == -1)
                return;

            slot = slottedSkills[findSlot(slottedSkills)];
            GameObject slottedSkill = slot.transform.GetChild(0).gameObject;
            slottedSkill.GetComponent<MelodyBehaviour>().melody = melody;
            slottedSkill.GetComponent<MelodyBehaviour>().IsClickable = true;
            slottedSkill.GetComponent<MelodyBehaviour>().enabled = true;
            slottedSkill.GetComponent<Image>().color = GetComponent<Image>().color;
            slottedSkill.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
            slottedSkill.GetComponent<Image>().enabled = true;

            slot.GetComponent<SlotBehaviour>().Slotted = true;

            currentSelected++;

            if (currentSelected == maxSelected)
                IsClickable = false;

            slottedSkill.GetComponent<Button>().onClick.AddListener(() => { IsClickable = true; currentSelected -= 1; });
            slottedSkill.GetComponent<Button>().interactable = true;
            warning.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { IsClickable = true; currentSelected = 0; });
            transform.parent.GetComponentInParent<TheodoreMenuManager>().SelectedSkill += 1;
            MusicManager.Instance?.PlayUIOneShot("Hover");
        }
    }

    int findSlot(GameObject[] slotList)
    {
        for (int i = 0; i < slotList.Length; i++)
            if (!slotList[i].GetComponent<SlotBehaviour>().Slotted)
                return i;
        return -1;
    }

    public void RemoveNonPersistentEvent()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().interactable = false;
    }
}
