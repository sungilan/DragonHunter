using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class CutSceneManager : MonoBehaviour
{
    private PlayableDirector pd;
    public TimelineAsset[] ta;

    public bool isWin;
    public bool isLose;
    private bool hasPlayedCutscene = false;  // 타임라인이 이미 재생되었는지 확인하는 변수

    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWin && !hasPlayedCutscene)
        {
            PlayWinCutscene();
        }
        if (isLose && !hasPlayedCutscene)
        {
            PlayLoseCutscene();
        }
    }

    public void PlayWinCutscene()
    {
        pd.Play(ta[0]);
        hasPlayedCutscene = true; // 타임라인이 재생되었음을 기록
    }
    public void PlayLoseCutscene()
    {
        pd.Play(ta[1]);
        hasPlayedCutscene = true; // 타임라인이 재생되었음을 기록
    }
    public void PlayBossCutscene()
    {
        pd.Play(ta[2]);
        hasPlayedCutscene = true; // 타임라인이 재생되었음을 기록
    }

    // 필요하다면 다른 이벤트에 따라 타임라인을 재생하거나 리셋하는 메서드 추가 가능
    public void ResetCutscene()
    {
        hasPlayedCutscene = false;
    }
}
