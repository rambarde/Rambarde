using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Characters;
using TMPro;
using UniRx;
using UnityEngine;

namespace Status {
    [Serializable]
    public abstract class StatusEffect {

        public static int INFINITE = -1;
        
        public ReactiveProperty<int> turnsLeft;
        public string spriteName;

        protected CharacterControl target;
        public EffectType type;

        private bool _justApplied = true;

        protected StatusEffect(CharacterControl target, int turns) {
            this.target = target;
            turnsLeft = new ReactiveProperty<int>(turns);
        }

        // Abstract methods
        protected virtual async Task PreTurnStart() { }

        protected virtual async Task PostTurnEnd() { }
        
        protected virtual async Task Apply() { }
        protected virtual async Task Remove() { }

        // Final methods
        public async Task TurnStart() {
            await PreTurnStart();

            if (turnsLeft.Value != INFINITE) {
                --turnsLeft.Value;
            }

            if (turnsLeft.Value  == 0) {
                await RemoveEffect();
            }
        }

        public async Task TurnEnd() {
            if (_justApplied) {
                _justApplied = false;
                return;
            }

            await PostTurnEnd();
        }

        public async Task RemoveEffect() {
            await Remove();
            target.statusEffects.Remove(this);
            target.effectTypes.Value &= ~type;
        }

        public static async Task ApplyEffect<T>(CharacterControl target, Lazy<T> addedEffect, int addedTurns = 0) where T : StatusEffect {
            //TODO: applying effect animation

            var effects = target.statusEffects;
            var effect = effects.FirstOrDefault(x => x.GetType() == typeof(T));

            if (effect is null) {
                effects.Add(addedEffect.Value);
                target.effectTypes.Value |= addedEffect.Value.type;
            }
            else {
                effect.AddTurns(addedTurns);
            }

        }
        
        public static async Task ApplyEffect(CharacterControl target, EffectType effectType, int nbrTurn) {
            // TODO: applying effect animation

            var effects = target.statusEffects;
            StatusEffect effect = effects.FirstOrDefault(e => e.type == effectType);
            if (effect is null) {
                effect = CreateEffect(target, effectType, nbrTurn);
                effects.Add(effect);
                target.effectTypes.Value |= effectType;
            } else {
                effect.turnsLeft.Value = Math.Max(nbrTurn, effect.turnsLeft.Value);
            }
        }
        
        public static async Task ApplyBuff(CharacterControl target, BuffType buffType, int nbrTurn) {
            
            DialogFilter filter = nbrTurn > 0 ? DialogFilter.Buff : DialogFilter.Unbuff;
/*            await CombatManager.Instance.dialogManager.ShowDialog(filter,
                CharacterType.None, Dialog.GetCharacterTypeFromCharacterControl(target));*/

            var effects = target.statusEffects;
            Buff effect = (Buff) effects.FirstOrDefault(e => 
                e.type == EffectType.Buff
                && ((Buff) e)._buffType == buffType);
            
            if (effect is null) {
                effect = new Buff(target, nbrTurn, buffType, nbrTurn);
                effects.Add(effect);
                target.effectTypes.Value |= EffectType.Buff;
            } else {
                effect.turnsLeft.Value = Math.Max(nbrTurn, effect.turnsLeft.Value);
            }
        }

        public static StatusEffect CreateEffect(CharacterControl target, EffectType effectType, int nbrTurn) {
            switch (effectType) {
                case EffectType.Poison :
                    return new PoisonEffect(target, 15, nbrTurn);
                case EffectType.HealthRegen :
                    return new HealthRegen(target, 15, nbrTurn);
                case EffectType.Deaf :
                    return new DeafEffect(target, nbrTurn);
                case EffectType.Destabilized :
                    return new Destabilized(target, nbrTurn);
                case EffectType.Merciless :
                    return new Merciless(target, nbrTurn);
                case EffectType.Dizzy :
                    return new Dizzy(target, nbrTurn);
                case EffectType.Rushing :
                    return new Rushing(target, nbrTurn);
                case EffectType.Lagging :
                    return new Lagging(target, nbrTurn);
                case EffectType.Confused :
                    return new Confused(target, nbrTurn);
                case EffectType.Unpredictable :
                    return new Unpredictable(target, nbrTurn);
                case EffectType.Inapt :
                    return new Inapt(target, nbrTurn);
                case EffectType.Exalted :
                    return new Exalted(target, nbrTurn);
                case EffectType.Disciplined :
                    return new Disciplined(target, nbrTurn);
                case EffectType.Cursed :
                    return new Cursed(target, nbrTurn);
                case EffectType.Condemned :
                    return new Condemned(target, nbrTurn);
                case EffectType.Invisible :
                    return new Invisible(target, nbrTurn);
                case EffectType.Marked :
                    Debug.Log("Oh hi Mark!");
                    return new Marked(target, nbrTurn);
                case EffectType.Grace :
                    return new Grace(target, nbrTurn);
                case EffectType.Counter :
                    return new Counter(target, nbrTurn);
                default:
                    Debug.LogError("Error : tried to create an invalid status effect [" + effectType + "]");
                    return null;
            }
        }

        public static string GetEffectDescription(EffectType effectType) {
            return "\n" + GetEffectTrueDescription(effectType);
        }
        private static String GetEffectTrueDescription(EffectType effectType) {
            switch (effectType) {
                case EffectType.Poison :
                    return "Perd un peu d'Endurance au début du tour.";
                case EffectType.HealthRegen :
                    return "Soigne un peu d'Endurance au début du tour.";
                case EffectType.Deaf :
                    return "Ne peut pas être ciblé par les morceaux de Théodore.";
                case EffectType.Destabilized :
                    return "Diminue fortement la Précision.";
                case EffectType.Merciless :
                    return "La prochaine compétence touche automatiquement sa cible avec un coup Critique.";
                case EffectType.Dizzy :
                    return "La prochaine compétence n'est pas effectuée et la roue des compétences n'avance pas à la fin du tour.";
                case EffectType.Rushing :
                    return "Effectue sa compétence en premier.";
                case EffectType.Lagging :
                    return "Effectue sa compétence en dernier.";
                case EffectType.Confused :
                    return "La roue des compétences tourne jusqu'à une compétence aléatoire au début du tour.";
                case EffectType.Unpredictable :
                    return "La roue des compétences est invisible.";
                case EffectType.Inapt :
                    return "La roue des compétences tourne jusqu'à <b>Incompétence</b> au début du tour.";
                case EffectType.Exalted :
                    return "Immédiatement après avoir effectué sa compétence, effectue la compétence suivante.";
                case EffectType.Disciplined :
                    return "Au lieu d'effectuer <b>Incompétence</b>, effectue la compétence suivante.";
                case EffectType.Cursed :
                    return "Effectuer <b>Incompétence</b> rend épuisé.";
                case EffectType.Condemned :
                    return "Rend épuisé si ce statut n'est pas soigné avant la fin du décompte.";
                case EffectType.Invisible :
                    return "Ne peut pas être ciblé par les compétences des adversaires.";
                case EffectType.Marked :
                    return "Est ciblé en priorité par les compétences des adversaires.";
                case EffectType.Grace :
                    return "Soigne une partie de l'Endurance après avoir été épuisé.";
                case EffectType.Counter :
                    return "Inflige instantanément des dégâts à un adversaire le ciblant.";
                default:
                    Debug.LogError("Error : tried to get description of an invalid status effect [" + effectType + "]");
                    return null;
            }
        }

        public static String GetEffectName(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.Poison:
                    return "Poison";
                case EffectType.HealthRegen:
                    return "Régénération";
                case EffectType.Deaf:
                    return "Assourdi";
                case EffectType.Destabilized:
                    return "Déstabilisé";
                case EffectType.Merciless:
                    return "Implacable";
                case EffectType.Dizzy:
                    return "Étourdi";
                case EffectType.Rushing:
                    return "Précipitation";
                case EffectType.Lagging:
                    return "Traînard";
                case EffectType.Confused:
                    return "Confus";
                case EffectType.Unpredictable:
                    return "Imprévisible";
                case EffectType.Inapt:
                    return "Inapte";
                case EffectType.Exalted:
                    return "Exalté";
                case EffectType.Disciplined:
                    return "Discipliné";
                case EffectType.Cursed:
                    return "Maudit";
                case EffectType.Condemned:
                    return "Condamné";
                case EffectType.Invisible:
                    return "Invisible";
                case EffectType.Marked:
                    return "Marqué";
                case EffectType.Grace:
                    return "Grâce";
                case EffectType.Counter:
                    return "Riposte";
                default:
                    Debug.LogError("Error : tried to get name of an invalid status effect [" + effectType + "]");
                    return null;
            }
        }


        public void AddTurns(int n) {
            turnsLeft.Value += n;
        }
    }

    [Flags]
    public enum EffectType {
        None = 0,
        Buff = 1 << 1,           //-
        HealthRegen = 1 << 2,    //-
        Poison = 1 << 3,         //-
        Destabilized = 1 << 4,   //-
        Merciless = 1 << 5,      
        Dizzy = 1 << 6,          //-
        Rushing = 1 << 7,        //-
        Lagging = 1 << 8,        //-
        Confused = 1 << 9,       //-
        Unpredictable  = 1 << 10,
        Inapt = 1 << 11,         //-
        Exalted = 1 << 12,       //~
        Disciplined = 1 << 13,   //-
        Cursed = 1 << 14,        //-
        Condemned = 1 << 15,     //-
        Deaf = 1 << 16,          //-
        Invisible = 1 << 17,     
        Marked = 1 << 18,        
        Grace = 1 << 19,         //-
        Counter = 1 << 20,       
    }
}