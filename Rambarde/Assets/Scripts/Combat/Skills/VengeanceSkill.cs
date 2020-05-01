using Characters;
using System.Threading.Tasks;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "VengeanceSkill", menuName = "Skill/VengeanceSkill")]
    public class VengeanceSkill : Skill {

        public new async Task Execute(CharacterControl s) {
            CharacterControl target = RandomTargetInTeam(s.team + 1);
            if (hasForcedTarget) {
                target = forcedTarget;
            }

            float x = (s.characterData.baseStats.maxHp - s.characterData.baseStats.hp.Value) * 2f;
            await target.TakeDamage(50 + x / 100f * s.currentStats.atq);
        }
    }
}