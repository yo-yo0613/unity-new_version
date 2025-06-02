using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MidiJack;

public class SpriteFadeController : MonoBehaviour
{
    [System.Serializable]
    public class SpriteGroup
    {
        public string groupName;
        public List<SpriteRenderer> sprites;
        public int midiNote;
        [HideInInspector] public bool isVisible = false;
        [HideInInspector] public bool hasTriggered = false; // 是否曾觸發過
    }

    public List<SpriteGroup> spriteGroups;
    public SpriteRenderer waterSprite;
    public float fadeDuration = 1.0f;
    public float waitBeforeSceneChange = 10f;
    public string nextSceneName = "open"; // 要切換的場景名稱

    private bool waterShown = false;

    private int currentGroupIndex = -1; // 沒有目前顯示的 group

    public UnityEngine.UI.Image fadeImage; // 指派該黑色Image


    void Start()
    {
        foreach (var group in spriteGroups)
        {
            foreach (var sr in group.sprites)
                SetAlpha(sr, 0f);
        }

        if (waterSprite != null)
        {
            SetAlpha(waterSprite, 0f);
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    
    void Update()
    {
        if (MidiMaster.GetKnob(spriteGroups[0].midiNote) > 0.5f || Input.GetKeyDown(KeyCode.A))
        {
            ShowOnlyGroup(0);
        }
        else if (MidiMaster.GetKnob(spriteGroups[1].midiNote) > 0.5f || Input.GetKeyDown(KeyCode.S))
        {
            ShowOnlyGroup(1);
        }
        else if (MidiMaster.GetKnob(spriteGroups[2].midiNote) > 0.5f || Input.GetKeyDown(KeyCode.D))
        {
            ShowOnlyGroup(2);
        }
        else if (MidiMaster.GetKnob(spriteGroups[3].midiNote) > 0.5f || Input.GetKeyDown(KeyCode.F))
        {
            ShowOnlyGroup(3);
        }
        // 依此類推，寫完整個spriteGroups的索引條件判斷
    }

    void ShowOnlyGroup(int indexToShow)
    {
        for (int i = 0; i < spriteGroups.Count; i++)
        {
            bool isTarget = (i == indexToShow);
            foreach (var sr in spriteGroups[i].sprites)
            {
                StartCoroutine(FadeTo(sr, isTarget ? 1f : 0f));
            }
            spriteGroups[i].isVisible = isTarget;

            if (isTarget)
            {
                spriteGroups[i].hasTriggered = true;  // <--- 這裡設定觸發過了
            }
        }

        currentGroupIndex = indexToShow;

        // Debug 輸出所有小精靈的狀態
        Debug.Log("=== 小精靈狀態 ===");
        for (int i = 0; i < spriteGroups.Count; i++)
        {
            Debug.Log($"Group {i} [{spriteGroups[i].groupName}] - isVisible: {spriteGroups[i].isVisible}, hasTriggered: {spriteGroups[i].hasTriggered}");
        }
        Debug.Log("=================");

        if (!waterShown && AllGroupsTriggered())
        {
            waterShown = true;
            StartCoroutine(FadeToBlackAndSwitchScene());
        }
    }
    bool AllGroupsTriggered()
    {
        foreach (var group in spriteGroups)
        {
            if (!group.hasTriggered) return false;
        }
        return true;
    }

    IEnumerator FadeToBlackAndSwitchScene()
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

        Debug.Log($"⌛ 漸黑完成，等待 {waitBeforeSceneChange} 秒切換場景...");
        yield return new WaitForSeconds(waitBeforeSceneChange);

        SceneManager.LoadScene(nextSceneName);
    }



    IEnumerator FadeTo(SpriteRenderer sr, float targetAlpha)
    {
        float t = 0f;
        float startAlpha = sr.color.a;
        Color c = sr.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            sr.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        sr.color = c;
    }

    void SetAlpha(SpriteRenderer sr, float alpha)
    {
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}
