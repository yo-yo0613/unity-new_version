using UnityEngine;
using UnityEngine.UI;  // 引入 UI 命名空間
using System.Collections;
using MidiJack;  // 引入 MidiJack 库

public class RiverControllers : MonoBehaviour
{
    public GameObject river;          // 河流 GameObject
    public Image fairyImage;          // 小精靈圖片的 Image 組件

    public int midiCCNumber = 1;      // MIDI 控制器編號
    private bool isFading = false;    // 判斷小精靈是否正在淡出
    private float fadeSpeed = 2f;     // 控制淡出速度，數值越大，淡出越快

    private void Update()
    {
        // 取得 MIDI 控制器的 Knob 值，範圍是 0 到 1
        float riverOpacity = MidiMaster.GetKnob(midiCCNumber);  // 取得 MIDI 控制器的透明度值

        // 透過透明度改變河流的透明度
        SpriteRenderer riverRenderer = river.GetComponent<SpriteRenderer>();
        Color color = riverRenderer.color;
        color.a = Mathf.Lerp(1, 0, riverOpacity);  // 使用Lerp來平滑透明度變化
        riverRenderer.color = color;

        Debug.Log($"River Opacity: {color.a}");  // 輸出當前河流透明度到控制台

        // 控制小精靈顯示與否，當透明度低於 20% 時顯示小精靈，否則隱藏
        if (riverOpacity > 0.2f && !isFading)
        {
            StartCoroutine(FadeInFairy());  // 開始小精靈的淡入效果
        }
        else if (riverOpacity <= 0.2f && !isFading)
        {
            StartCoroutine(FadeOutFairy());  // 開始小精靈的淡出效果
        }
    }

    // 小精靈的淡入效果
    private IEnumerator FadeInFairy()
    {
        isFading = true;
        float targetAlpha = 1f;  // 目標透明度為完全可見
        float currentAlpha = fairyImage.color.a;

        // 在一定時間內平滑過渡透明度
        while (fairyImage.color.a < targetAlpha)
        {
            Color color = fairyImage.color;
            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            fairyImage.color = color;
            yield return null;
        }

        fairyImage.enabled = true;  // 顯示小精靈
        isFading = false;
    }

    // 小精靈的淡出效果
    private IEnumerator FadeOutFairy()
    {
        isFading = true;
        float targetAlpha = 0f;  // 目標透明度為完全透明
        float currentAlpha = fairyImage.color.a;

        // 在一定時間內平滑過渡透明度
        while (fairyImage.color.a > targetAlpha)
        {
            Color color = fairyImage.color;
            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            fairyImage.color = color;
            yield return null;
        }

        fairyImage.enabled = false;  // 隱藏小精靈
        isFading = false;
    }
}
