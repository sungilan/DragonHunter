using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Boss boss;
    [SerializeField] CutSceneManager cutSceneManager;

    void Start()
    {
        // Boss ��ü�� ã�ų� �Ҵ� (�ʿ信 ���� ���� �Ҵ��ϰų� Find�� ã�� �� ����)
        boss = FindObjectOfType<Boss>();

        // ������ ����� �� ������ �޼��� ����
        if (boss != null)
        {
            boss.OnBossDeath += OnBossDeathHandler;
        }
    }

    // ���� ��� �� ����� �޼���
    private void OnBossDeathHandler()
    {
        Debug.Log("GameManager���� ���� ��� ����");
        cutSceneManager.PlayLoseCutscene();
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
