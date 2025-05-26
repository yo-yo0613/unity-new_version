using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using MidiJack;

public class SceneSwitcher : MonoBehaviour
{
    [Header("場景切換設定")]
    public int midiNote = 42;                // MIDI 按鈕編號
    public string nextSceneName;             // 要切的下一個場景名稱
    public bool waitForVideoEnd = false;     // 是否等影片播完才允許切換

    private VideoPlayer videoPlayer;
    private bool videoFinished = false;
    private bool hasSwitched = false;        // 避免多次切場景

    void Start()
    {
        InitVideoPlayer();
    }

    void Update()
    {
        CheckMidiInput();
    }

    void InitVideoPlayer()
    {
        videoPlayer = FindObjectOfType<VideoPlayer>();
        if (waitForVideoEnd && videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else if (waitForVideoEnd && videoPlayer == null)
        {
            Debug.LogWarning("找不到 VideoPlayer，但 waitForVideoEnd 被開啟！");
        }
    }

    void CheckMidiInput()
    {
        if (MidiMaster.GetKeyDown(midiNote))
        {
            if (!waitForVideoEnd || videoFinished)
            {
                SwitchScene();
            }
            else
            {
                Debug.Log("影片尚未播放完，無法切換場景");
            }
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
        Debug.Log("影片播放完成！");

        if (waitForVideoEnd)
        {
            SwitchScene();
        }
    }

    void SwitchScene()
    {
        if (hasSwitched) return; // 避免重複進入

        hasSwitched = true;
        Debug.Log("切換到場景: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}
