using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour
{

    GameObject StatusPanel01;
    GameObject StatusPanel02;
    GameObject StatusPanel03;

    public Sprite[] statusSprite;
    List<string> statusNames = new List<string>();
    List<string> statusEffect = new List<string>();

    void Start()
    {
        StatusPanel01 = transform.GetChild(0).gameObject;
        StatusPanel02 = transform.GetChild(1).gameObject;
        StatusPanel03 = transform.GetChild(2).gameObject;

        statusNames.Add("Régénération");
        statusNames.Add("Poison");
        statusNames.Add("Déstabilisé");
        statusNames.Add("Implacable");
        statusNames.Add("Etourdi");
        statusNames.Add("Précipitation");
        statusNames.Add("Traînard");
        statusNames.Add("Confus");
        statusNames.Add("Imprévisible");
        statusNames.Add("Inapte");
        statusNames.Add("Exalté");
        statusNames.Add("Discipliné");
        statusNames.Add("Maudit");
        statusNames.Add("Condamné");
        statusNames.Add("Assourdi");
        statusNames.Add("Invisible");
        statusNames.Add("Marqué");
        statusNames.Add("Grâce");
        statusNames.Add("Riposte");



        statusEffect.Add("Soigne un peu d'Endurance au début du tour.");
        statusEffect.Add("Perd un peu d'Endurance au début du tour.");
        statusEffect.Add("Diminue fortement la Précision.");
        statusEffect.Add("La prochaine compétence touche automatiquement sa cible avec un coup Critique.");
        statusEffect.Add("La prochaine compétence n'est pas effectuée et la roue des compétences n'avance pas à la fin du tour.");
        statusEffect.Add("Effectue sa compétence en premier.");
        statusEffect.Add("Effectue sa compétence en dernier.");
        statusEffect.Add("La roue des compétences tourne jusqu'à une compétence aléatoire au début du tour.");
        statusEffect.Add("La roue des compétences est invisible.");
        statusEffect.Add("La roue des compétences tourne jusqu'à Incompétence au début du tour.");
        statusEffect.Add("Immédiatement après avoir effectué sa compétence, effectue la compétence suivante.");
        statusEffect.Add("Au lieu d'effectuer Incompétence, effectue la compétence suivante.");
        statusEffect.Add("Effectuer Incompétence rend épuisé.");
        statusEffect.Add("Rend épuisé si ce statut n'est pas soigné avant la fin du décompte.");
        statusEffect.Add("Ne peut pas être ciblé par les morceaux de Théodore.");
        statusEffect.Add("Ne peut pas être ciblé par les compétences des adversaires.");
        statusEffect.Add("Est ciblé en priorité par les compétences des adversaires.");
        statusEffect.Add("Soigne une partie de l'Endurance après avoir été épuisé.");
        statusEffect.Add("Inflige instantanément des dégâts à un adversaire le ciblant.");
    }

    public void Activate(bool displayStatusInfo, List<int> statusID)
    {
        if(displayStatusInfo)
        {
            gameObject.SetActive(true);

            for(int i = 0; i<3; i++)
            {
                if(i<statusID.Count)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                    transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = statusSprite[statusID[i]]; //display status icon
                    transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>().text = statusNames[statusID[i]];     // display status name
                    transform.GetChild(i).GetChild(1).GetComponent<Text>().text = statusEffect[statusID[i]];                // display status effect
                }
                else
                    transform.GetChild(i).gameObject.SetActive(false);
            }





        }
        else
        {
            StatusPanel01.SetActive(false);
            StatusPanel02.SetActive(false);
            StatusPanel03.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }

    public List<string> getStatusNames()
    {
        return statusNames;
    }
}
