using UnityEngine;

public class ForceAnimatorLoop : MonoBehaviour
{
    public string animationName = "water-_00000"; // 狀態名稱
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // 如果動畫播放完，就重播
        if (state.IsName(animationName) && state.normalizedTime >= 1f)
        {
            animator.Play(animationName, 0, 0f); // 重播此動畫
        }
    }
}
