using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies {
    [CreateAssetMenu(fileName = "RepetitionGenerale", menuName = "Melody/RepetitionGenerale")]
    class RepetitionGenerale : Melody {
        protected override async Task ExecuteOnTarget(CharacterControl t)
        {
            t.skillWheelShouldTick = false;
        }
    }
}