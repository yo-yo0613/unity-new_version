using UnityEngine;
using MidiJack;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraZoomControl : MonoBehaviour
{
    public Camera mainCamera; // 用来控制的主相机
    public int zoomControlCC = 18; // MIDI 控制号，对应 nanoKontrol2 的旋钮

    public float zoomSpeed = 10f;  // 缩放速度
    public float minZoom = 9f;    // 最小缩放（对于正交相机，这是最接近的缩放）
    public float maxZoom = 5f;   // 最大缩放（对于正交相机，这是最远的缩放）

    public GameObject fairy;  // 小精灵 GameObject
    public float showTime = 5f;  // 小精灵显示时间（秒）

    public float targetZoom = 6f; // 相机的目标缩放值
    private bool fairyShown = false; // 控制小精灵是否已经显示过

    void Start()
    {
        // 如果没有设置主相机，则使用默认的主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // 默认使用主相机
            Debug.LogWarning("Main Camera 已设置为 Camera.main");
        }

        // 初始化时隐藏小精灵
        if (fairy != null)
        {
            fairy.SetActive(false);  // 隐藏小精灵
        }
    }

    void Update()
    {
        // 获取来自 MIDI 设备（nanoKontrol2）的缩放旋钮的值
        float zoomValue = MidiMaster.GetKnob(zoomControlCC); // 获取旋钮值

        // 基于 MIDI 旋钮的输入，计算目标缩放值（在 minZoom 和 maxZoom 之间进行插值）
        targetZoom = Mathf.Lerp(minZoom, maxZoom, zoomValue); // 在最小和最大缩放值之间进行插值

        // 将相机的正交尺寸平滑过渡到目标缩放值
        if (mainCamera.orthographic) // 确保是正交相机
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }

        // 控制小精灵的显示
        CheckAndShowFairy(zoomValue);
    }

    // 根据旋钮值控制小精灵显示ㄧㄥ
    void CheckAndShowFairy(float zoomValue)
    {

        //Debug.Log("Current zoom value: " + zoomValue);  // 检查当前的 zoomValue

        // 设置你想要的显示阈值
        float threshold = 0.5f; // 例如，当 zoomValue 小于等于 0.5 时才显示小精灵

        // 根据旋钮值控制小精灵显示
        if (zoomValue >= threshold && !fairyShown)
        {
            if (fairy != null)
            {
                fairy.SetActive(true);  // 显示小精灵
                fairyShown = true;  // 标记小精灵已显示
                Debug.Log("显示小精灵");

                // 启动一个协程，5秒后隐藏小精灵并跳转场景
                StartCoroutine(ShowFairyAndTransition());
            }
        }

        // 动态调整小精灵的缩放比例
        if (fairy != null)
        {
            float scale = Mathf.Lerp(0.9f, 1.5f, zoomValue); // 根据 zoomValue 插值计算缩放比例
            fairy.transform.localScale = new Vector3(scale, scale, scale); // 设置小精灵的缩放
        }
    }

    // 小精灵显示并等待5秒后跳转场景
    IEnumerator ShowFairyAndTransition()
    {
        // 等待指定时间
        yield return new WaitForSeconds(showTime);

        if (fairy != null)
        {
            fairy.SetActive(false);  // 隐藏小精灵
        }

        // 可以在这里实现场景跳转的逻辑
        SceneManager.LoadScene("past"); // 例如，加载名为 "PastScene" 的场景
    }
}
