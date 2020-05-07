using System.Linq;
using Characters;
using System.Threading.Tasks;
using Status;
using UnityEngine;

namespace Skills {
    [CreateAssetMenu(fileName = "ExplosionPrecoce", menuName = "Skill/Incompetence/ExplosionPrecoce")]
    public class ExplosionPrecoce : Skill {
        
        public new bool IsIncompetence => true;

        public new async Task Execute(CharacterControl s) {
            foreach (var c in CombatManager.Instance.teams[(int)s.team]) {
                await c.TakeDamage(80f / 100f * s.currentStats.atq);
            }
        }
    }
} 