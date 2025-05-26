using UnityEngine;
using MidiJack;

public class BambooAnimatorSpeed : MonoBehaviour
{
    public int midiCCNumber = 0;           // nanoKONTROL2 第幾個旋鈕，用來控制動畫速度
    public float maxSpeed = 2f;            // 最大動畫速度
    public float minSpeed = 0.1f;          // 最小動畫速度
    public int midiVolumeCCNumber = 1;     // 用來控制竹林音量的 MIDI CC 號（假設這是音量控制）
    public AudioClip bambooAudioClip;      // 用來播放的 MP3 音效
    
    private Animator animator;
    private AudioSource bambooAudioSource;
    private GlobalVolumeController globalVolumeController; // 引用 GlobalVolumeController 來判斷是否有音效播放

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // 確保 Bamboo 有一個 AudioSource 來播放音效
        bambooAudioSource = GetComponent<AudioSource>();
        if (bambooAudioSource == null)
        {
            bambooAudioSource = gameObject.AddComponent<AudioSource>(); // 如果沒有，則自動添加一個 AudioSource
        }
        
        // 設定音效
        if (bambooAudioClip != null)
        {
            bambooAudioSource.clip = bambooAudioClip; // 設定 AudioClip 為您所選的 MP3 音效
            bambooAudioSource.loop = true;             // 可以設置為循環播放（視情況而定）
            bambooAudioSource.Play();                  // 開始播放音效
        }
        else
        {
            Debug.LogError("Bamboo AudioClip is not assigned!");
        }

        // 獲取 GlobalVolumeController 組件
        globalVolumeController = FindObjectOfType<GlobalVolumeController>();
    }

    void Update()
    {
        // 控制動畫速度
        float knobValue = MidiMaster.GetKnob(midiCCNumber); // 0~1
        float speed = Mathf.Lerp(minSpeed, maxSpeed, knobValue);
        animator.speed = speed;

        // 只在 GlobalVolumeController 顯示有音效播放的情況下，控制竹林音量
        if (globalVolumeController != null && globalVolumeController.isAnySoundPlaying)
        {
            // 控制竹林音量
            float volumeKnobValue = MidiMaster.GetKnob(midiVolumeCCNumber); // 0~1 控制音量
            bambooAudioSource.volume = volumeKnobValue;  // 這樣可以根據旋鈕來控制音量大小
        }

        // Debug 訊息輸出
        //Debug.Log($"[Bamboo CC#{midiCCNumber}] Knob = {knobValue:F2} → Speed = {speed:F2}");
        //Debug.Log($"[Bamboo CC#{midiVolumeCCNumber}] Knob = {globalVolumeController.isAnySoundPlaying} → Volume = {bambooAudioSource.volume:F2}");

        //float knobValue = MidiMaster.GetKnob(midiCCNumber);
        //Debug.Log($"MIDI Knob Value for CC#{midiCCNumber}: {knobValue}");

        //float volume = Mathf.Lerp(minVolume, maxVolume, knobValue);
        //bambooAudioSource.volume = volume;
    }
}
