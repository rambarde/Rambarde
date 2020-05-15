using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuildReady : MonoBehaviour
{
    private GuildeManagerBehaviour guildManager;
    public Button pretButton;

    private void Start()
    {
        guildManager = GameObject.Find("GuildeMenu").GetComponent<GuildeManagerBehaviour>();
    }

    public void Update()
    {
        bool[] ready = guildManager.menuValid;
        bool orReady = ready[0] || ready[1] || ready[2];
        bool andReady = ready[0] && ready[1] && ready[2];
        if (andReady & orReady)
        {
            pretButton.interactable = true;
        }
        else
        {
            pretButton.interactable = false;
        }
    }

    public void checkReady()
    {
        bool[] ready = guildManager.menuValid;
        bool orReady = ready[0] || ready[1] || ready[2];
        bool andReady = ready[0] && ready[1] && ready[2];

        if (!orReady)
            Debug.Log("Rien n'est sélectionné");
            //feedback visuel

        if (orReady & !andReady)
        {
            if (!ready[0])
                Debug.Log("Menu clients non valide");
                //feedback visuel

            if (!ready[1])
                Debug.Log("Menu expédition non valide");
                //feedback visuel

            if (!ready[2])
                Debug.Log("Menu Théodore non valide");
                //feedback visuel
            //Debug.Log(guildManager.clients[0]);
        }
        if (andReady & orReady)
        {
            MusicManager.Instance?.PlayUIOneShot("Back");
            Debug.Log("C'est ok");
            //lance la scène combat ici
            GameManager.instruments = guildManager.instruments;
            GameManager.clients = guildManager.clients;
            GameManager.quest = guildManager.selectedQuest;
            GetComponent<GameManager>().SetClientsHp();
            GetComponent<GameManager>().ChangeScene(2);
        }
    }
}
