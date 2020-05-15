﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InstrumentBehaviour :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public Bard.Instrument instrument;
    [SerializeField]
    private bool _isClickable;
    public bool IsClickable { get { return _isClickable; } set { _isClickable = value; } }

    Tooltip instrumentTooltip;
    RectTransform canvasRectTransform;
    RectTransform tooltipRectTransform;
    GameObject[] instrumentSlots;
    GameObject[] instrumentSkillSlots;
    public GameObject[] melodiesInstrument;
    GameObject slot;
    GameObject warning;
    //GameObject counter;

    void Awake()
    {
        // do not display instruments the Bard doesnt own
        //Debug.Log(instrument.name);
        warning = transform.parent.GetComponentInParent<TheodoreMenuManager>().warning.gameObject;
        if (instrument != null && !instrument.owned)
        {
            IsClickable = false;
        }
        if (instrument != null && instrument.owned)
        {
            IsClickable = true;
        }

        if (instrument != null && IsClickable)
        {
            GetComponent<Image>().sprite = instrument.sprite;
            GetComponent<Image>().color = new Color(1, 1, 1);

            GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot");
            instrumentSlots = new GameObject[2];
            instrumentSkillSlots = new GameObject[8];
            int j = 0;
            int k = 0;

            for (int i = 0; i < slots.Length; i++)
            {
                GameObject slot = slots[i];
                if (slot.GetComponent<SlotBehaviour>() != null && slot.GetComponent<SlotBehaviour>().InstrumentSlot)
                {
                    instrumentSlots[j] = slot;
                    j += 1;
                }
                if (slot.GetComponent<SlotBehaviour>() != null && slot.GetComponent<SlotBehaviour>().InstrumentSkillSlot)
                {
                    instrumentSkillSlots[k] = slot;
                    k += 1;
                }
            }
            //melodiesInstrument = new GameObject[4];

            //for (int i = 0; i < transform.parent.GetChild(1).childCount; i++)
            //    melodiesInstrument[i] = transform.parent.GetChild(1).GetChild(i).gameObject;
        }
    }

    void Start()
    {
        if (GameObject.FindWithTag("Tooltip") != null)
        {
            instrumentTooltip = GameObject.FindWithTag("Tooltip").GetComponent<Tooltip>();
            tooltipRectTransform = GameObject.FindWithTag("Tooltip").GetComponent<RectTransform>() as RectTransform; 
            canvasRectTransform = tooltipRectTransform.parent.GetComponent<RectTransform>() as RectTransform;
        }
        
        if (instrument != null && IsClickable)
            for (int i= 0; i < melodiesInstrument.Length; i++)
            {
                Melodies.Melody melody = instrument.melodies[i];
                melodiesInstrument[i].GetComponent<MelodyBehaviour>().melody = melody;

                melodiesInstrument[i].GetComponent<Image>().sprite = melody.sprite;
                melodiesInstrument[i].GetComponent<Image>().color = new Color(1, 1, 1);
                melodiesInstrument[i].GetComponent<Image>().enabled = true;
            }

    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (instrumentTooltip != null && instrument.owned)
        {
            instrumentTooltip.setObject(gameObject);
            instrumentTooltip.Activate(true);

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
        if (instrumentTooltip != null)
            instrumentTooltip.Activate(false);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!IsClickable)
            return;

        if (transform.parent.GetComponent<SlotBehaviour>() != null)
        {
            this.instrument = null;
            this.enabled = false;
            gameObject.GetComponent<Image>().sprite = null;
            gameObject.GetComponent<Image>().enabled = false;
            transform.parent.GetComponent<SlotBehaviour>().Slotted = false;

            transform.parent.GetComponentInParent<TheodoreMenuManager>().SelectedSkill -= 4;
        }
        else
        {
            if (findSlot(instrumentSlots) == -1)
                return;

            slot = instrumentSlots[findSlot(instrumentSlots)];

            GameObject slottedSkill = slot.transform.GetChild(0).gameObject;
            slottedSkill.GetComponent<InstrumentBehaviour>().instrument = instrument;
            slottedSkill.GetComponent<InstrumentBehaviour>().enabled = true;
            slottedSkill.GetComponent<Image>().color = GetComponent<Image>().color;
            slottedSkill.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
            slottedSkill.GetComponent<Image>().enabled = true;

            slot.GetComponent<SlotBehaviour>().Slotted = true;

            foreach (Melodies.Melody melody in instrument.melodies)
                displayMelody(instrumentSkillSlots, melody);

            IsClickable = false;
            slottedSkill.GetComponent<Button>().onClick.AddListener(() => { IsClickable = true; });
            slottedSkill.GetComponent<Button>().interactable = true;
            warning.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { IsClickable = true; });
            transform.parent.GetComponentInParent<TheodoreMenuManager>().SelectedSkill += 4;
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

    private void displayMelody(GameObject[] slotList, Melodies.Melody melody) 
    {
        slot = slotList[findSlot(slotList)];

        GameObject slottedSkill = slot.transform.GetChild(0).gameObject;
        slottedSkill.GetComponent<MelodyBehaviour>().melody = melody;
        slottedSkill.GetComponent<MelodyBehaviour>().IsClickable = false;
        slottedSkill.GetComponent<Image>().sprite = melody.sprite;
        slottedSkill.GetComponent<Image>().enabled = true;

        slot.GetComponent<SlotBehaviour>().Slotted = true;
    }

    public void unDisplayMelody(GameObject slot)
    {
        GameObject slottedSkill = slot.transform.GetChild(0).gameObject;
        slottedSkill.GetComponent<MelodyBehaviour>().melody = null;
        slottedSkill.GetComponent<MelodyBehaviour>().IsClickable = false;
        slottedSkill.GetComponent<Image>().sprite = null;
        slottedSkill.GetComponent<Image>().enabled = false;

        slot.GetComponent<SlotBehaviour>().Slotted = false;
    }

    public void RemoveNonPersistentEvent()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().interactable = false;
    }

    void OnEnable()
    {
        Awake();
        Start();
    }
}
