using UnityEngine;
using MidiJack;

public class GlobalVolumeController : MonoBehaviour
{
    public bool isAnySoundPlaying = false;  // 設定為 public，使其他類別可以訪問

    public int midiCCNumber = 0;
    public float minVolume = 0f;
    public float maxVolume = 1f;

    private AudioSource[] allAudioSources;
    private float updateTimer = 0f;

    void Start()
    {
        // 从 PlayerPrefs 中读取之前保存的音量设置
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); // 默认音量 0.5
        SetVolume(savedVolume);
    }

    void Update()
    {
        // 🔁 每 1 秒自動更新一次 audio sources
        updateTimer += Time.deltaTime;
        if (updateTimer > 1f)
        {
            allAudioSources = FindObjectsOfType<AudioSource>();
            updateTimer = 0f;
        }

        // 获取旋钮的值，控制音量
        float knob = MidiMaster.GetKnob(midiCCNumber);
        float volume = Mathf.Lerp(minVolume, maxVolume, knob);

        SetVolume(volume);
    }

    void SetVolume(float volume)
    {
        if (allAudioSources != null)
        {
            foreach (var source in allAudioSources)
            {
                if (source != null)
                    source.volume = volume;
            }
        }

        // 保存音量设置到 PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // 更新音效播放狀態
        isAnySoundPlaying = volume > 0f;
    }
}
