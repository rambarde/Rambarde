using Melodies;
using UnityEngine;

namespace Music {
    public class Note : MonoBehaviour {
        public int note;
        public Melody melody;
        private bool _played;
        private int _length;

        public bool Played => _played;
        public int Length => _length;

        public void SetLength(int length)
        {
            _length = length;
            //todo GetComponent<RectTransform>().rect.width = 
        }
        
        public void Play() {
            melody.score.Value += 1;
            _played = true;
        }
    }
}
