using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 어떤 씬에서 어떤 BGM을 재생할지 결정하는 클래스
// 예외적으로 프롤로그, 스토리 관련 씬은 해당 씬의 TimeLine이 BGM을 재생함.
public class BGMSelector : MonoBehaviour
{
    [Serializable]
    public struct BGMInfo
    {
        public string sceneName;
        public AudioClip bgm;
    }

    [SerializeField] private List<BGMInfo> bgmList = new List<BGMInfo>();

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
        // 이 컴포넌트가 생성된 씬에 해당하는 BGM을 찾아 재생(기본적으로 StartScene에 BGM을 틀기 위함)
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneChanged(currentScene, LoadSceneMode.Single);
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        // 현재 씬에 해당하는 BGM을 찾아 재생
        foreach (var bgmInfo in bgmList)
        {
            if (bgmInfo.sceneName == scene.name)
            {
                SoundManager.Instance.Play(bgmInfo.bgm, Sound.Bgm);
                return;
            }
        }
    }
}
