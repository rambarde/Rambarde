using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ClientBehaviour : 
    MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler

{
    [SerializeField]
    private bool _isClickable;
    public bool IsClickable { get { return _isClickable; } set { _isClickable = value; } }

    public Client client;

    GameObject ClientImage;
    GameObject Name;
    GameObject Trait;
    GameObject archetype;
    GameObject Class;
    GameObject statEnd;
    GameObject statAtq;
    GameObject statProt;
    GameObject statPrec;
    GameObject statCrit;
    GameObject skills;
    GameObject counter;

    private void Awake()
    {
        ClientImage = transform.GetChild(1).gameObject;
        Name = transform.GetChild(2).gameObject;
        Trait = transform.GetChild(3).gameObject;
        archetype = transform.GetChild(4).gameObject;
        Class = transform.GetChild(5).gameObject;
        statEnd = transform.GetChild(6).GetChild(5).gameObject;
        statAtq = transform.GetChild(6).GetChild(6).gameObject;
        statProt = transform.GetChild(6).GetChild(7).gameObject;
        statPrec = transform.GetChild(6).GetChild(8).gameObject;
        statCrit = transform.GetChild(6).GetChild(9).gameObject;
        skills = transform.GetChild(7).gameObject;
        counter = GameObject.Find("ClientCounter");
    }

    void Start()
    {
        if (client.Character != null)
        {
            ClientImage.GetComponent<Image>().sprite = client.Character.clientImage;
            Name.GetComponent<Text>().text = client.Name;
            //Trait                                             ///add trait/envy to characterData????????
            Class.GetComponent<Text>().text = client.Character.name;

            if(client.Character.name == "Paladin" || client.Character.name == "Capitaine")
                archetype.GetComponent<Text>().text = "Stratège";
            if (client.Character.name == "Duelliste" || client.Character.name == "Roublard")
                archetype.GetComponent<Text>().text = "Téméraire";
            if (client.Character.name == "Astromancien" || client.Character.name == "Elementaliste")
                archetype.GetComponent<Text>().text = "Mage";

            statEnd.GetComponent<Text>().text = client.Character.baseStats.maxHp.ToString();
            statAtq.GetComponent<Text>().text = client.Character.baseStats.atq.ToString();
            statProt.GetComponent<Text>().text = client.Character.baseStats.baseProt + "%";
            statPrec.GetComponent<Text>().text = client.Character.baseStats.prec + "%";
            statCrit.GetComponent<Text>().text = client.Character.baseStats.crit + "%";

            for (int i = 0; i < client.SkillWheel.Length-1; i++)
            {
                GameObject skill = skills.transform.GetChild(i).gameObject;
                skill.GetComponent<SkillBehaviour>().skill = client.Character.skills[client.SkillWheel[i]];
                skill.GetComponent<SkillBehaviour>().AtqValue = client.Character.baseStats.atq;
                skill.GetComponent<Image>().sprite = client.Character.skills[client.SkillWheel[i]].sprite; 
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsClickable)
        {
            counter.GetComponent<Counter>().decrement();
            transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);

            IsClickable = true;
            GameObject.Find("Reset Client").GetComponent<Button>().onClick.RemoveListener(ResetSelected);
            transform.parent.GetComponentInParent<ClientMenuManager>().SelectedClient -= 1;
        }
        else
        {
            if (counter.GetComponent<Counter>().CurrentCount >= 3)
                return;

            counter.GetComponent<Counter>().increment();
            transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 215f / 255f, 0f);

            IsClickable = false;
            GameObject.Find("Reset Client").GetComponent<Button>().onClick.AddListener(ResetSelected);
            transform.parent.GetComponentInParent<ClientMenuManager>().SelectedClient += 1;
            MusicManager.Instance.PlayUIOneShot("Yeah"+Random.Range(0,11));
        }
    }

    public void ResetSelected()
    {
        if (IsClickable)
            return;

        transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
        transform.parent.GetComponentInParent<ClientMenuManager>().resetSelectedClient(1);
        IsClickable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<RectTransform>().SetAsLastSibling();
        GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }
}
