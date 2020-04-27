using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies
{
    [CreateAssetMenu(fileName = "DaCapo", menuName = "Melody/DaCapo")]
    class DaCapo : Melody
    {
        protected override async Task ExecuteOnTarget(CharacterControl t)
        {
            await StatusEffect.ApplyEffect(t, EffectType.Grace, 1);
            await StatusEffect.ApplyEffect(t, EffectType.Disciplined, 2);
        }
    }
}