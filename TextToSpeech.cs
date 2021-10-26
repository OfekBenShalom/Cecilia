
//This script created by Ofek Ben Shalom.

using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class TextToSpeech : MonoBehaviour    
{
    public InputField inputField;
    public AudioSource audioSource;          

    public string voice = "";                //Choose the voice of the character
    public string Language = "";            //Choose the language of the character

    public string answerResult;             //What will be te answer of the character.
 

    void Update()
    {
        inputField.text = answerResult;
    }

    public void Speech()
    {
        // Replace with your own subscription key and service region (e.g., "westus").
        var config = SpeechConfig.FromSubscription("2352ed5e3ff147399bad73ea0236b5bf", "westeurope");

        config.SpeechSynthesisLanguage = Language; // e.g. "de-DE"
        // The voice setting will overwrite language setting.
        // The voice setting will not overwrite the voice element in input SSML.

        config.SpeechSynthesisVoiceName = voice;

        using (var synthsizer = new SpeechSynthesizer(config, null))
        {
            var result = synthsizer.SpeakTextAsync(inputField.text).Result; // Checks result.

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                var sampleCount = result.AudioData.Length / 2;
                var audioData = new float[sampleCount];

                //Audio system convert to audioclip
                for (var i = 0; i < sampleCount; ++i)
                {
                    audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
                }

                // The default output audio format is 16K 16bit mono
                var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
                audioClip.SetData(audioData, 0);
                audioSource.clip = audioClip;
                audioSource.Play();

            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            }
        }
    }
}