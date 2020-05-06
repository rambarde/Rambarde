using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    StatusWindow statusWindow;
    List<int> statusID = new List<int>();

    void Start()
    {
        if(GameObject.FindGameObjectWithTag("statusWindow")!= null)
        {
            statusWindow = GameObject.FindGameObjectWithTag("statusWindow").GetComponent<StatusWindow>();

        }
        else
        {
            Debug.Log("status window not found");
        }
    }

    public void detectStatus()
    {
        foreach(string status in statusWindow.getStatusNames())
        {
            if (GetComponent<Text>().text.Contains(status))
            {
                statusID.Add(statusWindow.getStatusNames().IndexOf(status));
                //Debug.Log(status + " détecté");
            }
        }
    }

    public void resetStatusList()
    {
        statusID.Clear();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        statusWindow.Activate(true, statusID);

        Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        statusWindow.transform.position = pz;
    }

    public void OnPointerExit(PointerEventData poinerEventData)
    {
        if (statusWindow != null)
        {
            statusWindow.Activate(false, statusID);
        } 
    }

    public void displayStatus()
    {
        if (statusID.Count > 0)
        {
            statusWindow.Activate(true, statusID);

            Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            statusWindow.transform.position = pz;
            //Debug.Log(pz);
        }
    }

    public void deactivateStatusWindow()
    {
        if (statusWindow != null)
        {
            //Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            statusWindow.transform.position = new Vector3(2000,2000,0);
        }
    }
}
