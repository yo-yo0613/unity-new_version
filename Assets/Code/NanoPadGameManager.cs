using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.Universal; // 引入 URP Light2D
using System.Collections.Generic; // 引入 List
using MidiJack; // 引入 MidiJack 库

public class NanoPadImageGame : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public Image image3;

    public Sprite[] imageOptions;

    public GameObject[] glowPrefabs; // 用 Light 2D 作為 Fire Effect
    public Transform[] firePoints; // 7個固定位置

    private bool stop1 = false, stop2 = false, stop3 = false;
    private Coroutine spinningCoroutine;

    private bool hasWon = false; // 加入這個旗標，避免重複觸發
    private int activeFirePointsCount = 7; // 啟動的火焰點數量

    public GameObject fairyImagePrefab;      // 小精靈 UI Image prefab（內含 CanvasGroup）
    public Transform fairySpawnPoint;        // 小精靈出現位置（在 Canvas 裡）
    private bool fairySpawned = false;       // 控制只出現一次

    public GameObject fireflyPrefab;
    public int maxFireflies = 100;

    private List<GameObject> fireflies = new List<GameObject>();



    void Update()
    {
        // 這裡直接觸發按鍵後檢查圖片是否一致
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stop1 = stop2 = stop3 = false;
            hasWon = false;

            if (spinningCoroutine != null)
                StopCoroutine(spinningCoroutine);

            spinningCoroutine = StartCoroutine(SpinImages());
        }

        // 檢查按鍵是否觸發停止
        if (Input.GetKeyDown(KeyCode.T)) stop1 = true;
        if (Input.GetKeyDown(KeyCode.Y)) stop2 = true;
        if (Input.GetKeyDown(KeyCode.U)) stop3 = true;

        // 確保按鍵都按下後執行
        if (!hasWon && stop1 && stop2 && stop3)
        {
            hasWon = true;  // 當三張圖片一致，開始觸發光源效果

            if (spinningCoroutine != null)
            {
                StopCoroutine(spinningCoroutine);
                spinningCoroutine = null;
            }

            // 強制修改圖片來檢查條件
            if (image1.sprite == image2.sprite && image2.sprite == image3.sprite)
            {
                Debug.Log("我贏了第一張圖片 🎉");
                SpawnFireEffect(); // 啟動所有光源

                if (!fairySpawned)
                {
                    StartCoroutine(SpawnFairyWithFade());
                    fairySpawned = true;
                }
            }

            // 強制修改圖片來檢查條件
            if (image2.sprite == image1.sprite && image1.sprite == image3.sprite)
            {
                Debug.Log("我贏了第二張圖片 🎉");
                float midiValue = 15; // 假設這是從 MIDI 取得的值
                int targetCount = Mathf.RoundToInt(midiValue * maxFireflies);

                AdjustFireflyCount(targetCount);
            }

            // 強制修改圖片來檢查條件
            if (image3.sprite == image1.sprite && image1.sprite == image2.sprite)
            {
                Debug.Log("我贏了第三張圖片 🎉");
            }
        }
    }

    // 控制圖片隨機變換
    IEnumerator SpinImages()
    {
        while (true)
        {
            Sprite newSprite = GetRandomSprite();

            image1.sprite = newSprite;
            image2.sprite = newSprite;
            image3.sprite = newSprite;

            yield return new WaitForSeconds(0.1f); // 控制圖片的轉動速度
        }
    }

    IEnumerator SpawnFairyWithFade()
    {
        GameObject fairy = Instantiate(fairyImagePrefab, fairySpawnPoint.position, Quaternion.identity);

        // 設定為 Canvas 子物件
        Transform canvas = GameObject.Find("Canvas")?.transform;
        if (canvas != null)
            fairy.transform.SetParent(canvas, false); // 保持 localPosition 不變

        // 設定透明度起始值
        CanvasGroup cg = fairy.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;

            float duration = 1.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            cg.alpha = 1f;
        }

        Debug.Log("小精靈淡入完成 ✨");
    }

    Sprite GetRandomSprite()
    {
        int index = Random.Range(0, imageOptions.Length);
        return imageOptions[index]; // 隨機選擇圖片
    }

    // 當圖片相同時顯示火焰效果
    void SpawnFireEffect()
    {
        // 這裡可以確保每個光源都在指定的位置
        int activeFirePoints = Mathf.Min(firePoints.Length, glowPrefabs.Length, activeFirePointsCount);
        for (int i = 0; i < activeFirePoints; i++)
        {
            GameObject clone = Instantiate(glowPrefabs[i], firePoints[i].position, Quaternion.identity);
            clone.transform.localScale = new Vector3(100.5f, 100.5f, 100.5f);

            Light2D light2D = clone.GetComponent<Light2D>();
            if (light2D != null)
            {
                light2D.lightType = Light2D.LightType.Point;
                light2D.lightOrder = i;  // 控制顯示順序
                light2D.enabled = true;   // 啟動光源
            }
        }
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
}
