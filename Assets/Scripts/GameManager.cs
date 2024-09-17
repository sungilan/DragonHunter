using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Boss boss;
    [SerializeField] CutSceneManager cutSceneManager;

    void Start()
    {
        // Boss 객체를 찾거나 할당 (필요에 따라 직접 할당하거나 Find로 찾을 수 있음)
        boss = FindObjectOfType<Boss>();

        // 보스가 사망할 때 실행할 메서드 구독
        if (boss != null)
        {
            boss.OnBossDeath += OnBossDeathHandler;
        }
    }

    // 보스 사망 시 실행될 메서드
    private void OnBossDeathHandler()
    {
        Debug.Log("GameManager에서 보스 사망 감지");
        cutSceneManager.PlayLoseCutscene();
        LoadingManager.Instance.LoadScene(2);
    }

    void OnDestroy()
    {
        // 구독 해제 (중요)
        if (boss != null)
        {
            boss.OnBossDeath -= OnBossDeathHandler;
        }
    }
}
