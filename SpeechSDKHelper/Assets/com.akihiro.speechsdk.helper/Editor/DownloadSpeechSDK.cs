#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DownloadSpeechSDK
{
#if !CognitiveServicesSpeechHelper
    static DownloadSpeechSDK()
    {
        if (EditorUtility.DisplayDialog("SpeechSDK Helper", "SpeechSDKのダウンロードとインポート設定を行います．", "Download & Import setting", "Cancel"))
        {
            DownloadUnitypackage();
        }
    }
#endif

    [MenuItem("SpeechSDK/Download UnityPackage")]
    private static void DownloadUnitypackage()
    {
        Debug.Log("Download SpeechSDK");
        var downloadPath = Application.temporaryCachePath;
        var context = SynchronizationContext.Current;
        Task.Run(async () =>
        {
            var filePath = await DownloadFile(downloadPath);
            context.Post(_ =>
            {
                Debug.Log("Import SpeechSDK");
                AssetDatabase.ImportPackage(filePath, true);
                AddScriptingDefineSymbols();
            }, null);
        });
    }

    [MenuItem("SpeechSDK/Add ScriptingDefineSymbols")]
    private static void AddScriptingDefineSymbols()
    {
        foreach (var item in Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>())
        {
            var symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(item);
            if (!symbol.Contains("CognitiveServicesSpeechHelper"))
            {
                symbol = string.Join(";", symbol, "CognitiveServicesSpeechHelper");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(item, symbol);
            }
        }
    }

    [MenuItem("SpeechSDK/Remove ScriptingDefineSymbols")]
    private static void RemoveScriptingDefineSymbols()
    {
        foreach (var item in Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>())
        {
            var symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(item);
            if (symbol.Contains("CognitiveServicesSpeechHelper"))
            {
                symbol = symbol.Replace("CognitiveServicesSpeechHelper", "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(item, symbol);
            }
        }
    }

    private async static Task<string> DownloadFile(string downloadPath)
    {
        var handler = new HttpClientHandler() { AllowAutoRedirect = false };
        var client = new HttpClient(handler);
        var response = await client.GetAsync("https://aka.ms/csspeech/unitypackage");
        var packageUri = response.Headers.Location;
        response = await client.GetAsync(packageUri);
        if (response.IsSuccessStatusCode)
        {
            var allData = await response.Content.ReadAsByteArrayAsync();
            var filePath = Path.Combine(downloadPath, Path.GetFileName(packageUri.ToString()));
            File.WriteAllBytes(filePath, allData.ToArray());
            return filePath;
        }
        return null;
    }
}
#endif
