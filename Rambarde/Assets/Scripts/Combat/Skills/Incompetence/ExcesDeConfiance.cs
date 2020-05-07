using System.Linq;
using Characters;
using System.Threading.Tasks;
using Status;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "ExcesDeConfiance", menuName = "Skill/Incompetence/ExcesDeConfiance")]
    public class ExcesDeConfiance : Skill {
        
        public new bool IsIncompetence => true;

        public new async Task Execute(CharacterControl s) {
            CharacterControl target = RandomTargetInTeam(s.team );
            if (hasForcedTarget) {
                target = forcedTarget;
            }

            await target.TakeDamage(150f / 100f * s.currentStats.atq);
        }
    }
}
