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

    [SerializeField] private ParticleSystem fireBreath;
    [SerializeField] private DecalProjector attackRangeDecal;
    [SerializeField] private BoxCollider rangeCollider;

    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    [SerializeField] private BoxCollider meleeArea;
    private NavMeshAgent nav;
    //private Animator anim;
    //private bool isDead;
    [SerializeField] private Transform target;

    void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        //boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
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
        Debug.Log("생각중");
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
        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }
    IEnumerator TakeDown()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        //boxCollider.enabled = false;
        anim.SetTrigger("TakeDown");
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        //boxCollider.enabled = true;

        StartCoroutine(Think());
    }
    IEnumerator Meteo()
    {
        tauntVec = target.position + lookVec;

        anim.SetTrigger("Meteo");
        yield return new WaitForSeconds(1f);

        StartCoroutine(Think());
    }

    IEnumerator Hurricane()
    {
        anim.SetTrigger("Hurricane");
        yield return new WaitForSeconds(1.5f);
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
        if(other.CompareTag("Player"))
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
}