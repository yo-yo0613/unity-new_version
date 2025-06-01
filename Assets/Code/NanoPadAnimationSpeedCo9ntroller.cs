using UnityEngine;
using MidiJack;
using System.Collections;

public class NanoPadAnimationSpeedController : MonoBehaviour
{
    public Animator animator; // 連結到你的 Animator

    public int[] midiNumber = { 43, 45, 47, 35 }; // MIDI 控制器編號

    private bool triggered1 = false; // 鎖定開關
    private bool triggered2 = false;
    private bool triggered3 = false;

    void Update()
    {
        // 檢查 MIDI 按鍵是否被按下
        if (!triggered1 && MidiMaster.GetKeyDown(midiNumber[1]) || !triggered1 && Input.GetKeyDown(KeyCode.A)) // 假設使用 MIDI 通道 0 的按鍵 60
        {
            animator.speed = 1f; // 普通速度
            Debug.Log("速度設為 1.0x");
        }
        else if (!triggered2 && MidiMaster.GetKeyDown(midiNumber[0]) || !triggered2 && Input.GetKeyDown(KeyCode.S)) // 假設使用 MIDI 通道 0 的按鍵 61
        {
            animator.speed = 2f; // 加快
            Debug.Log("速度設為 2.0x");
        }
        else if (!triggered3 && MidiMaster.GetKeyDown(midiNumber[2]) || !triggered3 && Input.GetKeyDown(KeyCode.D)) // 假設使用 MIDI 通道 0 的按鍵 62
        {
            animator.speed = 0.5f; // 慢動作
            Debug.Log("速度設為 0.5x");
        }
        else if (MidiMaster.GetKeyDown(midiNumber[3]) || Input.GetKeyDown(KeyCode.F)) // 假設使用 MIDI 通道 0 的按鍵 63
        {
            animator.speed = 0f; // 停止動畫
            Debug.Log("動畫已停止");
        }
         
    }
}