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
    private bool hasPlayedCutscene = false;  // Ÿ�Ӷ����� �̹� ����Ǿ����� Ȯ���ϴ� ����

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
        hasPlayedCutscene = true; // Ÿ�Ӷ����� ����Ǿ����� ���
    }
    public void PlayLoseCutscene()
    {
        pd.Play(ta[1]);
        hasPlayedCutscene = true; // Ÿ�Ӷ����� ����Ǿ����� ���
    }
    public void PlayBossCutscene()
    {
        pd.Play(ta[2]);
        hasPlayedCutscene = true; // Ÿ�Ӷ����� ����Ǿ����� ���
    }

    // �ʿ��ϴٸ� �ٸ� �̺�Ʈ�� ���� Ÿ�Ӷ����� ����ϰų� �����ϴ� �޼��� �߰� ����
    public void ResetCutscene()
    {
        hasPlayedCutscene = false;
    }
}
