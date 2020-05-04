using System.Threading.Tasks;
using Characters;
using Melodies;
using Status;
using UnityEngine;

namespace Music.Melodies {
    [CreateAssetMenu(fileName = "Eloge", menuName = "Melody/Eloge")]
    class Eloge : Melody {
        protected override async Task ExecuteOnTarget(CharacterControl t) {
            t.ForceTarget(t);
        }
    }
}