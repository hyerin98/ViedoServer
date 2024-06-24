using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;
using TMPro;
using UnityEditor.AssetImporters;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController instance;
    [SerializeField] VideoManagerUI videoManagerUI;
    public MediaPlayer mediaPlayer;
    public TextMeshProUGUI messageText;
    public Button ExitButton;

    void Awake()
    {
        instance = this;
    }

    public void ShowMessage(string data)
    {
        if (messageText != null)
        {
            messageText.text += messageText.text == "" ? data : "\n" + data;
        }
        Debug.Log(data);

        Invoke("delay", 2f);
    }

    void delay()
    {
        messageText.gameObject.SetActive(false);
    }

    public void PlayVideo(string videoName)
    {
        //string videoPath = $"Videos/{videoName}.mov"; // 비디오 경로 설정
        string videoPath2 =  $"Assets/Videos/{videoName}.mp4";
        mediaPlayer.OpenMedia(MediaPathType.RelativeToProjectFolder, videoPath2, true);
    }

    public void OnIncomingData(string data)
    {
        if (data.StartsWith("VIDEO"))
        {
            string videoName = data.Substring(5); // "VIDEO1" -> "1"
            PlayVideo(videoName);
        }
    }

    public void PressExitButton(string videoName)
    {
        videoName = "Idle";
        PlayVideo(videoName);
    }
}
