using System.Threading.Tasks;
using Characters;
using UnityEngine;

namespace Melodies {
     [CreateAssetMenu(fileName = "PrestoMelody", menuName = "Melody/Presto")]
     public class PrestoMelody : Melody {
         protected override async Task ExecuteOnTarget(CharacterControl t) {
             if (t == null) {
                 Debug.Log("Tried to execute a " + targetMode + " melody with no target");
                 return;
             }

             await t.IncrementSkillsSlot();
         }
     }
 }