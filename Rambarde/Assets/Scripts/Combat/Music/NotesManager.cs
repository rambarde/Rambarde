using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bard;
using DG.Tweening;
using Melodies;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Music {
    public class NotesManager : MonoBehaviour
    {
        [SerializeField] private float notesSpeed = 200; //pixel per sec
        [SerializeField] private RectTransform m1, m2, m3, m4;
        [SerializeField] private RectTransform t1, t2, t3, t4;
        [SerializeField] private RectTransform separatorSpawner, separatorTarget;
        [SerializeField] private RectTransform musicStartPosition;
        [SerializeField] private GameObject simpleNotePrefab;
        [SerializeField] private GameObject longNotePrefab;
        [SerializeField] private GameObject separatorPrefab;

        private List<GameObject> notes;
        private Sequence notesSequence;
        private float screenNotesDistance;
        private float noteScrollDuration;

        public float MusicStartDelay => (m1.rect.x - musicStartPosition.rect.x) / notesSpeed;
        
        public void Init(List<Melody> melodies)
        {
            notesSequence = DOTween.Sequence();
            screenNotesDistance = -t1.anchoredPosition.x;
            float twoNotesDist;
            int melodyIndex = 0;
            int noteIndex = 0;
            foreach (var melody in melodies)
            {
                twoNotesDist = melody.Beat * notesSpeed; // note offset
                NoteInfo n1, n2;
                // spawn notes
                for (int i = 0; i < melody.Size; i++)
                {
                    (n1, n2) = melody.CreateNotes(i, melody, twoNotesDist);
                    if (n1 != null)
                        SpawnNote(n1, melody, notesSequence, noteIndex * melody.Beat);
                    if (n2 != null)
                        SpawnNote(n2, melody, notesSequence, noteIndex * melody.Beat);
                    noteIndex++;
                }
                
                if (melodyIndex+1 != melodies.Count) // no sep at the end
                {
                    // spawn separator
                    SpawnSeparator(notesSequence, (noteIndex - 1) * melody.Beat + melody.Beat / 2);
                }
                    
                melodyIndex++;
            }
        }

        public async Task Play()
        {
            notesSequence.Play();
                
            await Utils.AwaitObservable(
                this.UpdateAsObservable()
                    .TakeWhile(_ => notesSequence.IsActive() && notesSequence.IsPlaying())
            );
        }
        
        private void SpawnNote(NoteInfo noteInfo, Melody melody, Sequence notesSequence, float delay) {
            RectTransform parent = null;
            RectTransform target = null;
            Color c = Color.black;
            switch (noteInfo.Note) {
                case '1':
                    parent = m1;
                    target = t1;
                    c = Color.black;
                    break;
                case '2':
                    parent = m2;
                    target = t2;
                    c = new Color(0.5660378f, 0.2369758f, 0.09878961f);
                    break;
                case '3':
                    parent = m3;
                    target = t3;
                    c = new Color(1.0f, 0.8118182f, 0.655f);
                    break;
                case '4':
                    parent = m4;
                    target = t4;
                    c = Color.white;
                    break;
            }
            
            // Instantiate 
            GameObject noteObj = Instantiate(noteInfo.IsLongNote ? longNotePrefab : simpleNotePrefab, parent);
            //offset
            float offset = noteInfo.IsLongNote ? (noteInfo.Width - 65) / 2.0f : 0;
            Vector2 pos = ((RectTransform) noteObj.transform).anchoredPosition;
            pos.x += offset ;
            ((RectTransform) noteObj.transform).anchoredPosition = pos;
            // Init notes
            Note note = noteObj.AddComponent<Note>();
            note.Init(noteInfo.IsLongNote, noteInfo.Note, noteInfo.Melody, noteInfo.Width);
            noteObj.GetComponent<Image>().color = c;
            // Add to sequence
            if (target != null)
                notesSequence.Insert(delay,
                    ((RectTransform) noteObj.transform)
                    .DOAnchorPosX(target.anchoredPosition.x - (noteInfo.IsLongNote ? (noteInfo.Width - 65) / 2.0f : 0),
                        (screenNotesDistance + offset * 2) / notesSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => Destroy(noteObj)));
        }

        private void SpawnSeparator(Sequence notesSequence, float delay)
        {
            // Instantiate 
            GameObject noteSepObj = Instantiate(separatorPrefab, separatorSpawner);
            // Add to sequence
            notesSequence.Insert(delay,
                ((RectTransform) noteSepObj.transform)
                .DOAnchorPosX(separatorTarget.anchoredPosition.x,
                    screenNotesDistance / notesSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(noteSepObj)));
        }
    }
}
