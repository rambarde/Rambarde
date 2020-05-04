using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour
{

    GameObject StatusPanel01;
    GameObject StatusPanel02;
    GameObject StatusPanel03;

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



        statusEffect.Add("La cible récupère 5% de son endurance maximum au début de son tour");
        statusEffect.Add("La cible perd 5% de son endurance maximum au début de son tour");
        statusEffect.Add("La cible effectue ses compétence avec 30% de précision en mois");
        statusEffect.Add("La prochaine compétence de la cible est utilisée à 100% de précision et de chances de critiques. Si la cible est aussi affectée par un bonus aux chances de critique, celui-ci n'est pas affiché et ses effets sont écrasés par ceux d'Implacable. Cependant, il continue à se réduire naturellement comme n'importe quel bonus.");
        statusEffect.Add("La cible n'agit pas à son tour de jeu (et sa roue n'avance pas d'un cran).");
        statusEffect.Add("La cible joue en premier, même si elle n'a pas été influencée par Théodore. Si plusieurs combattants sont affectés, ils jouent tous en premier selon l'ordre 'Théodore de gauche à droite' puis 'les autres de gauche à droite', et les autres combattants agissent selon l'ordre classique. Si Précipitation est appliqué sur un combattant déjà affecté par Traînard, celui-ci est soigné de ce dernier.");
        statusEffect.Add("La cible joue en dernier, même si elle a été influencée par Théodore. Si plusieurs combattants sont affectés, d'abord les autres combattants agissent selon l'ordre classique, puis les traînards jouent selon l'ordre 'Théodore de gauche à droite' suivi de 'les autres de gauche à droite'. Si Traînard est appliqué sur un combattant déjà affecté par Précipitation, celui-ci est soigné de ce dernier.");
        statusEffect.Add("Au début du round, la roue de la cible tourne jusqu'à une compétence aléatoire.");
        statusEffect.Add("La roue de la cible est invisible.	");
        statusEffect.Add("Au début du round, la roue de la cible tourne jusqu'à son incompétence.	");
        statusEffect.Add("À son tour de jeu, la cible utilise sa compétence actuelle et immédiatement la suivante.	");
        statusEffect.Add("La prochaine fois qu'un client devait utiliser une incompétence, sa roue avance immédiatement d'un cran et il utilise la compétence suivante.	");
        statusEffect.Add("Si a son tour, la cible réalise une incompétence, elle est immédiatement épuisée.	");
        statusEffect.Add("Si ce statut n'est pas soigné avant la fin de son timer, la cible est immédiatement épuisée.	");
        statusEffect.Add("La cible n'est pas affectée par les morceaux de Théodore, qu'ils soient ciblés ou non.	");
        statusEffect.Add("Le combattant affecté ne peut pas être ciblé par les compétences des monstres. Le tick down s'effectue à la fin du round, et pas à la fin du tour du combattant affecté.	");
        statusEffect.Add("La cible est toujours prise pour cible par le camp adverse. Si plusieurs combattants du même camp sont marqués, seuls ceux-ci peuvent être pris pour cible. Le tick down s'effectue à la fin du round, et pas à la fin du tour du combattant affecté.	");
        statusEffect.Add("La cible revient immédiatement à 25% de son endurance max après être épuisé. Le tick down s'effectue à la fin du round, et pas à la fin du tour du combattant affecté.	");
        statusEffect.Add("À chaque fois que le combattant affecté est ciblé par une compétence d'un monstre, il contre-attaque avec la compétence [NOM] [ID]. Le tick down s'effectue à la fin du round, et pas à la fin du tour du combattant affecté.	");
    }

    public void Activate(bool displayStatusInfo, List<int> statusID)
    {
        if(displayStatusInfo)
        {
            /* TO DO
             * afficher le nombre de panels correspondant au nombre de status
             * pour chaque status, afficher le bon nom et la bonne définition4
             * */

            gameObject.SetActive(true);

            for(int i = 0; i<3; i++)
            {
                if(i<statusID.Count)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                    transform.GetChild(i).GetChild(0).GetComponent<Text>().text = statusNames[statusID[i]];  //display status name
                    transform.GetChild(i).GetChild(1).GetComponent<Text>().text = statusEffect[statusID[i]];
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
