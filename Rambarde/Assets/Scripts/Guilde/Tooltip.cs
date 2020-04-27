using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tooltip : MonoBehaviour
{
    public GameObject TooltipObject { get; set; }

    GameObject Name;
    GameObject effect;
    GameObject target;
    GameObject inspiration;
    GameObject trance;
    GameObject type;

    string baseCosts;
    string baseGeneration;

    void Start()
    {
        Name = this.transform.GetChild(1).gameObject;
        effect = this.transform.GetChild(2).gameObject;
        inspiration = this.transform.GetChild(3).gameObject;
        trance = this.transform.GetChild(4).gameObject;
        type = this.transform.GetChild(5).gameObject;

        baseCosts = "Coûte: \n";
        baseGeneration = "Génère:\n";

        Activate(false);
    }

    public void Activate(bool m_bool)
    {
        if (m_bool)
        {
            if (TooltipObject.GetComponent<MelodyBehaviour>() != null) 
            {
                Melodies.Melody melody = TooltipObject.GetComponent<MelodyBehaviour>().melody;

                Name.GetComponent<Text>().text = Utils.SplitPascalCase(melody.name);
                effect.GetComponent<Text>().text = melody.effect;
                inspiration.GetComponent<Text>().text = stringInspiration(melody);
                trance.GetComponent<Text>().text = stringTrance(melody);
                type.GetComponent<Text>().text = "Tier " + melody.tier;         //add trance melody possibility

                inspiration.SetActive(true);
                trance.SetActive(true);
                type.SetActive(true);
            }

            if (TooltipObject.GetComponent<InstrumentBehaviour>() != null)
            {
                Bard.Instrument instrument = TooltipObject.GetComponent<InstrumentBehaviour>().instrument;

                Name.GetComponent<Text>().text = Utils.SplitPascalCase(instrument.name);
                effect.GetComponent<Text>().text = instrument.passif;
                type.GetComponent<Text>().text = instrument.type;

                type.SetActive(true);
            }

            if (TooltipObject.GetComponent<SkillBehaviour>() != null)
            {
                Skills.Skill skill = TooltipObject.GetComponent<SkillBehaviour>().skill;

                Name.GetComponent<Text>().text = Utils.SplitPascalCase(skill.name);
                effect.GetComponent<Text>().text = skill.description;
            }

            Name.SetActive(true);
            effect.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            Name.SetActive(false);
            effect.SetActive(false);
            inspiration.SetActive(false);
            trance.SetActive(false);
            type.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    string stringInspiration(Melodies.Melody melody)
    {
        int inspi = melody.inspirationValue;
        string s_inspi;
        if (inspi > 0)
            s_inspi = baseGeneration + inspi + " d'inspiration";
        else
            s_inspi = baseCosts + -inspi + " d'inspiration";

        return s_inspi;
    }

    string stringTrance(Melodies.Melody melody)
    {
        int trance = melody.tranceValue;
        string s_trance;
        if (trance > 0)
            s_trance = baseGeneration + trance + " de transe";
        else
            s_trance = baseCosts + -trance + " de transe";

        return s_trance;
    }

    public void setObject(GameObject _object) { this.TooltipObject = _object; }
}
