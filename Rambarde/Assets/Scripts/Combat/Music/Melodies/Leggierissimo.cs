using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies
{
    [CreateAssetMenu(fileName = "Leggierissimo", menuName = "Melody/Leggierissimo")]
    class Leggierissimo : Melody
    {
        protected override async Task ExecuteOnTarget(CharacterControl t)
        {
            await CombatManager.Instance.dialogManager.ShowDialog(DialogFilter.CombatStart, CharacterType.Bard,
                CharacterType.None, CombatManager.Instance.bard.instruments[1].sprite, "Theodore");
                            
            await t.Heal(10);
                            
            await CombatManager.Instance.dialogManager.ShowDialog(DialogFilter.Heal,
                CharacterType.None, Dialog.GetCharacterTypeFromCharacterControl(t),
                t.characterData.clientImage, t.characterName);
            // Heal Poison, Confused, Destabilised
            Debug.Log("Annule tous les changements de stats de: " + t.name);
        }
    }
}