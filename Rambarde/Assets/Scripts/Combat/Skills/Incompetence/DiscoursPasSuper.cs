using System.Collections.Generic;
using System.Linq;
using Characters;
using System.Threading.Tasks;
using Status;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "DiscoursPasSuper", menuName = "Skill/Incompetence/DiscoursPasSuper")]
    public class DiscoursPasSuper : Skill {
        
        public new bool IsIncompetence => true;

        public new async Task Execute(CharacterControl s) {
            foreach (var c in CombatManager.Instance.teams[(int)s.team]) {
                await StatusEffect.ApplyBuff(c, BuffType.Protection, -2);
            }
        }
    }
}