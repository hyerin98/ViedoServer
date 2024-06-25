using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using TMPro;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController instance;
    public MediaPlayer mediaPlayer;
    public TextMeshProUGUI messageText;

    bool isExit;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mediaPlayer.Events.AddListener(OnMediaPlayerEvent); // MediaPlayer 이벤트 리스너 등록
    }

    public void ShowMessage(string data)
    {
        if (messageText != null)
        {
            messageText.text += messageText.text == "" ? data : "\n" + data;
        }
        Debug.Log(data);

        Invoke("DelayHideMessage", 2f);
    }

    void DelayHideMessage()
    {
        messageText.gameObject.SetActive(false);
    }

    public void PlayVideo(string videoName)
{
    if (mediaPlayer != null)
    {
        string videoPath = $"Assets/Videos/{videoName}.mp4";
        
        mediaPlayer.OpenMedia(MediaPathType.RelativeToProjectFolder, videoPath, false);
        mediaPlayer.Control.SetLooping(videoName == "4"); // "4" 비디오에만 루프 설정
        mediaPlayer.Play();
    }
    else
    {
        Debug.LogError("MediaPlayer is not assigned!");
    }
}


    void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
    {
        if (eventType == MediaPlayerEvent.EventType.FinishedPlaying && mp == mediaPlayer)
        {
            PlayVideo("4"); // 비디오 재생이 끝나면 Idle 비디오 재생
        }
    }

    public void OnIncomingData(string data)
    {
        if (data.StartsWith("VIDEO"))
        {
            string videoName = data.Substring(5); // "VIDEO1" -> "1"
            PlayVideo(videoName);
        }
        else if (data.StartsWith("EXIT"))
        {
            isExit = true;
            string videoName = data.Substring(4);
            PlayVideo(videoName);
        }
    }
}