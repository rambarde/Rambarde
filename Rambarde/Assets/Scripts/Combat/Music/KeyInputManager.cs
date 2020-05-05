using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Music {
    public class KeyInputManager : MonoBehaviour
    {
        public List<SphereCollider> colliders;
        //public List<Image> InputImages;

        private Note _currentNote;
        void Start() {
            for (int i = 0; i < colliders.Count; i++) {
                colliders[i]
                    .OnTriggerEnterAsObservable()
                    .Where(c => c.gameObject.CompareTag("Note"))
                    .Subscribe(c => _currentNote = c.gameObject.GetComponent<Note>())
                    .AddTo(this);
                colliders[i]
                    .OnTriggerExitAsObservable()
                    .Where(c => c.gameObject.CompareTag("Note"))
                    .Subscribe(c => {
                        //Missed
                        NoteMissed(c.gameObject.GetComponent<Note>().note);
                        _currentNote = null;
                    }).AddTo(this);
            }

            this.UpdateAsObservable()
                .Where(_ => GetInput() != 0)
                .Select(x => GetInput())
                .Subscribe(x => {

                    if (_currentNote != null && _currentNote.note == x) {
                        //Played
                        _currentNote.Play();
                        Destroy(_currentNote.gameObject);
                        _currentNote = null;
                    
                        
                        
                    } else {
                        Debug.Log("boooooooooh");
                        NoteMissed(x);
                    }
                }).AddTo(this);
        }

        private void TweenSequenceWithDelay(Tween a, Tween b, float delay) {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(a);
            sequence.AppendInterval(delay);
            sequence.Append(b);
            sequence.Play();
        }

        private void NoteMissed(int x)
        {
            MusicManager.Instance.PlayBuzz();
            Debug.Log("boooooooooh");
            //TweenSequenceWithDelay(
            //    InputImages[x - 1].DOColor(Color.red, 0.2f),
            //    InputImages[x - 1].DOColor(Color.white, 0.2f),
            //    0.3f);
        }

        private static int GetInput() {
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
    }
}
