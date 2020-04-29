using System;
using System.Collections.Generic;
using System.Linq;
using Bard;
using Characters;
using DG.Tweening;
using Melodies;
using Status;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {
    public List<RectTransform> instPanels;
    public RectTransform actionPanel;
    public Image inspiJauge;

    public GameObject melodyMenu;
    public GameObject musicMenu;
    public GameObject pauseMenu;

    private static void AddSubscription(List<IDisposable> subscriptions, BardControl bard, CharacterControl character, Melody melody) {
        subscriptions.Add(
            character.gameObject.OnMouseDownAsObservable()
                .Subscribe(__ => {
                    bard.SelectMelody(melody, character);
                    subscriptions.ForEach(s => s.Dispose());
                    
                    foreach (CharacterControl chara
                        in CombatManager.Instance.teams.SelectMany(team => team)) {

                        chara.gameObject.transform.Find("HighLight").gameObject.SetActive(false);
                    }
                }));
    }
    
    public async void Init(BardControl bard) {

        CombatManager.Instance.combatPhase.Subscribe(phase => {
            switch (phase) {
                case CombatPhase.SelectMelodies :
                case CombatPhase.TurnFight :
                    musicMenu.SetActive(false);
                    melodyMenu.SetActive(true);
                    break;
                case CombatPhase.RhythmGame :
                    musicMenu.SetActive(true);
                    melodyMenu.SetActive(false);
                    break;
            }
        });
        
        GameObject t1ButtonPrefab = await Utils.LoadResource<GameObject>("MelodyButtonT1");
        GameObject t2ButtonPrefab = await Utils.LoadResource<GameObject>("MelodyButtonT2");
        GameObject t3ButtonPrefab = await Utils.LoadResource<GameObject>("MelodyButtonT3");
        GameObject separatorPrefab = await Utils.LoadResource<GameObject>("Separator");

        for (int i = 0; i < bard.instruments.Count; ++i) {
            
            RectTransform panel = instPanels[i];
            Instrument inst = bard.instruments[i];
            
            int currentTier = 1;
            foreach (var melody in inst.melodies) {
                if (melody.tier > currentTier) {
                    Instantiate(separatorPrefab, panel);
                    currentTier = melody.tier;
                }

                GameObject buttonGo = null;
                
                switch (currentTier)
                {
                    case 1:
                        buttonGo = Instantiate(t1ButtonPrefab, panel);
                        break;
                    case 2:
                        buttonGo = Instantiate(t2ButtonPrefab, panel);
                        break;
                    case 3:
                        buttonGo = Instantiate(t3ButtonPrefab, panel);
                        break;
                }

                if (buttonGo != null)
                {
                    Button button = buttonGo.GetComponent<Button>();
                    Image image = buttonGo.transform.Find("Image").GetComponent<Image>();
                    TextMeshProUGUI label = buttonGo.GetComponentInChildren<TextMeshProUGUI>();
                    label.text = Utils.SplitPascalCase(melody.name);
                    image.sprite = melody.sprite;
                    button.OnClickAsObservable()
                        .Subscribe(_ => {
                            List<IDisposable> subscriptions = new List<IDisposable>();
                            switch (melody.targetMode) {
                                case MelodyTargetMode.EveryAlly :
                                case MelodyTargetMode.EveryEnemy :
                                case MelodyTargetMode.Everyone :
                                    bard.SelectMelody(melody);
                                    break;
                            
                                case MelodyTargetMode.OneAlly :
                                    foreach (CharacterControl character
                                        in CombatManager.Instance.teams[0].Where(c => ! c.HasEffect(EffectType.Deaf))) {
                                        character.gameObject.transform.Find("HighLight").gameObject.SetActive(true);
                                        AddSubscription(subscriptions, bard, character, melody);
                                    }
                                    break;
                            
                                case MelodyTargetMode.OneEnemy :
                                    foreach (CharacterControl character
                                        in CombatManager.Instance.teams[1].Where(c => ! c.HasEffect(EffectType.Deaf))) {
                                        character.gameObject.transform.Find("HighLight").gameObject.SetActive(true);
                                        AddSubscription(subscriptions, bard, character, melody);
                                    }
                                    break;
                            
                                case MelodyTargetMode.Anyone :
                                    foreach (CharacterControl character
                                        in CombatManager.Instance.teams.SelectMany(team => team)
                                            .Where(c => ! c.HasEffect(EffectType.Deaf))) {
                                    
                                        character.gameObject.transform.Find("HighLight").gameObject.SetActive(true);
                                        AddSubscription(subscriptions, bard, character, melody);
                                    }
                                    break;
                            
                                default:
                                    Debug.Log("Warning : Melody ["+ melody.name+"] " +
                                              "has no known targetMode ("+ melody.targetMode +")");
                                    break;
                            }
                        })
                        .AddTo(button);

                    melody.isPlayable.Subscribe(playable =>
                    {
                        button.interactable = playable;
                        SpriteRenderer spriteRenderer = button.GetComponent<SpriteRenderer>();
                        if (playable)
                        {
                            
                        }
                    }).AddTo(button);
                
                    button.OnPointerEnterAsObservable()
                        .Subscribe(_ => { })
                        .AddTo(button);
                }
            }

            bard.actionPoints.Subscribe(p => {
                int nbrSquare = (bard.maxActionPoints.Value - p) / 8; //- - - - - WARNING MAGIC NUMBER NBR NOTE PER BEAT
                int colored = 0;
                List<Image> actionImages = actionPanel.GetComponentsInChildren<Image>().ToList();
                actionImages.RemoveAt(0);
                actionImages.Reverse();
                foreach (Image image in actionImages) {
                    image.color = colored >= nbrSquare ? Color.white : Color.red;
                    colored++; 
                }
            });

            bard.inspiration.current.Subscribe(inspi =>
            {
                inspiJauge.DOFillAmount(inspi / (float) bard.inspiration.maximumValue, .5f);
            });
        }
    }
    
    public void Pause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
}