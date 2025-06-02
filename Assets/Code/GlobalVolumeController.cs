using UnityEngine;
using MidiJack;

public class GlobalVolumeController : MonoBehaviour
{
    public bool isAnySoundPlaying = false;
    public int midiCCNumber = 1; // 請確保 Inspector 有設定，0 通常沒作用
    public float minVolume = 0f;
    public float maxVolume = 1f;

    private AudioSource[] allAudioSources;
    private float updateTimer = 0f;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SetVolume(savedVolume);
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer > 1f)
        {
            allAudioSources = FindObjectsOfType<AudioSource>();
            updateTimer = 0f;
        }

        float knob = MidiMaster.GetKnob(midiCCNumber);
        Debug.Log("Midi knob value: " + knob);

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

        PlayerPrefs.SetFloat("MusicVolume", volume);

        // 更新是否有聲音播放
        isAnySoundPlaying = volume > 0f;
    }
}
