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
            await t.Heal(10);
            // Heal Poison, Confused, Destabilised
            Debug.Log("Annule tous les changements de stats de: " + t.name);
        }
    }
}