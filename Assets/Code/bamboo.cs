using UnityEngine;
using MidiJack;

public class bamboo : MonoBehaviour
{
    public int midiCCNumber = 0;           // nanoKONTROL2 第幾個旋鈕，用來控制動畫速度
    public float maxSpeed = 2f;            // 最大動畫速度
    public float minSpeed = 0.1f;          // 最小動畫速度
    public int midiVolumeCCNumber = 1;     // 用來控制竹林音量的 MIDI CC 號（假設這是音量控制）
    public AudioClip bambooAudioClip;      // 用來播放的 MP3 音效
    
    private Animator animator;
    private AudioSource bambooAudioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // 確保 Bamboo 有一個 AudioSource 來播放音效
        bambooAudioSource = GetComponent<AudioSource>();
        if (bambooAudioSource == null)
        {
            bambooAudioSource = gameObject.AddComponent<AudioSource>(); // 如果沒有 AudioSource，則自動添加一個
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
    }

    void Update()
    {
        // 控制動畫速度
        float knobValue = MidiMaster.GetKnob(midiCCNumber); // 0~1
        float speed = Mathf.Lerp(minSpeed, maxSpeed, knobValue);
        animator.speed = speed;

        // 控制竹林音量
        float volumeKnobValue = MidiMaster.GetKnob(midiVolumeCCNumber); // 0~1 控制音量
        bambooAudioSource.volume = volumeKnobValue;  // 這樣可以根據旋鈕來控制音量大小

        // Debug 訊息輸出
        Debug.Log($"[Bamboo CC#{midiCCNumber}] Knob = {knobValue:F2} → Speed = {speed:F2}");
        Debug.Log($"[Bamboo CC#{midiVolumeCCNumber}] Knob = {volumeKnobValue:F2} → Volume = {bambooAudioSource.volume:F2}");
    }
}