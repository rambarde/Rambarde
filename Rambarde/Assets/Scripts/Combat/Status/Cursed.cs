using System.Threading.Tasks;
using Characters;

namespace Status {
    class Cursed : StatusEffect {
        public Cursed(CharacterControl target, int turns) : base(target, turns) {
            type = EffectType.Cursed;
            spriteName = "Statut_Maudit";
        }
        
        protected override async Task Remove() {
            await target.TakeDamage(target.currentStats.hp.Value);
        }
    }
}