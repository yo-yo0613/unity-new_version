using UnityEngine;
using MidiJack;
using System.Collections;

public class MusicPadController : MonoBehaviour
{
    [System.Serializable]
    public class MusicPad
    {
        public int noteNumber;
        public AudioClip clip;
        public bool loop = true;
        [HideInInspector] public AudioSource source;
        [HideInInspector] public Coroutine currentCoroutine;
    }

    public MusicPad[] pads;
    public float fadeDuration = 1f;  // 淡入淡出時間（秒）

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
            pad.source.volume = 0f; // 預設為 0
        }
    }

    void Update()
    {
        foreach (var pad in pads)
        {
            if (MidiMaster.GetKnob(pad.noteNumber) > 0.5f)
            {
                if (pad.currentCoroutine != null)
                {
                    StopCoroutine(pad.currentCoroutine);
                }

                if (!pad.source.isPlaying)
                {
                    pad.source.volume = 0f;
                    pad.source.Play();
                    Debug.Log("開始播放 " + pad.clip.name);
                    pad.currentCoroutine = StartCoroutine(FadeVolume(pad.source, 0f, 1f));
                }

                else
                {
                    pad.currentCoroutine = StartCoroutine(FadeOutAndStop(pad));
                }
            }
        }

        for (int i = 0; i < 128; i++) // MIDI note 範圍通常 0~127
        {
            if (MidiMaster.GetKeyDown(i))
            {
                Debug.Log($"Detected MIDI key down: {i}");
            }
        }
    }

    IEnumerator FadeVolume(AudioSource source, float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / fadeDuration);
            //Debug.Log("volume: " + source.volume);  // ← 加這行
            yield return null;
        }
        source.volume = to;
    }


    IEnumerator FadeOutAndStop(MusicPad pad)
    {
        float startVolume = pad.source.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            pad.source.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        pad.source.volume = 0f;
        pad.source.Stop();
    }
}
