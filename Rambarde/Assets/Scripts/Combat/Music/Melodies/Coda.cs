﻿using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies
{
    [CreateAssetMenu(fileName = "Coda", menuName = "Melody/Coda")]
    class Coda : Melody
    {
        protected override async Task ExecuteOnTarget(CharacterControl t)
        {
            await StatusEffect.ApplyEffect(t, EffectType.Merciless, 1);
            await StatusEffect.ApplyEffect(t, EffectType.Rushing, 1);
        }
    }
}