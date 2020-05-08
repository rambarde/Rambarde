using Melodies;
using UnityEngine;

namespace Music {
    public class Note : MonoBehaviour
    {
        private bool _isLongNote;
        private char _note;
        private Melody _melody;
        private bool _played;
        private float _width;

        public bool IsLongNote => _isLongNote;
        public int Data => int.Parse(_note.ToString());
        public bool LongPlay { get; set; }
        public bool Played => _played;
        public float Width => _width;

        public void Init(bool isLongNote, char note, Melody melody, float width)
        {
            _isLongNote = isLongNote;
            _note = note;
            _melody = melody;
            _played = false;
            _width = width;
        }
        
        public void Start()
        {
            _played = false;
            if (!_isLongNote) return;
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 size = rectTransform.sizeDelta;
            size.x = _width;
            rectTransform.sizeDelta = size;
        }
        
        public void Play() {
            _melody.score.Value += 1;
            _played = true;
        }
    }
}
