using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skills;
using Status;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = System.Random;

namespace Characters {
    public enum Team {
        PlayerTeam = 0,
        EmemyTeam = 1
    }

    public class CharacterControl : MonoBehaviour {
        public string characterName;
        public Team team;
        public int clientNumber;
        public Skill[] skillWheel;
        public Stats currentStats;
        public Equipment[] equipment;
        public CharacterData characterData;
        public ReactiveCollection<StatusEffect> statusEffects;
        public ReactiveProperty<EffectType> effectTypes;
        
        public List<Skill> skillSlot;
        public Subject<SlotAction> slotAction;
        public bool influenced;
        public bool skillWheelShouldTick;
        private CharacterControl _forcedTarget;
        private bool _hasForcedTarget = false;
            
        private static Random rng = new Random();
        private int _skillIndex;
        
        private Animator _animator;
        private CombatManager _combatManager;

        private IObservable<int> _animationSkillStateObservable;

        #region Turn

        public IEnumerable<CharacterControl> GetTeam() {
            return _combatManager.teams[(int) team];
        }

        public async Task EffectsTurnStart() {
            for (var i = statusEffects.Count - 1; i >= 0; --i) {
                var effect = statusEffects[i];
                await effect.TurnStart();
            }
        }

        public async Task ExecTurn() {

            if (HasEffect(EffectType.Confused)) {
                int r = UnityEngine.Random.Range(0, skillWheel.Length);
                for (int i = 0; i <= r; ++i)
                    await IncrementSkillsSlot();
            }

            Skill skill = skillWheel[_skillIndex];

            if (HasEffect(EffectType.Dizzy)) {
                skillWheelShouldTick = false;
            } else {
                //force target
                if (_hasForcedTarget) {
                    skill.ForceTarget(_forcedTarget);
                    _hasForcedTarget = false;
                    _forcedTarget = null;
                }
                
                //calculate chances of miss and critical hit, and merciless effect

                // Play and wait for skillAnimation to finish
                await SkillPreHitAnimation(skill.animationName);
                // Execute the skill
                await skill.Execute(this);

                if (HasEffect(EffectType.Exalted)) {
                    await IncrementSkillsSlot();
                    await SkillPreHitAnimation(skill.animationName);
                    await skill.Execute(this);
                }
            }

            // Apply effects at the end of the turn
            for (var i = statusEffects.Count - 1; i >= 0; --i)
                await statusEffects[i].TurnEnd();

            // Increment skillIndex ONLY if effects did not affect the wheel
            if (skillWheelShouldTick)
                await IncrementSkillsSlot();

            skillWheelShouldTick = true;
        }

        private async Task SkillPreHitAnimation(string animationName) {
            _animator.SetTrigger(animationName);
            var stateHash = Animator.StringToHash(animationName);

            await Utils.AwaitObservable(
                this.UpdateAsObservable()
                    .SkipWhile(_ => !_animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(stateHash))
                    .TakeWhile(_ => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(stateHash))
            );
        }

        #endregion

        #region CharacterData

        public async Task Init(CharacterData data, int[] intSkillWheel)
        {
            characterData = data;
            characterData.Init();
            //characterData.armors.ObserveCountChanged().Subscribe(_ => UpdateStats(0)).AddTo(this);
            //characterData.weapons.ObserveCountChanged().Subscribe(_ => UpdateStats(1)).AddTo(this);

            Skill[] temp = new Skill[4];

            for (int j = 0; j < intSkillWheel.Length; j++)
                temp[j] = characterData.skills[intSkillWheel[j]];
            skillWheel = temp;

            UpdateStats();
            currentStats.Init();

            slotAction = new Subject<SlotAction>();
            statusEffects = new ReactiveCollection<StatusEffect>();
            effectTypes = new ReactiveProperty<EffectType>(EffectType.None);
            skillSlot = skillWheel.ToList();

            //animator
            _combatManager = CombatManager.Instance;
            _animator = GetComponentInChildren<Animator>();
            AnimatorOverrideController myOverrideController = await Utils.LoadResource<AnimatorOverrideController>("Animations/" + characterData.animatorController);
            _animator.runtimeAnimatorController = myOverrideController;
        }

        //private void UpdateStats(int accessory) {
        public void UpdateStats() 
        {
            if (equipment != null && equipment.Length >= 2 && equipment[0] != null && equipment[1] != null) {
                currentStats.atq = characterData.baseStats.atq + equipment[0].atqMod + equipment[1].atqMod;
                currentStats.prec = characterData.baseStats.prec * (equipment[0].precMod + equipment[1].precMod + 1);
                currentStats.crit = characterData.baseStats.crit * (equipment[0].critMod + equipment[1].critMod + 1);
                currentStats.maxHp = characterData.baseStats.maxHp + equipment[0].endMod + equipment[1].endMod;
                currentStats.prot.Value = characterData.baseStats.prot.Value * (equipment[0].protMod + equipment[1].protMod + 1);
            } else {
                currentStats.atq = characterData.baseStats.atq;
                currentStats.prec = characterData.baseStats.prec;
                currentStats.crit = characterData.baseStats.crit;
                currentStats.maxHp = characterData.baseStats.maxHp;
                currentStats.prot = characterData.baseStats.prot;
            }
        }

        public async Task Heal(float pts) {
            var newHealth = currentStats.hp.Value + pts;
            if (newHealth < currentStats.maxHp)
                currentStats.hp.Value = newHealth;

            await Utils.AwaitObservable(Observable.Timer(TimeSpan.FromSeconds(1)));
        }

        private float CalculateDamage(float dmg) {
            var curEnd = currentStats.hp.Value;
            curEnd -= dmg * (1 - currentStats.prot.Value / 100f);
            if (curEnd < 0) {
                curEnd = 0;
            }

            return Mathf.Ceil(curEnd);
        }

        public async Task TakeDamage(float dmg) {
            currentStats.hp.Value = CalculateDamage(dmg);
            if (currentStats.hp.Value > 0) return;
            
            //TODO: spawn floating text for damage taken
            await Utils.AwaitObservable(Observable.Timer(TimeSpan.FromSeconds(0.7f)));
            _combatManager.Remove(this);
        }

        #endregion

        public void ForceTarget(CharacterControl target) {
            _hasForcedTarget = true;
            _forcedTarget = target;
        }

        public async Task IncrementSkillsSlot() {
            _skillIndex = (_skillIndex + 1) % skillSlot.Count;
            skillWheelShouldTick = false;
            
            //wait for skill wheel animation finish
            slotAction.OnNext(new SlotAction { Action = SlotAction.ActionType.Increment});
            await Utils.AwaitObservable(slotAction
                .SkipWhile(action => action.Action != SlotAction.ActionType.Sync)
                .Take(1));
        }

        public async Task DecrementSkillsSlot()
        {
            _skillIndex = (_skillIndex - 1) % skillSlot.Count;
            if (_skillIndex < 0) _skillIndex = skillSlot.Count - 1;
            skillWheelShouldTick = false;
            
            //wait for skill wheel animation finish
            slotAction.OnNext(new SlotAction { Action = SlotAction.ActionType.Decrement});
            await Utils.AwaitObservable(slotAction
                .SkipWhile(action => action.Action != SlotAction.ActionType.Sync)
                .Take(1));
        }

        public void ShuffleList<T>(IList<T> list)
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }

        public async Task ShuffleSkillsSlot()
        {
            ShuffleList(skillSlot);
            skillWheelShouldTick = false;
            
            //wait for skill wheel animation finish
            slotAction.OnNext(
                new SlotAction
                {
                    Action = SlotAction.ActionType.Shuffle,
                    Skills = skillSlot
                });
            await Utils.AwaitObservable(slotAction
                .SkipWhile(action => action.Action != SlotAction.ActionType.Sync)
                .Take(1));
        }

        public bool HasEffect(EffectType effect) => effectTypes.Value.HasFlag(effect);
        
    }
}