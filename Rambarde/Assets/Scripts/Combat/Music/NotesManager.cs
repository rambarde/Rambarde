using System;
using System.Collections.Generic;
using Bard;
using DG.Tweening;
using Melodies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Music {
    public class NotesManager : MonoBehaviour {
        
        [SerializeField] private Transform m1, m2, m3, m4;
        [SerializeField] private Transform t1, t2, t3, t4;
        [SerializeField] private GameObject notePrefab;

        private List<GameObject> notes;
        private Sequence notesSequence;
        
        public void Init(List<Melody> melodies)
        {
            notesSequence = DOTween.Sequence();
            int melodyIndex = 0;
            foreach (var melody in melodies)
            {
                // spawn notes
                for (int i = 0; i < melody.Size; i++)
                {
                    
                }
                
                melodyIndex++;
                if (melodyIndex != melodies.Count)
                {
                    // spawn separator
                    
                }
            }
        }
        
        private GameObject SpawnNote(string note, Melody melody) {
            Transform parent = null;
            Color c = Color.black;
            switch (note) {
                case "-" :
                    melody.score.Value += 1; //???
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
                    //return;
                break;
            }

            GameObject noteObj = Instantiate(notePrefab, parent);
            noteObj.GetComponent<Note>().note = int.Parse(note);
            noteObj.GetComponent<Note>().melody = melody;
            noteObj.GetComponent<NoteMove>().speed = 200f;
            noteObj.GetComponent<Image>().color = c;

            return noteObj;
        }

        private GameObject SpawnSeparator(string note, Melody melody)
        {
            return new GameObject();
        }
    }
}
