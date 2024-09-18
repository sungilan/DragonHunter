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
            boss.OnBossDeath -= OnBossDeathHandler;  // ���� ������ ���� ����
        }

        boss = newBoss;
        boss.OnBossDeath += OnBossDeathHandler;  // �� ������ ��� �̺�Ʈ ����
        isBossDead = false;
    }

    // ���� ��� �� ����� �޼���
    private void OnBossDeathHandler()
    {
        if (isBossDead) return;
        Debug.Log("GameManager���� ���� ��� ����");
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
        // ���� ���� (�߿�)
        if (boss != null)
        {
            boss.OnBossDeath -= OnBossDeathHandler;
        }
    }
}
