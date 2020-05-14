using System.Collections.Generic;
using Bard;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Music {
    public class RythmGamePlayManager : MonoBehaviour
    {
        public List<CircleCollider2D> colliders;
        public BardControl bardControl;
        public MusicManager musicManager;
        public Sprite rightIcon;
        public Sprite wrongIcon;
        public GameObject noteFeedback1;
        public GameObject noteFeedback2;
        public GameObject noteFeedback3;
        public GameObject noteFeedback4;

        private Note[] _currentNotes;

        void Start() 
        {
            _currentNotes = new Note[4];
            
            for (int i = 0; i < colliders.Count; i++) {
                var index = i;
                // 
                colliders[i]
                    .OnTriggerEnter2DAsObservable()
                    .Where(c => c.gameObject.CompareTag("Note"))
                    .Subscribe(c => 
                    {
                        Note note = c.gameObject.GetComponent<Note>();
                        if (note == null)
                        {
                            note = c.gameObject.GetComponentInParent<Note>();
                            note.LongPlay = c.gameObject.name.Contains("Start");
                        }
                        _currentNotes[index] = note;
                    }).AddTo(this);
                
                colliders[i]
                    .OnTriggerExit2DAsObservable()
                    .Where(c => c.gameObject.CompareTag("Note"))
                    .Subscribe(c =>
                    {
                        Note note = c.gameObject.GetComponent<Note>();
                        if (note == null) note = c.gameObject.GetComponentInParent<Note>();
                        
                        if (note.IsLongNote)
                        {
                            if (!note.Played)
                            {
                                // long note missed
                                if (!note.LongPlay)
                                {
                                    // error animation error sound
                                    PlayFeedbackAnimation(note.Data, false);
                                }
                            }
                        }
                        else
                        {
                            // missed note
                            if (!note.Played)
                            {
                                // error animation error sound
                                PlayFeedbackAnimation(note.Data, false);
                            }
                        }
                    }).AddTo(this);
            }

            this.UpdateAsObservable()
                .Where(_ => GetKeyDownInput() != 0)
                .Select(x => GetKeyDownInput())
                .Subscribe(x => 
                {
                    // missed note
                    if (_currentNotes[x - 1] == null)
                    {
                        // error animation error sound
                        PlayFeedbackAnimation(x, false);
                    }
                    else 
                    {
                        // missed note
                        if (_currentNotes[x - 1].Data != x)
                        {
                            // error animation error sound
                            PlayFeedbackAnimation(x, false);
                        }
                        // good note
                        else
                        {
                            if (!_currentNotes[x - 1].IsLongNote)
                            {
                                // played note animation
                                _currentNotes[x - 1].Play();
                                PlayFeedbackAnimation(x, true);
                            }
                        }
                    }
                }).AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => GetKeyUpInput() != 0)
                .Select(x => GetKeyUpInput())
                .Subscribe(x =>
                {
                    if (_currentNotes[x - 1] != null)
                    {
                        if (_currentNotes[x - 1].IsLongNote)
                        {
                            if (_currentNotes[x - 1].LongPlay)
                            {
                                // error animation error sound
                                PlayFeedbackAnimation(x, false);
                            }
                            else
                            {
                                // played note animation
                                _currentNotes[x - 1].Play();
                                PlayFeedbackAnimation(x, true);
                            }
                        }   
                    }
                });
        }
        
        

        private static int GetKeyDownInput()
        {
            if (CombatManager.Instance.combatPhase.Value != CombatPhase.RhythmGame) return 0;
            
            if (Input.GetKeyDown(KeyCode.A)) {
                return 1;
            }
            if (Input.GetKeyDown(KeyCode.Z)) {
                return 2;
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                return 3;
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                return 4;
            }
            
            return 0;
        }
        
        private static int GetKeyUpInput() 
        {
            if (CombatManager.Instance.combatPhase.Value != CombatPhase.RhythmGame) return 0;
            
            if (Input.GetKeyUp(KeyCode.A)) {
                return 1;
            }
            if (Input.GetKeyUp(KeyCode.Z)) {
                return 2;
            }
            if (Input.GetKeyUp(KeyCode.E)) {
                return 3;
            }
            if (Input.GetKeyUp(KeyCode.R)) {
                return 4;
            }
            
            return 0;
        }

        private void LogNotePlay(string message) {
            Debug.Log(message);
        }

        private void FeedbackIconAnimation(int note, bool noteState)
        {
            GameObject feedback = null;
            Sprite icon = noteState ? rightIcon : wrongIcon;
            switch (note)
            {
                case 1:
                    feedback = noteFeedback1;
                    break;
                case 2:
                    feedback = noteFeedback2;
                    break;
                case 3:
                    feedback = noteFeedback3;
                    break;
                case 4:
                    feedback = noteFeedback4;
                    break;
            }

            if (feedback == null) return;
            
            var feedbackTransform = feedback.transform;
            var feedbackIcon = feedback.GetComponent<Image>();

            feedbackTransform.localScale = Vector3.one;
            feedbackIcon.sprite = icon;

            Sequence sequence = DOTween.Sequence();
            Sequence iconSequence = DOTween.Sequence();
            iconSequence.Insert(0, feedbackIcon.DOFade(0.8f, 0.3f));
            iconSequence.Insert(0.3f, feedbackIcon.DOFade(0, 0.2f));
            sequence.Insert(0, iconSequence);
            sequence.Insert(0, feedbackTransform.DOScale(new Vector3(2.5f, 2.5f, 2.5f), 0.3f));

            sequence.Play();
        }

        private void PlayFeedbackAnimation(int note, bool noteState)
        {
            FeedbackIconAnimation(note, noteState);
            if(noteState == true)
                bardControl.RightNoteAnimation();
            else
            {
                bardControl.WrongNoteAnimation();
                MusicManager.Instance.PlayBuzz();
            }
        }
    }
}
