using System.Linq;
using Characters;
using System.Threading.Tasks;
using Status;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "PulsarSkill", menuName = "Skill/PulsarSkill")]
    public class PulsarSkill : Skill {

        public new async Task Execute(CharacterControl s) {
            CharacterControl target = RandomTargetInTeam(s.team + 1);
            if (hasForcedTarget) {
                target = forcedTarget;
            }
            
            await target.TakeDamage(0.2f * s.currentStats.atq);
            foreach (var buff in target.statusEffects.Where(e => e.type == EffectType.Buff && ((Buff) e)._modifier > 0)) {
                await ((Buff) buff).RemoveEffect();
            }
        }
    }
}