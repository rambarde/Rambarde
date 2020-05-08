using System.Linq;
using Characters;
using System.Threading.Tasks;
using Status;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "Degonfle", menuName = "Skill/Incompetence/Degonfle")]
    public class Degonfle : Skill {
        
        public new bool IsIncompetence => true;

        public new async Task Execute(CharacterControl s) {
            CharacterControl target = RandomTargetInTeam(s.team );
            if (hasForcedTarget) {
                target = forcedTarget;
            }

            await StatusEffect.ApplyBuff(s, BuffType.Protection, -3);
            await StatusEffect.ApplyEffect(target, EffectType.Marked, 2);
        }
    }
}