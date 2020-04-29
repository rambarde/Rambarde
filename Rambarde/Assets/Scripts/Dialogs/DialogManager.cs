using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Skills;
using Status;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public struct Quote
{
    public CharacterType character;
    public string phrase;
}

public class DialogManager : MonoBehaviour
{
    public float dialogAppearRate = .4f;
    public int textSpeed = 200;
    public CanvasGroup dialogCanvas;
    public TextMeshProUGUI dialogText;
    public Image dialogImage;

    private Dictionary<CharacterType, List<DialogPhrase>> dialogs;
    private List<string> closedList;

    public async Task Init(List<CharacterType> characters)
    {
        dialogs = new Dictionary<CharacterType, List<DialogPhrase>>();
        closedList = new List<string>();
        foreach (var character in characters)
        {
            Dialog dialog = await Utils.LoadResource<Dialog>("ScriptableObjects/Dialogs/" + character);
            dialogs.Add(character, dialog.GetPhrases());
        }
    }
    
    bool IsDialogAvailable()
    {
        return Random.Range(0.0f, 1f) <= dialogAppearRate;
    }
    
    private Quote GetDialogQuote(DialogFilter filter, CharacterType actionExecutor, CharacterType actionReceiver)
    {
        List<Quote> quotes = new List<Quote>();

        // get executors dialog
        switch (actionExecutor)
        {
            //bard///////////////////////////////////////////////
            case CharacterType.Bard:
                //unbuff filter
                if (filter.HasFlag(DialogFilter.Unbuff))
                {
                    switch (actionReceiver)
                    {
                        //fake monsters
                        case CharacterType.Goblins:
                        case CharacterType.Orcs:
                        case CharacterType.OrcsLeader:
                        case CharacterType.Treant:
                        case CharacterType.Golem:
                            filter |= DialogFilter.FakeMonsters;
                            break;
                        //real monsters
                        case CharacterType.Ghost:
                        case CharacterType.Wight:
                        case CharacterType.Wisp:
                        case CharacterType.Brasier:
                        case CharacterType.Skeleton:
                        case CharacterType.AngrySkeleton:
                        case CharacterType.ColdSkeleton:
                            filter |= DialogFilter.RealMonsters;
                            break;
                    }
                }
                break;
            //clients/////////////////////////////////////////////
            case CharacterType.Client:
                //unbuff filter
                if (filter.HasFlag(DialogFilter.Unbuff))
                {
                    switch (actionReceiver)
                    {
                        //real monsters
                        case CharacterType.Ghost:
                        case CharacterType.Wight:
                        case CharacterType.Wisp:
                        case CharacterType.Brasier:
                        case CharacterType.Skeleton:
                        case CharacterType.AngrySkeleton:
                        case CharacterType.ColdSkeleton:
                            filter |= DialogFilter.RealMonsters;
                            break;
                    }
                }
                break;
            //fake monsters /////////////////////////////////
            case CharacterType.Goblins:
            case CharacterType.Orcs:
            case CharacterType.OrcsLeader:
            case CharacterType.Treant:
                //unbuff filter
                if (filter.HasFlag(DialogFilter.Unbuff))
                {
                    //get fake monsters dialog
                    foreach (var filteredQuote in GetFilteredCharacterQuotes(filter,actionExecutor))
                        quotes.Add(filteredQuote);
                    // and then add client target condition
                    if (actionReceiver == CharacterType.Client)
                        filter |= DialogFilter.Clients;
                }
                //enemy vectory
                if(filter.HasFlag(DialogFilter.Victory))
                    filter |= DialogFilter.FakeMonsters;
                break;
            //monsters ////////////////////////////////////////
            case CharacterType.Ghost:
            case CharacterType.Wight:
            case CharacterType.Wisp:
            case CharacterType.Brasier:
            case CharacterType.Skeleton:
            case CharacterType.AngrySkeleton:
            case CharacterType.ColdSkeleton:
                //enemy vectory
                if(filter.HasFlag(DialogFilter.Victory))
                    filter |= DialogFilter.RealMonsters;
                break;
        }
        // get filtered dialogs
        if(actionExecutor != CharacterType.None)
            foreach (var filteredQuote in GetFilteredCharacterQuotes(filter,actionExecutor))
                quotes.Add(filteredQuote);
        
        // get receivers dialog
        switch (actionReceiver)
        {
            case CharacterType.Client:
                if (filter.HasFlag(DialogFilter.Damage) || filter.HasFlag(DialogFilter.CriticalDamage) ||
                    filter.HasFlag(DialogFilter.Kill) || filter.HasFlag(DialogFilter.Heal) ||
                    filter.HasFlag(DialogFilter.Buff) || filter.HasFlag(DialogFilter.Unbuff))
                    filter |= DialogFilter.Clients;
                break;
            //fake monsters
            case CharacterType.Goblins:
            case CharacterType.Orcs:
            case CharacterType.OrcsLeader:
            case CharacterType.Treant:
            case CharacterType.Golem:
                if (filter.HasFlag(DialogFilter.Damage) || filter.HasFlag(DialogFilter.CriticalDamage) ||
                    filter.HasFlag(DialogFilter.Kill) || filter.HasFlag(DialogFilter.Heal) ||
                    filter.HasFlag(DialogFilter.Buff) || filter.HasFlag(DialogFilter.Unbuff))
                    filter |= DialogFilter.FakeMonsters;
                break;
            //real monsters
            case CharacterType.Ghost:
            case CharacterType.Wight:
            case CharacterType.Wisp:
            case CharacterType.Brasier:
            case CharacterType.Skeleton:
            case CharacterType.AngrySkeleton:
            case CharacterType.ColdSkeleton:
                if (filter.HasFlag(DialogFilter.Damage) || filter.HasFlag(DialogFilter.CriticalDamage) ||
                    filter.HasFlag(DialogFilter.Kill) || filter.HasFlag(DialogFilter.Heal) ||
                    filter.HasFlag(DialogFilter.Buff) || filter.HasFlag(DialogFilter.Unbuff))
                    filter |= DialogFilter.RealMonsters;
                break;
        }
        if(actionReceiver != CharacterType.None)
            foreach (var filteredQuote in GetFilteredCharacterQuotes(filter,actionReceiver))
                quotes.Add(filteredQuote);

        // when all characters are involved  
        if(actionExecutor == CharacterType.None && actionReceiver == CharacterType.None)
            foreach (var character in dialogs.Keys)
            foreach (var filteredQuote in GetFilteredCharacterQuotes(filter,character))
                quotes.Add(filteredQuote);
        
        if (quotes.Count != 0)
        {
            int rand = Random.Range(0, quotes.Count);
            closedList.Add(quotes[rand].phrase);
            return quotes[rand];
        }

        return new Quote();
    }

    private IEnumerable<Quote> GetFilteredCharacterQuotes(DialogFilter filter, CharacterType character)
    {
        if (!dialogs.ContainsKey(character)) {
            Debug.Log("Warning : tried to get quotes from unkown character type : " + character);
        }
        foreach (var dialogPhrase in dialogs[character])
            if (dialogPhrase.filter == filter)
                yield return new Quote() {character = character, phrase = dialogPhrase.phrase};
    }

    public async Task ShowDialog(DialogFilter filter, CharacterType actionExecutor, CharacterType actionReceiver)
    {
        if (!IsDialogAvailable()) return;
        
        Quote quote = GetDialogQuote(filter, actionExecutor, actionReceiver);
        dialogText.text = quote.phrase;
        dialogText.maxVisibleCharacters = 0;
        dialogImage = await Utils.LoadResource<Image>("" + quote.character.ToString());
        dialogCanvas.DOFade(1, .5f).SetEase(Ease.InOutCubic);
        await Utils.AwaitObservable(
            Observable.Timer(TimeSpan.FromMilliseconds(textSpeed))
                .Repeat()
                .Zip(quote.phrase.ToObservable(), (_, y) => y),
            _ => dialogText.maxVisibleCharacters += 1);
        await Utils.AwaitObservable(Observable.Timer(TimeSpan.FromSeconds(1)));
        dialogCanvas.DOFade(0, .5f).SetEase(Ease.InOutCubic);
    }
}
