using UnityEngine;
using MidiJack;

public class BambooAnimatorSpeed : MonoBehaviour
{
    public int midiCCNumber = 0;         // nanoKONTROL2 第幾個旋鈕
    public float maxSpeed = 2f;          // 最大動畫速度
    public float minSpeed = 0.1f;        // 最小動畫速度
    public AudioClip bambooClip;         // 竹林的音樂片段（MP3 文件）

    private Animator animator;
    private AudioSource audioSource;     // 用來播放音樂的 AudioSource

    void Start()
    {
        // 获取 Animator 组件
        animator = GetComponent<Animator>();

        // 检查是否有 AudioSource，若没有，则添加一个新的
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 如果音频源没有片段，则设置为指定的 Bamboo Clip（.mp3）
        if (audioSource.clip == null && bambooClip != null)
        {
            audioSource.clip = bambooClip;
            audioSource.loop = true;  // 循环播放
            audioSource.playOnAwake = false;  // 不自动播放
        }

        // 确保音频播放
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        // 获取 MIDI 旋钮值 (0-1)，并映射到动画速度
        float knobValue = MidiMaster.GetKnob(midiCCNumber);
        float speed = Mathf.Lerp(minSpeed, maxSpeed, knobValue);
        animator.speed = speed; // 更新 Animator 播放速度

        // 更新音量：通过旋钮来控制音量的大小
        float volume = Mathf.Lerp(0f, 1f, knobValue);  // 旋钮控制音量在 0 到 1 之间
        audioSource.volume = volume;  // 设置音频的音量

        // 打印调试信息
        Debug.Log($"[Bamboo CC#{midiCCNumber}] Knob = {knobValue:F2} → Speed = {speed:F2}, Volume = {volume:F2}");
    }
}
