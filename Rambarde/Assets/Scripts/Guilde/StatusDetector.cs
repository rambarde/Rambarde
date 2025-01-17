﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    StatusWindow statusWindow;
    List<int> statusID = new List<int>();

    Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GameObject.Find("GuildeMenu").GetComponent<Canvas>();

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
        RectTransform rt = (RectTransform)gameObject.transform;
        //statusWindow.transform.position = gameObject.transform.position + new Vector3(rt.rect.width/2.0f, 0, 0);
        statusWindow.transform.position = Utils.WorldToUiSpace(parentCanvas, gameObject.transform.position + new Vector3(rt.rect.width / 2.0f, 0, 0));

        statusWindow.Activate(true, statusID);
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
            RectTransform rt = (RectTransform)gameObject.transform;
            RectTransform sw_rt = (RectTransform)statusWindow.transform;
            

            switch(statusID.Count)
            {
                case (1):
                case (2): statusWindow.transform.position = gameObject.transform.position + new Vector3(rt.rect.width, - sw_rt.rect.height / 3.4f, 0); break;
                case (3): statusWindow.transform.position = gameObject.transform.position + new Vector3(rt.rect.width, 0, 0); break;
            }
            //statusWindow.transform.position = gameObject.transform.position + Utils.WorldToUiSpace(parentCanvas, new Vector3(0, -sw_rt.rect.height / 3.0f, 0));
            statusWindow.Activate(true, statusID);
        }
    }

    public void deactivateStatusWindow()
    {
        if (statusWindow != null)
        {
            statusWindow.transform.position = new Vector3(2000,2000,0);
        }
    }
}
