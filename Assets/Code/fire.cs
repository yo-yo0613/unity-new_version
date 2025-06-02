using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MidiJack;
using System.Collections.Generic; // 引入 List
using UnityEngine.SceneManagement;

public class fire : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public Image image3;

    public Sprite[] imageOptions;

    public float[] midiCCNumbers = {44, 42, 41, 45}; // 第一個 MIDI 控制器編號

    public GameObject glowPrefab; // 火焰粒子物件 (Glow)
    public Transform[] firePoints; // 4個固定位置

    private bool stop1 = false, stop2 = false, stop3 = false;
    private Coroutine spinningCoroutine;

    private bool hasWon = false; // ✅ 加入這個旗標，避免重複觸發

    public Sprite targetImage1;  // 用來指定當三張圖片相同時觸發的圖片

    public Sprite targetImage2;

    public Sprite targetImage3;

    public Image fairyImage1;      // 小精靈 UI Image prefab（內含 CanvasGroup）
    public Image fairyImage2;
    public Image fairyImage3;
    private bool fairyShown1 = false;       // 控制只出現一次
    private bool fairyShown2 = false;
    private bool fairyShown3 = false;
    public GameObject water;    // 水的圖片

    public GameObject fireflyPrefab;
    public int maxFireflies = 100;

    private List<GameObject> fireflies = new List<GameObject>();

    private bool sceneTransitionStarted = false;

    void Start()
    {
        if (fairyImage1 != null)
        {
            CanvasGroup cg = fairyImage1.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f; // 一開始完全透明
            }
        }

        if (fairyImage2 != null)
        {
            CanvasGroup cg = fairyImage2.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f; // 一開始完全透明
            }
        }

        if (fairyImage3 != null)
        {
            CanvasGroup cg = fairyImage3.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f; // 一開始完全透明
            }
        }

        if (water != null)
        {
            SpriteRenderer sr = water.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0f;
                sr.color = color;
            }
        }
    }


    void Update()
    {

        if (MidiMaster.GetKnob((int)midiCCNumbers[0]) > 0.5f  || Input.GetKeyDown(KeyCode.Space)) // 使用 MIDI 按鍵或空白鍵來開始旋轉
        {
            stop1 = stop2 = stop3 = false;
            hasWon = false;

            if (spinningCoroutine != null)
                StopCoroutine(spinningCoroutine);

            spinningCoroutine = StartCoroutine(SpinImages());
        }

        if (MidiMaster.GetKnob((int)midiCCNumbers[1]) > 0.5f || Input.GetKeyDown(KeyCode.T)) stop1 = true;
        if (MidiMaster.GetKnob((int)midiCCNumbers[2]) > 0.5f || Input.GetKeyDown(KeyCode.Y)) stop2 = true;
        if (MidiMaster.GetKnob((int)midiCCNumbers[3]) > 0.5f || Input.GetKeyDown(KeyCode.U)) stop3 = true;

        if (!hasWon && stop1 && stop2 && stop3)
        {
            hasWon = true;

            if (spinningCoroutine != null)
            {
                StopCoroutine(spinningCoroutine);
                spinningCoroutine = null;
            }

            // 檢查 image1, image2, image3 是否都與 targetImage 相同
            if (image1.sprite == targetImage1 && image2.sprite == targetImage1 && image3.sprite == targetImage1)
            {
                Debug.Log("我贏了第一張圖片 🎉");
                SpawnFireEffect();  // 當三張圖片都是目標圖片時，才會生成火焰效果

                if (!fairyShown1)
                {
                    StartCoroutine(FadeInFairy1());
                    fairyShown1 = true;
                }
            }

            // 檢查 image1, image2, image3 是否都與 targetImage 相同
            if (image1.sprite == targetImage2 && image2.sprite == targetImage2 && image3.sprite == targetImage2)
            {
                Debug.Log("我贏了第二張圖片 🎉");

                if (!fairyShown2)
                {
                    StartCoroutine(FadeInFairy2());
                    fairyShown2 = true;
                }

                float midiValue = 1; // 假設這是從 MIDI 取得的值
                int targetCount = Mathf.RoundToInt(midiValue * maxFireflies);

                AdjustFireflyCount(targetCount);
            }

            // 檢查 image1, image2, image3 是否都與 targetImage 相同
            if (image1.sprite == targetImage3 && image2.sprite == targetImage3 && image3.sprite == targetImage3)
            {
                Debug.Log("我贏了第三張圖片 🎉");

                if (!fairyShown3)
                {
                    StartCoroutine(FadeInFairy3());
                    fairyShown3 = true;
                }

                ShowWaterImmediately(); // 🔥 呼叫封裝好的方法
                StartCoroutine(FadeInWater());

            }

            // 新增這段：當三隻小精靈都出現，觸發轉場（只一次）
            if (fairyShown1 && fairyShown2 && fairyShown3 && !sceneTransitionStarted)
            {
                sceneTransitionStarted = true;
                StartCoroutine(WaitAndLoadScene());
            }

        }
    }


    IEnumerator SpinImages()
    {
        while (true)
        {
            Sprite newSprite = GetRandomSprite();

            if (!stop1) image1.sprite = newSprite;
            if (!stop2) image2.sprite = newSprite;
            if (!stop3) image3.sprite = newSprite;

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator FadeInFairy1()
    {
        CanvasGroup cg = fairyImage1.GetComponent<CanvasGroup>();
        if (cg == null)
            yield break;

        float duration = 1.5f;
        float elapsed = 0f;
        cg.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
        Debug.Log("小精靈淡入完成1 ✨");
    }

    IEnumerator FadeInFairy2()
    {
        CanvasGroup cg = fairyImage2.GetComponent<CanvasGroup>();
        if (cg == null)
            yield break;

        float duration = 1.5f;
        float elapsed = 0f;
        cg.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
        Debug.Log("小精靈淡入完成2 ✨");
    }

    IEnumerator FadeInFairy3()
    {
        CanvasGroup cg = fairyImage3.GetComponent<CanvasGroup>();
        if (cg == null)
            yield break;

        float duration = 1.5f;
        float elapsed = 0f;
        cg.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
        Debug.Log("小精靈淡入完成3 ✨");
    }

    Sprite GetRandomSprite()
    {
        int index = Random.Range(0, imageOptions.Length);
        return imageOptions[index];
    }

    private bool fireEffectSpawned = false; // 這個變數用來確保火焰只生成一次

    void SpawnFireEffect()
    {
        if (fireEffectSpawned) return;  // 如果火焰已經生成過，直接返回，不再執行

        foreach (Transform point in firePoints)
        {
            GameObject clone = Instantiate(glowPrefab, point.position, Quaternion.identity);
            clone.transform.localScale = new Vector3(100.5f, 100.5f, 100.5f);
        }

        fireEffectSpawned = true; // 設置為 true，表示火焰已經生成過
    }

    void AdjustFireflyCount(int target)
    {
        while (fireflies.Count < target)
        {
            SpawnFirefly();
        }

        while (fireflies.Count > target)
        {
            RemoveFirefly();
        }
    }

    void SpawnFirefly()
    {
        Vector3 screenPos = new Vector3(
            Random.Range(0f, Screen.width),
            Random.Range(0f, Screen.height),
            10f // 距離攝影機一定深度（調整畫面可見範圍）
        );

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        GameObject f = Instantiate(fireflyPrefab, worldPos, Quaternion.identity);
        fireflies.Add(f);
    }

    void RemoveFirefly()
    {
        GameObject f = fireflies[fireflies.Count - 1];
        fireflies.RemoveAt(fireflies.Count - 1);
        Destroy(f);
    }

    void ShowWaterImmediately()
    {
        if (water != null)
        {
            SpriteRenderer sr = water.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 1f; // 完全顯示
                sr.color = color;
                Debug.Log("✅ 水的透明度已設為 1");
            }
        }
    }

    IEnumerator FadeInWater()
    {
        SpriteRenderer sr = water.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float duration = 1.5f;
        float elapsed = 0f;
        Color color = sr.color;
        color.a = 0f;
        sr.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            sr.color = color;
            yield return null;
        }

        color.a = 1f;
        sr.color = color;

        Debug.Log("✨ 水已淡入完成！");
    }

    IEnumerator WaitAndLoadScene()
    {
        Debug.Log("✨ 所有小精靈皆出現，開始倒數轉場...");
        
        yield return new WaitForSeconds(1f); // 等 10 秒

        // 👉 若你有動畫，這裡可以播放動畫、等待它完成
        // yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("XXX") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        SceneManager.LoadScene("SecondToThree"); // ← 請改成你要的場景名稱
}
}
