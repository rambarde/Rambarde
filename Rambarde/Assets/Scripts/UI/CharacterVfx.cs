using System.Globalization;
using Characters;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class CharacterVfx : MonoBehaviour {
        private CharacterControl _characterControl;
        public Image greenBar;
        public GameObject statusEffects;
        public TextMeshProUGUI characterProt;

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