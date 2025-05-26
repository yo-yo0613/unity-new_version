using UnityEngine;

public class ParallaxWithFloat : MonoBehaviour
{
    [Header("視差設定")]
    public Transform targetCamera;          // 主相機
    public Vector3 positionOffset;          // 物件相對位置偏移（視差用）

    [Header("Y 軸浮動設定")]
    public float floatRange = 1.0f;         // Y 浮動幅度
    public float floatSpeed = 1.0f;         // 浮動速度

    private float baseY;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main.transform;
        }

        // 紀錄初始 Y
        baseY = transform.position.y;
    }

    void Update()
    {
        // 基本視差位置（你原本的相機驅動位置邏輯）
        Vector3 targetPos = targetCamera.position + positionOffset;

        // 加上 Y 軸浮動
        targetPos.y = baseY + Mathf.Sin(Time.time * floatSpeed) * floatRange;

        // 設定物件位置
        transform.position = targetPos;
    }
}
