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
    }

    void Update()
    {
        foreach (var group in spriteGroups)
        {
            if (MidiMaster.GetKeyDown(group.midiNote))
            {
                if (!group.isVisible)
                {
                    foreach (var sr in group.sprites)
                        StartCoroutine(FadeTo(sr, 1f));
                    group.isVisible = true;
                    group.hasTriggered = true;
                    Debug.Log($"【{group.groupName}】→ 淡入");
                }
                else
                {
                    foreach (var sr in group.sprites)
                        StartCoroutine(FadeTo(sr, 0f));
                    group.isVisible = false;
                    Debug.Log($"【{group.groupName}】→ 淡出");
                }
            }
        }

        // 檢查是否所有 group 都觸發過，且 water 還沒出現過
        if (!waterShown && AllGroupsTriggered())
        {
            waterShown = true;
            StartCoroutine(ShowWaterAndSwitchScene());
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

    IEnumerator ShowWaterAndSwitchScene()
    {
        Debug.Log("🎉 所有小精靈已觸發，water 精靈出現");
        if (waterSprite != null)
        {
            yield return StartCoroutine(FadeTo(waterSprite, 1f));
        }

        Debug.Log($"⌛ 等待 {waitBeforeSceneChange} 秒切換場景...");
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
