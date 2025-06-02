using UnityEngine;
using UnityEngine.SceneManagement;
using MidiJack;
using System.Collections;

public class SwitchScene : MonoBehaviour
{
    private bool isLoading = false;  // 防止重複觸發載入

    void Update()
    {
        if (!isLoading && MidiMaster.GetKnob(46) > 0.5f)
        {
            StartCoroutine(LoadSceneAsync("1"));
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 這裡可以設定不立即切換畫面，直到載入完成
        asyncLoad.allowSceneActivation = false;

        // 等待載入進度到達90%
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
            yield return null;
        }

        // 進度到90%後，可以等待一些條件（例如動畫結束），這裡直接允許切換
        asyncLoad.allowSceneActivation = true;

        // 等待場景切換完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("場景切換完成");
        isLoading = false;
    }
}
