using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Text2SpeechSample : MonoBehaviour
{
    public string synthText = "あらゆる現実を全て自分の方へ捻じ曲げたのだ";

    /// <summary>
    /// Azureから取得したサブスクリプションキーとリージョンを設定
    /// </summary>
    private const string subscriptionKey = "<subscriptionKey>";
    private const string region = "<region>";

    private SpeechSDKHelper speechSDKHelper = new SpeechSDKHelper(subscriptionKey, region);

    public async void SynthesizeAudioClip()
    {
        var clip = await speechSDKHelper.Text2SpeechAudioClip(synthText);
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    public async void SynthesizeWAV()
    {
        await speechSDKHelper.Text2SpeechWAV(synthText, Path.Combine(Application.dataPath, "test.wav"));
    }
}
