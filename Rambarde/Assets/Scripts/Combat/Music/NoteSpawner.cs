using System;
using System.Collections.Generic;
using Bard;
using Melodies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Music {
    public class NoteSpawner : MonoBehaviour {
        
        [SerializeField] private Transform m1, m2, m3, m4;
        [SerializeField] private GameObject notePrefab;

        public void InitNotes(List<NoteInfo> notes)
        {
            foreach (var noteInfo in notes)
            {
                
            }
        }
        
        private GameObject SpawnNote(string note, Melody melody) {
            Transform parent;
            Color c;
            switch (note) {
                case "-" :
                    melody.score.Value += 1;
                    return new GameObject();
                
                case "1" :
                    parent = m1;
                    c = Color.black;
                    break;
                case "2" :
                    parent = m2;
                    c = new Color(0.5660378f, 0.2369758f, 0.09878961f);
                    break;
                case "3" :
                    parent = m3;
                    c = new Color(1.0f, 0.8118182f, 0.655f);
                    break;
                case "4" :
                    parent = m4;
                    c = Color.white;
                    break;
                default:
                    Debug.Log("warning : tried to spawn a note of unkown type : [" + note + "]");
                    return;
            }

            GameObject noteObj = Instantiate(notePrefab, parent);
            noteObj.GetComponent<Note>().note = int.Parse(note);
            noteObj.GetComponent<Note>().melody = melody;
            noteObj.GetComponent<NoteMove>().speed = 200f;
            noteObj.GetComponent<Image>().color = c;
        }
    }
}
