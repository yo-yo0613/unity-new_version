using System.Collections; // ← 為了使用 IEnumerator
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingSceneToThree : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndLoadScene());
    }

    IEnumerator WaitAndLoadScene()
    {
        Debug.Log("開始等待，預計等待 1 秒");
        yield return new WaitForSeconds(1f); // 等待 15 秒

        // 可選：這裡可以加動畫播放完成的判斷條件

        SceneManager.LoadScene("threee"); // 切換到指定場景
    }
}