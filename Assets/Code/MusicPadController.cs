using UnityEngine;
using MidiJack;

public class MusicPadController : MonoBehaviour
{
    [System.Serializable]
    public class MusicPad
    {
        public int noteNumber;            // nanoPAD2 pad 編號（MIDI Note）
        public AudioClip clip;            // 音樂片段
        public bool loop = true;          // 是否循環播放
        [HideInInspector] public AudioSource source;
    }

    public MusicPad[] pads;

    void Start()
    {
        foreach (var pad in pads)
        {
            GameObject audioObj = new GameObject("Audio_" + pad.noteNumber);
            audioObj.transform.parent = this.transform;
            pad.source = audioObj.AddComponent<AudioSource>();
            pad.source.clip = pad.clip;
            pad.source.loop = pad.loop;
            pad.source.playOnAwake = false;
        }
    }

    void Update()
    {
        foreach (var pad in pads)
        {
            if (MidiMaster.GetKeyDown(pad.noteNumber))
            {
                if (pad.source.isPlaying)
                {
                    pad.source.Stop(); // 再按一次就停止
                }
                else
                {
                    pad.source.Play();  // 播放自己，不管其他人
                }
            }
        }

        // 除錯：印出所有按下的 note
        for (int i = 0; i < 128; i++)
        {
            if (MidiMaster.GetKeyDown(i))
                Debug.Log("Pressed Note: " + i);
        }
    }
}
