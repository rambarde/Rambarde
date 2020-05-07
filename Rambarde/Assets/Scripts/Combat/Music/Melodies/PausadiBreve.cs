using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies
{
    [CreateAssetMenu(fileName = "PausaDiBreve", menuName = "Melody/PausaDiBreve")]
    class PausadiBreve : Melody
    {
        protected override async Task ExecuteOnTarget(CharacterControl t)
        {
            await CombatManager.Instance.dialogManager.ShowDialog(DialogFilter.Heal,
                CharacterType.Bard, CharacterType.None);
                            
            await t.Heal(40);
                            
            await CombatManager.Instance.dialogManager.ShowDialog(DialogFilter.Heal,
                CharacterType.None, Dialog.GetCharacterTypeFromCharacterControl(t));
        }
    }
}