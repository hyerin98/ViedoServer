using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using TMPro;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController instance;
    [SerializeField] VideoManagerUI videoManagerUI;
    public TextMeshProUGUI messageText;

    void Awake()
    {
        instance = this;

        videoManagerUI.VideoStarted += OnVideoStarted;
        videoManagerUI.VideoFinished += OnVideoFinished;
    }

    private void OnVideoStarted(string name)
    {
    }

    private void OnVideoFinished(string name)
    {
        switch (name)
        {
            case "2":
                TraceBox.Log("VIDEO" + name + " 영상끝남");
                videoManagerUI.PlayVideo("1");
                break;
            case "3":
                TraceBox.Log("VIDEO" + name + " 영상끝남");
                videoManagerUI.PlayVideo("1");
                break;
            case "4":
                TraceBox.Log("VIDEO" + name + " 영상끝남");
                videoManagerUI.PlayVideo("1");
                break;
        }

        // 서버에 비디오 종료 알림
        ServerNotifyVideoFinished(name);
        //TraceBox.Log(name + "이라는 이름을 가진 동영상 끝났슴");
    }

    public void ServerNotifyVideoFinished(string videoName)
    {
        // 서버에 비디오 종료 알림 메시지를 보냄
        if (Server.instance != null)
        {
            Server.instance.Broadcast($"VIDEO_FINISHED|{videoName}", Server.instance.clients);
            //TraceBox.Log("서버에 메시지 보내기: " + videoName + "!!!!");
        }
    }


    void Start()
    {
        videoManagerUI.SetVideo("1", MediaPathType.RelativeToDataFolder, "StreamingAssets/1.mp4", true, false, true);
        videoManagerUI.SetVideo("2", MediaPathType.RelativeToDataFolder, "StreamingAssets/2.mp4", false);
        videoManagerUI.SetVideo("3", MediaPathType.RelativeToDataFolder, "StreamingAssets/3.mp4", false);
        videoManagerUI.SetVideo("4", MediaPathType.RelativeToDataFolder, "StreamingAssets/4.mp4", false);

        videoManagerUI.ChangeLoop("2", false);
        videoManagerUI.ChangeLoop("3", false);
        videoManagerUI.ChangeLoop("4", false);
    }

    public void ShowMessage(string data)
    {
        if (messageText != null)
        {
            messageText.text += messageText.text == "" ? data : "\n" + data;
        }
        //TraceBox.Log(data);

        Invoke("DelayHideMessage", 2f);
    }

    void DelayHideMessage()
    {
        messageText.gameObject.SetActive(false);
    }

    public void OnIncomingData(string data)
    {
        if (data.StartsWith("VIDEO"))
        {
            string videoName = data.Substring(5); // "VIDEO1" -> "1"
            videoManagerUI.PlayVideo(videoName);
        }
        else if (data.StartsWith("EXIT"))
        {
            string videoName = data.Substring(4);
            videoManagerUI.PlayVideo(videoName);
        }
    }
}

