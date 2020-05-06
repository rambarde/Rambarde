using System;
using System.Linq;
using System.Threading.Tasks;
using Bard;
using Characters;
using Status;
using UniRx;
using UnityEngine;

namespace Melodies {
    public abstract class Melody : ScriptableObject {
        [SerializeField] private string data;
        public string Data => data;
        
        [SerializeField] private string parallelData;
        public string ParallelData => parallelData;
        public int Size => data.Length; 
        
        [SerializeField] private float beat;
        public float Beat => beat;
        
        public MelodyTargetMode targetMode;
        [NonSerialized] public ReactiveProperty<int> score = new ReactiveProperty<int>(0);
        
        [NonSerialized] public CharacterControl target = null;
        [NonSerialized] public ReactiveProperty<bool> isPlayable = new ReactiveProperty<bool>(true);

        public int tier;
        [TextArea] public string effect;
        public int inspirationValue;
        public int tranceValue;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        public Sprite sprite;

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

    }
}