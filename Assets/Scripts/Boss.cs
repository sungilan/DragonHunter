using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class Boss : Enemy
{

    // ��������Ʈ ����
    public delegate void BossDeathHandler();

    // ��������Ʈ�� �̺�Ʈ�� ����
    public event BossDeathHandler OnBossDeath;

    [System.Serializable]
    public struct EffectData
    {
        public GameObject effectPrefab;   // ����Ʈ ������
        public Transform effectPosition;  // ����Ʈ ���� ��ġ
        public float destroyAfterSeconds; // ����Ʈ�� ������������ �ð�
    }

    // ����Ʈ �����͸� ����Ʈ�� ����
    [SerializeField] private List<EffectData> effectDataList;

    [SerializeField] private ParticleSystem fireBreath;
    [SerializeField] private DecalProjector attackRangeDecal;
    [SerializeField] private BoxCollider rangeCollider;

    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook;

    [SerializeField] private BoxCollider meleeArea;
    private NavMeshAgent nav;
    //private Animator anim;
    //private bool isDead;
    [SerializeField] private Transform target;
    [SerializeField] private GameManager gameManager;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    private void OnEnable()
    {
        // ������ Ȱ��ȭ�Ǿ��� �� GameManager�� �˸�
        gameManager.RegisterBoss(this);
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();

            if (OnBossDeath != null)
            {
                OnBossDeath.Invoke();
            }
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);
        int ranAction = Random.Range(0, 4);
        switch (ranAction)
        {
            case 0:
                StartCoroutine(FireBreath());
                break;
            case 1:
                StartCoroutine(TakeDown());
                break;
            case 2:
                StartCoroutine(Meteo());
                break;
            case 3:
                StartCoroutine(Hurricane());
                break;
        }
    }
    IEnumerator FireBreath()
    {
        SoundManager.Instance.PlaySE("�극��");
        anim.SetTrigger("FireBreath");
        // ���� �ִϸ��̼� ���� ���� ��������
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼��� ���� ���̸�ŭ ���
        yield return new WaitForSeconds(stateInfo.length);

        StartCoroutine(Think());
    }
    IEnumerator TakeDown()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        //boxCollider.enabled = false;
        anim.SetTrigger("TakeDown");
        // ���� �ִϸ��̼� ���� ���� ��������
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        // �ִϸ��̼��� ���� ���̸�ŭ ���
        yield return new WaitForSeconds(stateInfo.length - 1f);
        isLook = true;
        nav.isStopped = true;
        //boxCollider.enabled = true;

        StartCoroutine(Think());
    }
    IEnumerator Meteo()
    {
        tauntVec = target.position + lookVec;

        anim.SetTrigger("Meteo");
        // ���� �ִϸ��̼� ���� ���� ��������
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼��� ���� ���̸�ŭ ���
        yield return new WaitForSeconds(stateInfo.length);

        StartCoroutine(Think());
    }

    IEnumerator Hurricane()
    {
        anim.SetTrigger("Hurricane");
        // ���� �ִϸ��̼� ���� ���� ��������
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼��� ���� ���̸�ŭ ���
        yield return new WaitForSeconds(stateInfo.length);
        StartCoroutine(Think());
    }
    public void ToggleFireBreath(int isActive)
    {
        bool active = isActive > 0;
        fireBreath.gameObject.SetActive(active);
        rangeCollider.enabled = active;
    }
    public void ToggleAttackRange(int isActive)
    {
        bool active = isActive > 0;
        attackRangeDecal.gameObject.SetActive(active);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus playerHealth = other.GetComponentInParent<PlayerStatus>();
            int damage = Random.Range(minAtk, maxAtk);

            bool isCriticalHit = Random.value < criticalHitChance;

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, isCriticalHit);
            }
        }
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("Player"))
    //    {
    //        PlayerStatus playerHealth = other.GetComponentInParent<PlayerStatus>();

    //        if (playerHealth != null)
    //            {
    //                StartCoroutine(DealMultipleHits(playerHealth));
    //            }
    //    }
    //}
    //private IEnumerator DealMultipleHits(PlayerStatus playerHealth)
    //{
    //    int damage = Random.Range(minAtk, maxAtk);

    //    bool isCriticalHit = Random.value < criticalHitChance;

    //    yield return new WaitForSeconds(1f);
    //    for (int i = 0; i < 5; i++)
    //    {
    //        if (isCriticalHit)
    //        {
    //            damage = (int)(damage * criticalHitMultiplier);
    //            playerHealth.TakeDamage(damage, true);  // ġ��Ÿ ����
    //        }
    //        else
    //        {
    //            playerHealth.TakeDamage(damage, false);  // �Ϲ� ������ ����
    //        }

    //        yield return new WaitForSeconds(1f);  // �� Ÿ�� ������ ����
    //    }
    //}
    protected override IEnumerator TakeDamageCorutine(int _dmg, bool isCritical)
    {
        if (isDead) yield break;
        isStun = true;
        agent.isStopped = true;

        if (currentHp > 0)
        {
            worldCanvasController.AddDamageText(transform.position + new Vector3(0, 1.5f, 0), _dmg, isCritical);
            hpBarCanvas.gameObject.SetActive(true);
            currentHp -= _dmg;
            if (currentHp <= 0)
            {
                StartCoroutine(Death());  // ü���� 0 ������ �� ��� ó��
                yield break;
            }
            int _random = Random.Range(0, sound_Hurt.Length);
            SoundManager.Instance.PlaySE(sound_Hurt[_random]);
            anim.SetTrigger("Hurt");
            yield return new WaitForSeconds(1f);
            isStun = false;
            agent.isStopped = false;
            hpBarCanvas.gameObject.SetActive(false);
        }
    }

    protected override IEnumerator Death()
    {
        // ���� ���� ��� ó�� ����
        Sprite coin = Resources.Load<Sprite>("Coin");
        worldCanvasController.DropItem(transform.position + new Vector3(0, -0.5f, 0), coin);
        currentHp = 0;
        agent.enabled = false;
        isWalking = false;
        isRunning = false;

        // ��� �̺�Ʈ ȣ��
        if (OnBossDeath != null)
        {
            OnBossDeath.Invoke();
        }

        int _random = Random.Range(0, sound_Death.Length);
        SoundManager.Instance.PlaySE(sound_Death[_random]);
        anim.SetTrigger("Death");
        isDead = true;

        hpBarCanvas.gameObject.SetActive(false);

        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    public void MakeEffect(int effectIndex)
    {
        if (effectIndex < 0 || effectIndex >= effectDataList.Count)
            return;

        EffectData data = effectDataList[effectIndex];
        GameObject gm = Instantiate(data.effectPrefab, data.effectPosition.position, data.effectPosition.rotation);
        gm.transform.parent = this.transform;
        StartCoroutine(DestroyEffectAfterTime(gm, data.destroyAfterSeconds));
    }

    // ���� �ð� �� ����Ʈ ����
    private IEnumerator DestroyEffectAfterTime(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(effect);
    }
}