using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstrumentShop : MonoBehaviour
{
    public List<Bard.Instrument> instruments;
    public List<Button> instrumentsButtons;
    public GameObject InfoPanel;

    GameObject purchaseButton;

    int selectedInstrumentID = -1;

    // Start is called before the first frame update
    void Start()
    {
        purchaseButton = GameObject.Find("PurchaseButton");
        purchaseButton.GetComponent<Button>().interactable = false;

        for (int i = 0; i<instrumentsButtons.Count; i++)
        {
            Bard.Instrument instrument = instruments[i];
            Button button = instrumentsButtons[i];
            string instruName = instrumentsButtons[i].name;
            Debug.Log(instruName);
            GameObject.Find(instruName + "_Image").GetComponent<Image>().sprite = instrument.sprite;
            GameObject.Find(instruName + "_Name").GetComponent<Text>().text = Utils.SplitPascalCase(instrument.name);
            GameObject.Find(instruName + "_Price").GetComponent<Text>().text = instrument.price + "G";
            
            UpdateInstrumentShop();
        }
    }

    void UpdateInstrumentShop()
    {
        for (int i = 0; i < instrumentsButtons.Count; i++)
        {
           if(instruments[i].owned)
            {
                instrumentsButtons[i].interactable = false;
            }
        }
    }

    public void PurchaseInstrument()
    {
        if (selectedInstrumentID!=-1)
        {
            GameObject goldManager = GameObject.FindGameObjectWithTag("GoldLabel");
            if (goldManager.GetComponent<GoldValue>().HasEnoughGold(instruments[selectedInstrumentID].price))
            {
                goldManager.GetComponent<GoldValue>().Pay(instruments[selectedInstrumentID].price);
                instruments[selectedInstrumentID].owned = true;
                UpdateInstrumentShop();
            }
            else
            {
                goldManager.GetComponent<GoldValue>().DisplayNoGoldMessage();
            }
        }
    }
    
    public void DisplayInstrumentInfo(int instrumentID)
    {
        selectedInstrumentID = instrumentID;

        InfoPanel.transform.GetChild(0).GetComponent<Image>().sprite = instruments[selectedInstrumentID].sprite;
        InfoPanel.transform.GetChild(1).GetComponent<Text>().text = Utils.SplitPascalCase(instruments[selectedInstrumentID].name);
        InfoPanel.transform.GetChild(2).GetComponent<Text>().text = instruments[selectedInstrumentID].price.ToString() + "G";
        InfoPanel.transform.GetChild(3).GetComponent<Text>().text = instruments[selectedInstrumentID].passif;

        for(int i = 0; i<instruments[selectedInstrumentID].melodies.Length; i++)
        {
            InfoPanel.transform.GetChild(4).GetChild(i).GetComponent<Text>().text = Utils.SplitPascalCase(instruments[selectedInstrumentID].melodies[i].name) + " : " + instruments[selectedInstrumentID].melodies[i].effect;
        }

        purchaseButton.GetComponent<Button>().interactable = true;
    }
}
