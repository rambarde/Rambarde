using System.Linq;
using Characters;
using System.Threading.Tasks;
using Status;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "AssassinEgoiste", menuName = "Skill/Incompetence/AssassinEgoiste")]
    public class AssassinEgoiste : Skill {
        
        public new bool IsIncompetence => true;

        public new async Task Execute(CharacterControl s) {

            await StatusEffect.ApplyEffect(s, EffectType.Confused, 3);
            await StatusEffect.ApplyEffect(s, EffectType.Unpredictable, 3);
        }
    }
}
