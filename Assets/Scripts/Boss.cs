using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class Boss : Enemy
{

    // 델리게이트 선언
    public delegate void BossDeathHandler();

    // 델리게이트를 이벤트로 선언
    public event BossDeathHandler OnBossDeath;

    [System.Serializable]
    public struct EffectData
    {
        public GameObject effectPrefab;   // 이펙트 프리팹
        public Transform effectPosition;  // 이펙트 생성 위치
        public float destroyAfterSeconds; // 이펙트가 사라지기까지의 시간
    }

    // 이펙트 데이터를 리스트로 관리
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
        // 보스가 활성화되었을 때 GameManager에 알림
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
        SoundManager.Instance.PlaySE("브레스");
        anim.SetTrigger("FireBreath");
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션의 실제 길이만큼 대기
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
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        // 애니메이션의 실제 길이만큼 대기
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
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션의 실제 길이만큼 대기
        yield return new WaitForSeconds(stateInfo.length);

        StartCoroutine(Think());
    }

    IEnumerator Hurricane()
    {
        anim.SetTrigger("Hurricane");
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션의 실제 길이만큼 대기
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
    //            playerHealth.TakeDamage(damage, true);  // 치명타 적용
    //        }
    //        else
    //        {
    //            playerHealth.TakeDamage(damage, false);  // 일반 데미지 적용
    //        }

    //        yield return new WaitForSeconds(1f);  // 각 타격 사이의 간격
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
                StartCoroutine(Death());  // 체력이 0 이하일 때 사망 처리
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
        // 보스 전용 사망 처리 로직
        Sprite coin = Resources.Load<Sprite>("Coin");
        worldCanvasController.DropItem(transform.position + new Vector3(0, -0.5f, 0), coin);
        currentHp = 0;
        agent.enabled = false;
        isWalking = false;
        isRunning = false;

        // 사망 이벤트 호출
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

    // 일정 시간 후 이펙트 삭제
    private IEnumerator DestroyEffectAfterTime(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(effect);
    }
}