using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Music {
    public class RythmGamePlayManager : MonoBehaviour
    {
        public List<CircleCollider2D> colliders;
        //public List<Image> InputImages;

        private Note[] _currentNotes;
        void Start() {
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
                        if (note.IsLongNote)
                        {
                            if (!note.Played)
                            {
                                // long note missed
                                if (!note.LongPlay)
                                {
                                    // error animation
                                
                                    // error sound

                                }
                            }
                            else
                            {
                                
                            }
                        }
                        else
                        {
                            // missed note
                            if (!note.Played)
                            {
                                // error animation
                                
                                // error sound
                                
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
                    if (_currentNotes[x-1] == null) {
                        
                    }
                    else 
                    {
                        // missed note
                        if (_currentNotes[x - 1].Data != x)
                        {
                            
                        }
                        // good note
                        else
                        {
                            if (_currentNotes[x - 1].IsLongNote)
                            {
                                // while the button is pressed
                                this.UpdateAsObservable()
                                    .SkipWhile(_ => GetKeyInput() == x)
                                    .Subscribe(_ =>
                                    {
                                        
                                    })
                            }
                            
                            // played note animation
                            
                        }
                    }
                }).AddTo(this);
        }

        private static int GetKeyDownInput() {
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
        
        private static int GetKeyInput() {
            if (Input.GetKey(KeyCode.A)) {
                return 1;
            }
            if (Input.GetKey(KeyCode.Z)) {
                return 2;
            }
            if (Input.GetKey(KeyCode.E)) {
                return 3;
            }
            if (Input.GetKey(KeyCode.R)) {
                return 4;
            }
            
            return 0;
        }
    }
}
