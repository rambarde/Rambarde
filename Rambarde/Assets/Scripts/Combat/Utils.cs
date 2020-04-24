using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class Utils {
    private static bool AddressableResourceExists(string address) {
        return Addressables.ResourceLocators.OfType<ResourceLocationMap>()
            .SelectMany(locationMap =>
                locationMap.Locations.Keys.Select(key => key.ToString())
            ).Contains(address);
    }
    public static async Task<T> LoadResource<T>(string address) where T : UnityEngine.Object {

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded) {
            Debug.LogError("Cannot load resource at address : " + address);
        }
        return handle.Result;
    }

    public static async Task AwaitObservable<T>(IObservable<T> obs, Action<T> f = null) {
        if (f is null)
            obs.Subscribe();
        else obs.Subscribe(f);
        await obs;
    }

    public delegate void LerpOnSubscribe(Pair<float> pair, GameObject go, ref float curLerpTime, float speed, float lerpTime, ref float t);

    public static IDisposable UpdateGameObjectLerp(Pair<float> values, GameObject go, float speed, float lerpTime, LerpOnSubscribe subscribe, Action<Pair<float>> doOnComplete) {
        float t = 0f, currentLerpTime = 0f;
        return Observable.EveryLateUpdate()
            .TakeWhile(_ => currentLerpTime < lerpTime + speed * Time.deltaTime)
            .DoOnCompleted(() => doOnComplete(values))
            .Subscribe(_ => { subscribe(values, go, ref currentLerpTime, speed, lerpTime, ref t); });
    }
    
    public static Vector3 WorldToUiSpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out Vector2 movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }
    
    public static string SplitPascalCase(string s)
    {
        var r = new Regex(@"(?<!^)(?=[A-Z](?![A-Z]|$))", RegexOptions.IgnorePatternWhitespace);
        return (r.Replace(s, " ")).Replace("_", " ");
    }
}