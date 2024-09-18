using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Boss boss;
    [SerializeField] CutSceneManager cutSceneManager;
    [SerializeField] private bool isBossDead = false;

    public void RegisterBoss(Boss newBoss)
    {
        if (boss != null)
        {
            boss.OnBossDeath -= OnBossDeathHandler;  // 기존 보스의 구독 해제
        }

        boss = newBoss;
        boss.OnBossDeath += OnBossDeathHandler;  // 새 보스의 사망 이벤트 구독
        isBossDead = false;
    }

    // 보스 사망 시 실행될 메서드
    private void OnBossDeathHandler()
    {
        if (isBossDead) return;
        Debug.Log("GameManager에서 보스 사망 감지");
        isBossDead = true;
        StartCoroutine(BossDeath());
    }
    IEnumerator BossDeath()
    {
        cutSceneManager.PlayWinCutscene();
        yield return new WaitForSeconds(5f);
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
