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
            await CombatManager.Instance.dialogManager.ShowDialog(DialogFilter.Heal,
                CharacterType.Bard, CharacterType.None);
                            
            await t.Heal(10);
                            
            await CombatManager.Instance.dialogManager.ShowDialog(DialogFilter.Heal,
                CharacterType.None, Dialog.GetCharacterTypeFromCharacterControl(t));
            await t.statusEffects.First(e => e.type == EffectType.Poison).RemoveEffect();
            await t.statusEffects.First(e => e.type == EffectType.Confused).RemoveEffect();
            await t.statusEffects.First(e => e.type == EffectType.Destabilized).RemoveEffect();
        }
    }
}