using UnityEngine;
using UnityEngine.SceneManagement;
using MidiJack;

public class SwitchScene : MonoBehaviour
{
    void Update()
    {
        // 检测按下空格键
        if (MidiMaster.GetKeyDown(50))
        {
            // 切换到名为 "1" 的场景
            SceneManager.LoadScene("1");
        }
    }
}
