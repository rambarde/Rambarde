using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using System.Threading.Tasks;
using DG.Tweening;
using Melodies;
using Music;
using UniRx;
//using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace Bard {
    
    public class BardControl : MonoBehaviour {
        
        public Hud hud;
        public Inspiration inspiration;
        public List<Instrument> instruments;
        public ReactiveProperty<int> actionPoints;
        public ReactiveProperty<int> maxActionPoints;
        public List<Melody> selectedMelodies = new List<Melody>();
        public ReactiveCommand onDone = new ReactiveCommand();
        public Animator animator;
        public NotesManager notesManager;

        [SerializeField] private int baseActionPoints;
        [SerializeField] private int errorMargin = 1;
        private int _selectedInstrumentIndex;
        private static readonly int Right = Animator.StringToHash("Right");
        private static readonly int Wrong = Animator.StringToHash("Wrong");

        private void Start() {
            inspiration = GetComponent<Inspiration>();
            actionPoints = new ReactiveProperty<int>(baseActionPoints);
            maxActionPoints = new ReactiveProperty<int>(baseActionPoints);
            if (!CombatManager.Instance.ignoreGameManager) {
                instruments = GameManager.instruments;
            } else {
                CombatManager.Instance.bard = this;
            }
            
            SetActionPlayableMelodies();
            SetInspirationPlayableMelodies();
            hud.Init(this);
        }

        public void SelectMelody(Melody melody, CharacterControl target = null) {
            if (! instruments.Any(instrument => instrument.melodies.Contains(melody))) {
                Debug.Log("Warning : Melody [" + melody.name +"] is not equipped");
                MusicManager.Instance?.PlayUIOneShot("Accept");
                return;
            }

            if (!melody.isPlayable.Value) {
                Debug.Log("Warning : Melody [" + melody.name + "] is not playable");
                MusicManager.Instance?.PlayUIOneShot("Accept");
                return;
            }
            MusicManager.Instance?.PlayUIOneShot("Hover");
            SetActionPlayableMelodies();
            SetInspirationPlayableMelodies();

            if (_selectedInstrumentIndex == 0) { //if no instrument was selected
                //select melody's instrument
                _selectedInstrumentIndex = instruments.FindIndex(i => i.melodies.Contains(melody));
            }

            //if an instrument is selected
            if (_selectedInstrumentIndex != 0) {
                //make other instrument melodies unplayable
                int unselected = _selectedInstrumentIndex == 1 ? 2 : 1;
                foreach (Melody m in instruments[unselected].melodies) {
                    m.isPlayable.Value = false;
                }
            }

            selectedMelodies.Add(melody);
            actionPoints.Value -= melody.Size;
            inspiration.SelectMelody(melody);
            
            melody.isPlayable.Value = false;
            melody.target = target;
        }

        /**
         * Make melodies playable or not based on the action points
         */
        private void SetActionPlayableMelodies()
        {
            foreach (var melody in instruments.SelectMany(instrument => instrument.melodies)) 
            {
                if(melody != null)
                    melody.isPlayable.Value &= actionPoints.Value - melody.Size >= 0;
            }
        }

        private void SetInspirationPlayableMelodies() 
        {
            foreach (var melody
                in instruments.SelectMany(instrument => instrument.melodies).Where(m => m.isPlayable.Value)) {
                switch (melody.tier) {
                    case 2:
                        melody.isPlayable.Value = (inspiration.current.Value >= inspiration.tier2MinValue);
                        break;
                    case 3:
                        melody.isPlayable.Value = (inspiration.current.Value >= inspiration.tier3MinValue);
                        break;
                    default:
                        melody.isPlayable.Value = melody.isPlayable.Value;
                        break;
                }
            }
        }

        public void Undo() {
            // reset action points
            foreach (var melody in selectedMelodies) {
                inspiration.UnselectMelody(melody);
            }
            MusicManager.Instance?.PlayUIOneShot("Hover");
            Reset();
        }

        public void Reset() {
            _selectedInstrumentIndex = 0;
            actionPoints.Value = maxActionPoints.Value;
            
            foreach (Instrument instrument in instruments) {
                foreach (Melody melody in instrument.melodies) {
                    melody.isPlayable.Value = true;
                    melody.score.Value = 0;
                }
            }
            selectedMelodies.Clear();
            
            SetActionPlayableMelodies();
            SetInspirationPlayableMelodies();
        }

        public async void Done() 
        {
            MusicManager.Instance?.PlayUIOneShot("Hover");
            await CombatManager.Instance.ExecTurn();
        }

        public async Task ExecMelodies() {
            CombatManager.Instance.combatPhase.Value = CombatPhase.ExecMelodies;
            
            foreach (var melody in selectedMelodies) {
                //apply melodies based on their score (and reset their score)
                if (melody.score.Value >= melody.MaxScore() - errorMargin) {
                    await melody.Execute();
                    inspiration.current.Value += melody.inspirationValue;
                    inspiration.ResetTurnValues();
                } else {
                    Debug.Log("you failed melody [" + melody.name +"] : " + melody.score.Value + "/" + melody.MaxScore());
                }
            }
        }
        
        //                           bpm\      /beat division(croche)
        //private float _beat = 60f / (110f * 3f);

        public void InitRhythmGame()
        {
            CombatManager.Instance.combatPhase.Value = CombatPhase.RhythmGame;
            
            selectedMelodies.ForEach(m => m.score.Value = 0);
            
            notesManager.Init(selectedMelodies);
        }

        public async Task StartRhythmGame()
        {
            await notesManager.Play();
        }

        public void RightNoteAnimation()
        {
            animator.SetTrigger(Right);
        }
        
        public void WrongNoteAnimation()
        {
            animator.SetTrigger(Wrong);
        }
    }
}