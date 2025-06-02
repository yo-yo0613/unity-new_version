using UnityEngine;
using UnityEngine.SceneManagement;
using MidiJack;
using System.Collections;

public class WaitingScene : MonoBehaviour
{
    void Update()
    {
        StartCoroutine(WaitAndLoadScene());
    }

    IEnumerator WaitAndLoadScene()
    {
        Debug.Log("✨ 開始倒數轉場...");
        
        yield return new WaitForSeconds(15f); // 等 15 秒

        // 👉 若你有動畫，這裡可以播放動畫、等待它完成
        // yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("XXX") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        SceneManager.LoadScene("past"); // ← 請改成你要的場景名稱
    }
}
