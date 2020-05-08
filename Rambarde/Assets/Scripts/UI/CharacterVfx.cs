using System.Globalization;
using Characters;
using DG.Tweening;
using Status;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class CharacterVfx : MonoBehaviour {
        private CharacterControl _characterControl;
        public Image greenBar;
        public GameObject statusEffects;
        public TextMeshProUGUI characterProt;
        [SerializeField] private CanvasGroup effectTooltip;
        
        private const float LerpTime = 1f;
        private const string ResourcesDir = "CharacterVfx";


        public async void Init(CharacterControl characterControl)
        {
            _characterControl = characterControl;
            
            if (greenBar) {
                _characterControl.currentStats.hp.AsObservable().Subscribe(x =>
                    greenBar.DOFillAmount(x/_characterControl.currentStats.maxHp,LerpTime)
                ).AddTo(this);
            }

            if (characterProt)
            {
                _characterControl.currentStats.prot.AsObservable().Subscribe(x =>
                    characterProt.text = x.ToString(CultureInfo.InvariantCulture) + "%"
                ).AddTo(this);
            }
            
            TextMeshProUGUI descText = null;
            TextMeshProUGUI propsText = null;
            Image imageIcon = null;
            if (effectTooltip != null)
            {
                descText = effectTooltip.transform.Find("Desc").GetComponent<TextMeshProUGUI>();
                propsText = effectTooltip.transform.Find("Props").GetComponent<TextMeshProUGUI>();
                imageIcon = effectTooltip.transform.Find("Icon").GetComponent<Image>();

                RectTransform rt = effectTooltip.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(300, 200);
            }

            if (statusEffects) {
                _characterControl.statusEffects.ObserveAdd().Subscribe(async x => {
                    //TODO: Add animation for added effect
                    var added = x.Value;

                    var go = Instantiate(await Utils.LoadResource<GameObject>("StatusEffectIcon"), statusEffects.transform);
                    var image = go.transform.Find("Image").gameObject.GetComponent<Image>();
                    var text = go.transform.Find("TurnsLeft").gameObject.GetComponent<TextMeshProUGUI>();

                    image.sprite = await Utils.LoadResource<Sprite>(added.spriteName);
                    added.turnsLeft.AsObservable().Subscribe(turns => {
                        //TODO: Add animation for text change 
                        text.text = turns.ToString();
                        if (turns == 0) {
                            Destroy(go);
                        }
                    }).AddTo(go);
                    
                    image.GetComponent<Image>().OnPointerEnterAsObservable()
                        .Subscribe(_ =>
                        {
                            //update tooltip ui
                            
                            imageIcon.sprite = image.sprite;
                            if (added.type != EffectType.Buff) {
                                descText.text = StatusEffect.GetEffectDescription(added.type);
                                propsText.text = StatusEffect.GetEffectName(added.type)
                                                 + "\n" + added.turnsLeft.Value + " tour" + (added.turnsLeft.Value > 1 ? "s" : "");
                            } else {
                                switch (((Buff) added)._buffType) {
                                    case BuffType.Attack :
                                        descText.text = "\nModifie l'Attaque";
                                        propsText.text = "Buff d'Attaque"
                                                         + "\nNiv. " + added.turnsLeft.Value;
                                        break;
                                    case BuffType.Protection :
                                        descText.text = "\nModifie la Protection";
                                        propsText.text = "Buff d'Protection"
                                                         + "\nNiveau " + added.turnsLeft.Value;
                                        break;
                                    case BuffType.Critical :
                                        descText.text = "\nModifie le Critique";
                                        propsText.text = "Buff d'Critique"
                                                         + "\nNiveau " + added.turnsLeft.Value;
                                        break;
                                    
                                }
                                
                            }
                            

                            effectTooltip.DOFade(1, .5f);
                        });
                    image.OnPointerExitAsObservable()
                        .Subscribe(_ =>
                        {
                            // hide tooltip ui 
                            effectTooltip.DOFade(0, .5f);
                        });
                    
                }).AddTo(this);
            }
        }

        // private static void LerpHealthBar(Pair<float> pair, GameObject go, ref float curLerpTime, float speed, float lerpTime, ref float t) {
        //     float f = Mathf.Lerp(pair.Previous, pair.Current, t / LerpTime);
        //     go.GetComponent<Image>().fillAmount = f / 100f;
        //     curLerpTime += speed * Time.deltaTime;
        //     t = curLerpTime / LerpTime;
        //     t = Mathf.Sin(t * Mathf.PI * 0.5f);
        // }

        // private void UpdateBar(Pair<float> values, GameObject bar, int speed, Action<Pair<float>> update) {
        //     float t = 0f, currentLerpTime = 0f;
        //     bar.LateUpdateAsObservable()
        //         .TakeWhile(_ => currentLerpTime < LerpTime + speed * Time.deltaTime)
        //         .DoOnCompleted(() => update(values))
        //         .Subscribe(_ => {
        //             float f = Mathf.Lerp(values.Previous, values.Current, t / LerpTime);
        //             bar.GetComponent<Image>().fillAmount = f / 100f;
        //             currentLerpTime += speed * Time.deltaTime;
        //             t = currentLerpTime / LerpTime;
        //             t = Mathf.Sin(t * Mathf.PI * 0.5f);
        //         });
        // }
    }
}