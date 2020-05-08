using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bard;
using Characters;
using Combat.Characters;
using Status;
using UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum CombatPhase {
    SelectMelodies,
    RhythmGame,
    ExecMelodies,
    TurnFight,
    ResolveFight
}

public class CombatManager : MonoBehaviour {
    public BardControl bard; //TODO init bard in GameManager
    public List<List<CharacterControl>> teams = new List<List<CharacterControl>>(2);
    public GameObject playerTeamGo, enemyTeamGo;
    public RectTransform playerTeamUiContainer;
    public RectTransform enemyTeamUiContainer;
    public ReactiveProperty<CombatPhase> combatPhase = new ReactiveProperty<CombatPhase>(CombatPhase.SelectMelodies);
    public DialogManager dialogManager;
    
    private List<CharacterBase> clientsMenu;
    private List<CharacterBase> currentMonsters;
    

    [Header("Combat Testing Only")]
    [SerializeField] public bool ignoreGameManager = false;
    [SerializeField] private CharacterBase[] forcedClients = new CharacterBase[3];
    [SerializeField] private CharacterBase[] forcedMonsters = new CharacterBase[3];
    
    public async Task ExecTurn() {
        combatPhase.Value = CombatPhase.RhythmGame;
        bard.InitRhythmGame();
        await bard.StartRhythmGame();
        await Utils.AwaitObservable(Observable.Timer(TimeSpan.FromSeconds(1)));
        combatPhase.Value = CombatPhase.ExecMelodies;
        await bard.ExecMelodies();
        bard.Reset();
        combatPhase.Value = CombatPhase.TurnFight;
        await ResolveTurnFight();
        //combatPhase.Value = CombatPhase.ResolveFight;
        //await ResolveFight();

        combatPhase.Value = CombatPhase.SelectMelodies;
    }

    private async Task ResolveTurnFight() {
        //sort characters
        List<CharacterControl>[] charactersGroups = {
            new List<CharacterControl>(),
            new List<CharacterControl>(),
            new List<CharacterControl>(),
            new List<CharacterControl>()
        };
        foreach (var c in teams.SelectMany(t => t)) {
            if (c.HasEffect(EffectType.Rushing)) {
                charactersGroups[0].Add(c);
            } else if (c.influenced) {
                charactersGroups[1].Add(c);
            } else if (c.HasEffect(EffectType.Lagging)) {
                charactersGroups[3].Add(c);
            } else {
                charactersGroups[2].Add(c);
            }
        }
        List<CharacterControl> characters = charactersGroups.SelectMany(t => t).ToList();
        
        // Apply status effects to all characters
        foreach (var character in characters) {
            GameObject l = character.transform.Find("HighLight").gameObject;
            l.SetActive(true);

            await character.EffectsTurnStart();

            l.SetActive(false);
        }

        
        //TODO check if fight is not over between each skill
        // Execute all character skills
        foreach (CharacterControl character in characters) {
            if (character == null) continue;
            
            GameObject l = character.transform.Find("HighLight").gameObject;
            l.SetActive(true);

            await character.ExecTurn();

            l.SetActive(false);
        }
    }

    public void Remove(CharacterControl characterControl) {
        int charTeam = (int) characterControl.team;
        if(charTeam == 0)
            GameManager.quest.FightMax[characterControl.clientNumber] = GameManager.CurrentFight + 1; //decreasing the number of total fight

        Destroy(characterControl.gameObject);
        teams[charTeam].Remove(characterControl);

        if (teams[charTeam].Count == 0)
        {
            GetComponent<GameManager>().ChangeCombat();
            if (!GameManager.QuestState)
            {
                GameManager.CurrentInspiration = bard.inspiration.current.Value;    //save the current inspiration for the next fight
                GetComponent<GameManager>().ChangeScene(3);
    // develop-nico
    //        }
    //        else
    //        {
    //            int gold = GetComponent<GameManager>().CalculateGold();
    //            GameManager.CurrentInspiration = 0;                                 //reset inspiration
    //            GetComponent<GameManager>().ChangeScene(0);
    //        }
    //    }
    //}

    //private async Task ResolveFight()
    //{
    //    if (teams[1].Count == 0)        //no more enemies
    //    {
    //        GetComponent<GameManager>().ChangeCombat();
    //        if (!GameManager.QuestState)
    //        {
    //            GameManager.CurrentInspiration = bard.inspiration.current.Value;    //save the current inspiration for the next fight
    //            GetComponent<GameManager>().ChangeScene(2);
    //        }
    //        else
    //        {
    //            int gold = GetComponent<GameManager>().CalculateGold();
    //            GameManager.CurrentInspiration = 0;                                 //reset inspiration

            } else {
                /*int gold = */GetComponent<GameManager>().CalculateGold();
                GetComponent<GameManager>().ChangeScene(1);
            }
        }

        if (teams[0].Count == 0)        //no more clients
        {
            Debug.Log("All my friends are dead, push me to the edge");
            GetComponent<GameManager>().ChangeScene(1);
        }
    }

    #region Unity

    public static CombatManager Instance { get; private set; }

    public void Awake() {
        Instance = this;
    }

    private async void Start() {

        clientsMenu = new List<CharacterBase>();
        currentMonsters = new List<CharacterBase>();
        if (ignoreGameManager) {
            //init characters based on editor (without gameManager)
            foreach (var client in  forcedClients)
                clientsMenu.Add(client);
        
            foreach (var monster in forcedMonsters)
                currentMonsters.Add(monster);
        } else {
            //init characters based on gameManager (loaded from the Expedition)
            foreach (var client in  GameManager.clients)
                clientsMenu.Add(client);
        
            foreach (var monster in GameManager.quest.fightManager.fights[GameManager.CurrentFight].monsters)
                currentMonsters.Add(monster);
        }

        teams = new List<List<CharacterControl>> {new List<CharacterControl>(), new List<CharacterControl>()};

        //Task[] setupTasks = new Task[6];
        int i = 0;
        foreach (Transform t in playerTeamGo.transform) {
            /*setupTasks[i] = */await SetupCharacterControl(t, clientsMenu, i, Team.PlayerTeam);
            ++i;
        }

        i = 0;
        foreach (Transform t in enemyTeamGo.transform) {
            /*setupTasks[i+3] = */await SetupCharacterControl(t, currentMonsters, i, Team.EnemyTeam);
            ++i;
        }

        //Task.WaitAll(setupTasks);

        //dialogs
        dialogManager = GetComponent<DialogManager>();
        List<CharacterType> characterTypes =
            teams[(int) Team.EnemyTeam].Select(Dialog.GetCharacterTypeFromCharacterControl).Distinct().ToList();
        characterTypes.Add(CharacterType.Client);
        characterTypes.Add(CharacterType.Bard);
        await dialogManager.Init(characterTypes);
        
        await dialogManager.ShowDialog(DialogFilter.CombatStart, CharacterType.Bard,
            CharacterType.None);
        await dialogManager.ShowDialog(DialogFilter.CombatStart, CharacterType.Client,
            CharacterType.None);
        await dialogManager.ShowDialog(DialogFilter.CombatStart,
            Dialog.GetCharacterTypeFromCharacterControl(teams[(int) Team.EnemyTeam][Random.Range(0, 3)]),
            CharacterType.None);

    }

    private async Task SetupCharacterControl(Transform characterTransform, IReadOnlyList<CharacterBase> team, int i, Team charTeam) {
        string charPrefabName = charTeam == Team.PlayerTeam ? "PlayerTeamCharacterPrefab" : "EnemyCharacterPrefab";
        string charPrefabUiName = charTeam == Team.PlayerTeam ? "PlayerTeamCharacterUI" : "EnemyCharacterUI";

        // instantiate the character prefab
        var characterGameObject = Instantiate(await Utils.LoadResource<GameObject>(charPrefabName), characterTransform);

        // Load the character 3d model
        var model = Instantiate(await Utils.LoadResource<GameObject>(team[i].Character.modelName), characterGameObject.transform);
        model.AddComponent<Animator>().runtimeAnimatorController = await Utils.LoadResource<RuntimeAnimatorController>("Animations/Character");

        // Init the character control
        CharacterControl character = characterGameObject.GetComponent<CharacterControl>();
        character.team = charTeam;
        if (charTeam == Team.PlayerTeam)
            character.clientNumber = i;
        await character.Init(team[i].Character, team[i].SkillWheel);
        teams[(int) charTeam].Add(character);

        // instantiate the UI on the canvas
        var charUi = characterGameObject.transform.Find(charPrefabUiName);
        charUi.SetParent(charTeam == Team.PlayerTeam ? playerTeamUiContainer : enemyTeamUiContainer);
        charUi.localScale = Vector3.one;
        charUi.localEulerAngles = Vector3.zero;
        charUi.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = character.currentStats.prot.Value.ToString() + " %";
        charUi.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = team[i].Name;
        if (charTeam == Team.PlayerTeam)
            charUi.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = character.characterData.clientImage;
        characterGameObject.GetComponent<CharacterVfx>().Init(character);
        SlotUi slotUi = charUi.GetComponentInChildren<SlotUi>();
        slotUi.Init(character);
    }

    #endregion
}
