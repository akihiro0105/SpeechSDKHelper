using UnityEngine;
using UnityEngine.UI;

public class Speech2TextSample : MonoBehaviour
{
    public Text recognizeText;

    /// <summary>
    /// Azureから取得したサブスクリプションキーとリージョンを設定
    /// </summary>
    private const string subscriptionKey = "<subscriptionKey>";
    private const string region = "<region>";

    private SpeechSDKHelper speechSDKHelper = new SpeechSDKHelper(subscriptionKey, region);

    public async void Recognize()
    {
        recognizeText.text = "音声認識中";
        recognizeText.text = await speechSDKHelper.Speech2Text();
    }
}
