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
                        if (note == null) note = c.gameObject.GetComponentInParent<Note>();
                        
                        if (note.IsLongNote)
                        {
                            if (!note.Played)
                            {
                                // long note missed
                                if (!note.LongPlay)
                                {
                                    // error animation
                                
                                    // error sound

                                    Debug.Log("error");
                                }
                            }
                        }
                        else
                        {
                            // missed note
                            if (!note.Played)
                            {
                                // error animation
                                
                                // error sound
                                
                                Debug.Log("error");
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
                        // error animation
                                
                        // error sound

                        Debug.Log("error");
                    }
                    else 
                    {
                        // missed note
                        if (_currentNotes[x - 1].Data != x)
                        {
                            // error animation
                                
                            // error sound

                            Debug.Log("error");
                        }
                        // good note
                        else
                        {
                            if (!_currentNotes[x - 1].IsLongNote)
                            {
                                _currentNotes[x - 1].Play();
                                // played note animation
                                
                                Debug.Log("played");
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
                                // error animation
                                
                                // error sound

                                Debug.Log("error");
                            }
                            else
                            {
                                _currentNotes[x - 1].Play();
                                // played note animation
                                
                                Debug.Log("played");
                            }
                        }   
                    }
                });
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
        
        private static int GetKeyUpInput() {
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
    }
}
