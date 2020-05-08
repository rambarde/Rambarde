using System;
using System.Linq;
using System.Threading.Tasks;
using Bard;
using Characters;
using Music;
using Status;
using UniRx;
using UnityEngine;

namespace Melodies {
    public abstract class Melody : ScriptableObject 
    {
        [SerializeField] private string data;
        [SerializeField] private string parallelData;
        [SerializeField] private float bpm;
        [SerializeField] private int measure;

        public string Data => data;
        public string ParallelData => parallelData;
        public int Size => data.Length;
        public float Beat => 60f / bpm;
        public int Measure => measure;

        public MelodyTargetMode targetMode;
        [NonSerialized] public ReactiveProperty<int> score = new ReactiveProperty<int>(0);
        [NonSerialized] public CharacterControl target = null;
        [NonSerialized] public ReactiveProperty<bool> isPlayable = new ReactiveProperty<bool>(true);

        public int tier;
        [TextArea] public string effect;
        public int inspirationValue;
        public int tranceValue;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        public Sprite sprite;
        public AudioClip clip;
        
        public async Task Execute() {
            switch (targetMode) {
                case MelodyTargetMode.Anyone :
                case MelodyTargetMode.OneAlly :
                case MelodyTargetMode.OneEnemy :
                    if (target.HasEffect(EffectType.Deaf)) {
                        break;
                    }
                    target.influenced = true;
                    await ExecuteOnTarget(target);
                    break;
                case MelodyTargetMode.Everyone :
                    foreach (var character in CombatManager.Instance.teams
                            .SelectMany(_=>_)
                            .Where(c => !c.HasEffect(EffectType.Deaf))) {
                        character.influenced = true;
                        await ExecuteOnTarget(character);
                    }
                    break;
                case MelodyTargetMode.EveryAlly :
                    foreach (var character in CombatManager.Instance.teams[0]
                            .Where(c => !c.HasEffect(EffectType.Deaf))) {
                        character.influenced = true;
                        await ExecuteOnTarget(character);
                    }
                    break;
                case MelodyTargetMode.EveryEnemy :
                    foreach (CharacterControl character in CombatManager.Instance.teams[1]
                            .Where(c => !c.HasEffect(EffectType.Deaf))) {
                        character.influenced = true;
                        await ExecuteOnTarget(character);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        protected abstract Task ExecuteOnTarget(CharacterControl t);

        public (NoteInfo n1, NoteInfo n2) CreateNotes(int index, Melody melody, float twoNotesDist)
        {
            if (index >= data.Length) return (null, null);
            
            var n1 = CreateNote(data, index, melody, twoNotesDist);;
            var n2 = CreateNote(parallelData, index, melody, twoNotesDist);
            return (n1, n2);
        }

        private NoteInfo CreateNote(string noteData, int index, Melody melody, float twoNotesDist)
        {
            int length = 1;
            NoteInfo note = null;
            if (noteData[index] != '-' && noteData[index] != '_')
            {
                // the last note is necessarily simple
                if (index == noteData.Length)
                {
                    note = new NoteInfo {IsLongNote = false, Note = noteData[index], Melody = melody};
                }
                else
                {
                    // long
                    if (noteData[index + 1] == '_')
                    {
                        while (noteData[index + length] == '_')
                            length++;

                        note = new NoteInfo
                        {
                            IsLongNote = true, Note = noteData[index], Melody = melody,
                            Width = twoNotesDist * (length - 1) + 65 // there are n-1 spaces between n notes
                        }; 
                    }
                    // simple
                    else
                    {
                        note = new NoteInfo {IsLongNote = false, Note = noteData[index], Melody = melody};
                    }
                }
            }
            else if (noteData[index] == '-') score.Value += 1;

            return note;
        }
    }
}