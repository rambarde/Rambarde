using System.Linq;
using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies {
    [CreateAssetMenu(fileName = "AriaApaisante", menuName = "Melody/AriaApaisante")]
    class AriaApaisante : Melody {
        protected override async Task ExecuteOnTarget(CharacterControl t)
        {
            await t.Heal(10);
            await t.statusEffects.First(e => e.type == EffectType.Poison).RemoveEffect();
            await t.statusEffects.First(e => e.type == EffectType.Confused).RemoveEffect();
            await t.statusEffects.First(e => e.type == EffectType.Destabilized).RemoveEffect();
        }
    }
}