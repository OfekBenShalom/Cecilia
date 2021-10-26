
//This script created by Ofek Ben Shalom.

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
using System.IO;

public class Cecila : MonoBehaviour    
{
    static WebCamTexture webcam;
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    public InputField SavePath;
    
    Animator m_anime;                                //Animation system
    float m_timeAnime;                              //Delay after the character hear you
    public Transform cam;                           //Your vision
    public RawImage camFaceCapture;                 //picture you
    string _SavePath = "C:/Users/Ofek-PC/Desktop/"; //Choose the picture Save path(The sign needs to be like this "/" and not like this "\")
    int _CaptureCounter = 0;                       

    // Start is called before the first frame update
    void Start()
    {
        Commands(); 
        m_anime = GetComponent<Animator>();
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
        if (webcam == null)
        {
            webcam = new WebCamTexture();
        }

        if (!webcam.isPlaying)
        {
            webcam.Play();
        }
        camFaceCapture.material.mainTexture = webcam;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam); //Make the character look at you all the time.
        _SavePath = SavePath.text;

        if(SavePath.text == null)
        {
            SavePath.text = "C:/Users/Ofek-PC/Desktop/";
        }
    }

    void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) 
    {
        System.Action keywordAction;
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }  
    }

    void Commands() // All the commands
    {
        keywords.Add("Hello", Hello);
        keywords.Add("Take a picture", TakePicture);
        keywords.Add("Shut down", ShutDown);
        keywords.Add("Thank you", ThankYou);
    }

    void Hello() // The character say "hello..." by command "Hello" 
    {
        m_anime.SetBool("Idle", false);
        m_anime.SetBool("Bow", true);
        m_timeAnime = 2.22f;
        GetComponent<TextToSpeech>().answerResult = "Hello, Welcome to the test application. In order to begin please scan your face";
        StartCoroutine(TimeAnime());
    }

    void Idle() //Default animation
    {
        m_anime.SetBool("Idle", true);
        m_anime.SetBool("Bow", false);
        m_anime.SetBool("Thanks", false);
    }

    void ThankYou()  // The character bless you goodbye by the command "Thank you" 
    {
        m_anime.SetBool("Idle", false);
        m_anime.SetBool("Thanks", true);
        GetComponent<TextToSpeech>().answerResult = "you are welcome, have a nice day";
        m_timeAnime = 2.29f;
        StartCoroutine(TimeAnime());
    }

    IEnumerator TimeAnime()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<TextToSpeech>().Speech();
        yield return new WaitForSeconds(m_timeAnime);
        camFaceCapture.enabled = true;
        Idle();
    }

    void TakePicture() 
    {
        Texture2D photo = new Texture2D(webcam.width, webcam.height);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply(); // Take the picture
        System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter.ToString() + ".png", photo.EncodeToPNG()); // Save the picture on dektop
        ++_CaptureCounter;
        pictureSpeech();
    }
    void pictureSpeech() // The character confirm that the picture has been saved on desktop
    {
        GetComponent<TextToSpeech>().answerResult = "The image is taken and has been saved on your desktop";
        StartCoroutine(TimeAnime());
    }

    void ShutDown() //Shutdown the system and quit 
    {
        Application.Quit();
    }
}
