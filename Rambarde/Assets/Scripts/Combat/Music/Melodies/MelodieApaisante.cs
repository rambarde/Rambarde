using System.Linq;
using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies {
    [CreateAssetMenu(fileName = "MelodieApaisante", menuName = "Melody/MelodieApaisante")]
    class MelodieApaisante : Melody {
        protected override async Task ExecuteOnTarget(CharacterControl t) {
            if (t.HasEffect(EffectType.Poison)) {
                await t.statusEffects.First(e => e.type == EffectType.Poison).RemoveEffect();
            }
            if (t.HasEffect(EffectType.Confused)) {
                await t.statusEffects.First(e => e.type == EffectType.Confused).RemoveEffect();
            }
            if (t.HasEffect(EffectType.Destabilized)) {
                await t.statusEffects.First(e => e.type == EffectType.Destabilized).RemoveEffect();
            }
        }
    }
}