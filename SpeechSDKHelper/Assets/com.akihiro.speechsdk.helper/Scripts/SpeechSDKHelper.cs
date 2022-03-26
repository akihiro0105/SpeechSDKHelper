#if CognitiveServicesSpeechHelper
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
#endif
using System.Threading.Tasks;
using UnityEngine;

public class SpeechSDKHelper
{
    private readonly string subscriptionKey;
    private readonly string region;

    public SpeechSDKHelper(string subscriptionKey, string region)
    {
        this.subscriptionKey = subscriptionKey;
        this.region = region;
    }

    public async Task<AudioClip> Text2SpeechAudioClip(string text)
    {
#if CognitiveServicesSpeechHelper
        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechSynthesisLanguage = "ja-JP";
        config.SpeechSynthesisVoiceName = "ja-JP-NanamiNeural";
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);
        using (var synthesizer = new SpeechSynthesizer(config, null))
        {
            using (var result = await synthesizer.SpeakTextAsync(text))
            {
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    var sampleCount = result.AudioData.Length / 2;
                    var audioData = new float[sampleCount];
                    for (var i = 0; i < sampleCount; ++i)
                    {
                        audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
                    }
                    var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
                    audioClip.SetData(audioData, 0);
                    return audioClip;
                }
            }
        }
#endif
        return null;
    }

    public async Task Text2SpeechWAV(string text, string file)
    {
#if CognitiveServicesSpeechHelper
        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechSynthesisLanguage = "ja-JP";
        config.SpeechSynthesisVoiceName = "ja-JP-NanamiNeural";
        var audioConfig = AudioConfig.FromWavFileOutput(Path.Combine(Application.dataPath, "out.wav"));
        using (var synthesizer = new SpeechSynthesizer(config, audioConfig))
        {
            using (var result = await synthesizer.SpeakTextAsync(text))
            {

            }
        }
#endif
    }

    public async Task<string> Speech2Text()
    {
#if CognitiveServicesSpeechHelper
        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechRecognitionLanguage = "ja-JP";
        using (var recognizer = new SpeechRecognizer(config))
        {
            // 最大15秒間録音
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                return result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                Debug.LogWarning("NoMatch");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                Debug.LogWarning($"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}");
            }
        }
#endif
        return null;
    }
}
