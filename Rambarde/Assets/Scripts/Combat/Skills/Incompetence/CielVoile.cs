using System.Linq;
using Characters;
using System.Threading.Tasks;
using Status;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "CielVoile", menuName = "Skill/Incompetence/CielVoile")]
    public class CielVoile : Skill {
        
        public new bool IsIncompetence => true;

        private EffectType[] toRemove = {
            EffectType.HealthRegen, EffectType.Merciless, EffectType.Rushing, EffectType.Exalted,
            EffectType.Disciplined, EffectType.Invisible, EffectType.Grace
        };
        
        public new async Task Execute(CharacterControl s) {
            foreach (var eff in CombatManager.Instance.teams[(int) s.team]
                .SelectMany(c => c.statusEffects.Where(e => toRemove.Contains(e.type)))) {
                await eff.RemoveEffect();
            }
        }
    }
}

/*
Eneleve Régénération, Implacable, Précipitation, Exalté, Discipliné, Invisible, Grâce
*/