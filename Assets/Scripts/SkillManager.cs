using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : Singleton<SkillManager>
{
    public Animator anim;
    [SerializeField] private Button attackBtn;
    [SerializeField] private Button[] skillBtns;
    [SerializeField] private Image[] skillIcons = new Image[3];
    [SerializeField] private GameObject coolDowntext;
    [SerializeField] private TextMeshProUGUI[] coolCount = new TextMeshProUGUI[3];
    private Coroutine[] coolCoroutines;
    public Skill[] skillSet = new Skill[3];

    private bool comboPossible;
    private int comboStep;
    [SerializeField] private BoxCollider weaponCol;
    
    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

    private void Awake()
    {
        GetSkill(0);
        coolCoroutines = new Coroutine[skillIcons.Length]; // 각 스킬 아이콘에 대해 쿨타임을 관리할 코루틴 배열 초기화

        for (int i = 0; i < skillBtns.Length; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 임시 변수 사용
            skillBtns[i].onClick.AddListener(() => UseSkill(index));
        }
        attackBtn.onClick.AddListener(() => NormalAttack());
    }
    // 애니메이션 이벤트에 추가할 함수들

    // 콤보의 시작점(콤보 입력이 가능하게 해줌)
    public void ComboPossible()
    {
        comboPossible = true;
    }
    public void NextAtk()
    {
        if (comboStep == 2)
            {
                anim.Play("NormalAttack_B");
            }
        if (comboStep == 3)
            {
                SoundManager.Instance.PlaySE("토우");
                anim.Play("NormalAttack_C");
            }
        if (comboStep == 4)
            {
                anim.Play("NormalAttack_D");
            }
        if (comboStep == 5)
            {
                anim.Play("NormalAttack_E");
            }
    }
    // 콤보 단계를 초기화 해주는 함수
    public void ResetCombo()
    {
        comboPossible = false;
        comboStep = 0;
    }

    // 기본 공격 기능
    private void NormalAttack()
    {
        if (comboStep == 0)
        {
            // 최초 입력시 첫번째 공격 애니메이션을 재생
            SoundManager.Instance.PlaySE("이얏");
            anim.Play("NormalAttack_A");
            comboStep = 1;
            return;
        }
        if (comboStep != 0)
        {
            if (comboPossible)
            {
                // 무차별 입력을 방지하기 위해 false로 해줌
                comboPossible = false;
                comboStep += 1;
            }
        }
    }
    public void AttackStart()
    {
        weaponCol.enabled = true;
        damagedEnemies.Clear();
    }
    public void AttackEnd()
    {
        weaponCol.enabled = false;
    }
    public void PlayAttackSound(string attackSound)
    {
        SoundManager.Instance.PlaySE(attackSound);
    }

    void UseSkill(int index)
    {
        if (index < 0 || index >= skillSet.Length)
        {
            Debug.Log("Invalid skill index.");
            return;
        }

        Skill skill = skillSet[index];
        if (skill != null)
        {
            skill.UseSkillIfReady();
            SkillCool(index);
        }
        else
        {
            Debug.Log($"Skill {index} is null.");
        }
    }

    public void SkillCool(int _skillNum)
    {
        if (_skillNum < 0 || _skillNum >= skillIcons.Length)
        {
            Debug.Log("Invalid skill number.");
            return;
        }

        // 쿨타임 코루틴이 이미 진행 중이면 기존 코루틴의 fillAmount만 업데이트
        if (coolCoroutines[_skillNum] != null)
        {
            // 재설정 필요 없으므로 return
            return;
        }

        skillIcons[_skillNum].fillAmount = 0; // 쿨타임 시작 시 이미지 초기화
        coolCoroutines[_skillNum] = StartCoroutine(FillCool(_skillNum));
    }

    private IEnumerator FillCool(int _skillNum)
    {
        float cooldown = skillSet[_skillNum].skillCool;
        float elapsedTime = 0f;

        // 쿨타임 텍스트 활성화
        coolCount[_skillNum].gameObject.SetActive(true);

        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
            skillIcons[_skillNum].fillAmount = Mathf.Clamp01(elapsedTime / cooldown);

            // 남은 시간을 계산하여 텍스트로 표시
            float remainingTime = cooldown - elapsedTime;
            coolCount[_skillNum].text = ((int)remainingTime).ToString();

            yield return null;
        }

        skillIcons[_skillNum].fillAmount = 1f;
        coolCount[_skillNum].text = "";  // 쿨타임이 끝났을 때 텍스트 지우기
        coolCount[_skillNum].gameObject.SetActive(false);  // 쿨타임 텍스트 비활성화
        coolCoroutines[_skillNum] = null; // 쿨타임 완료 후 코루틴 참조 제거
    }

    public Skill[] GetSkill(int _type)
    {
        // 스킬 초기화 예시
        switch (_type)
        {
            case 0:
                skillSet[0] = new Dash(1.2f, 3f, 2f, "대쉬");
                skillSet[1] = new SpinAttack(6f, 30f, 4f, "회천");
                skillSet[2] = new UltimateAttack(6f, 5f, 6f, "벽력일섬");
                SKillIconSet(0);
                Debug.Log(skillSet[0].skillName);
                Debug.Log(skillSet[1].skillName);
                Debug.Log(skillSet[2].skillName);
                break;
            case 1:
                skillSet[0] = new Dash(1.2f, 3f, 2f, "쿠나이");
                //skillSet[1] = new TalktoEnemy(1.2f, 0f, 4f, "변장");
                //skillSet[2] = new ThrowSomething(6f, 3f, 6f, 1, "재채기분말");
                SKillIconSet(1);
                break;
            case 2:
                skillSet[0] = new Dash(1.2f, 3f, 2f, "일본도");
                //skillSet[1] = new SlashBlade(2f, 3f, 18f, "바람가르기");
                //skillSet[2] = new ThrowSomething(6f, 0f, 0f, 2, "술병");
                SKillIconSet(2);
                break;
            default:
                Debug.Log("잘못된 스킬 타입입니다.");
                break;
        }
        return skillSet;
    }

    private void SKillIconSet(int _type)
    {
        for (int i = 0; i < skillIcons.Length; i++)
        {
            skillIcons[i].sprite = Resources.Load<Sprite>($"Skill_Icon{_type}_{i}");
            skillIcons[i].fillAmount = 1;
        }
    }
    public IEnumerator CoolText()
    {
        coolDowntext.SetActive(true);
        yield return new WaitForSeconds(1f);
        coolDowntext.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            int damage = Random.Range(DataManager.Instance.minAtk, DataManager.Instance.maxAtk);

            bool isCriticalHit = Random.value < DataManager.Instance.criticalHitChance;
            
            // 적이 이미 데미지를 받았는지 확인
            if (!damagedEnemies.Contains(enemy))
            {
                if(isCriticalHit)
                {
                    damage = (int)(damage * DataManager.Instance.criticalHitMultiplier);
                    enemy.TakeDamage(damage, true);
                }
                else
                {
                    enemy.TakeDamage(damage, false);
                }
                damagedEnemies.Add(enemy);
            }
        }
    }
}
public abstract class Skill
{
    [Header("skill info")]
    public string skillName;
    public float skillRange;
    public float skillDamage;
    public float skillCool;
    protected float lastUsedTime;
    protected Transform caster;

    public Skill(float skillRange, float skillDamage, float skillCool, string skillName)
    {
        this.skillName = skillName;
        this.skillRange = skillRange;
        this.skillDamage = skillDamage;
        this.skillCool = skillCool;
        this.lastUsedTime = -skillCool; // 처음부터 스킬을 사용할 수 있도록 설정
        caster = DataManager.Instance.player.characterBody;
    }

    public bool IsOffCooldown()
    {
        return Time.time >= lastUsedTime + skillCool;
    }
    public void UseSkillIfReady()
    {
        if (IsOffCooldown())
        {
            UseSkill();
            lastUsedTime = Time.time;
        }
        else
        {
            SkillManager.Instance.StartCoroutine(SkillManager.Instance.CoolText());
            Debug.Log($"{skillName} is on cooldown.");
        }
    }

    public abstract void UseSkill();
}

public class Dash : Skill
{
    private float dashDistance = 5f;  // 대쉬할 거리
    private float dashSpeed = 10f;    // 대쉬 속도
    public Dash(float skillRange, float soundRange, float skillCool, string skillName)
        : base(skillRange, soundRange, skillCool, skillName) { }

    public override void UseSkill()
    {
        if (IsOffCooldown())
        {
            Debug.Log("Dash skill used!");
            SoundManager.Instance.PlaySE("오소이");
            SkillManager.Instance.anim.SetTrigger("Dash");

            // 현재 캐릭터가 바라보고 있는 방향으로 대쉬
            Vector3 dashDirection = caster.forward;
            Vector3 targetPosition = caster.position + dashDirection * dashDistance;

            // 대쉬 시작
            SkillManager.Instance.StartCoroutine(DashMovement(targetPosition));

            SkillManager.Instance.SkillCool(0);
        }
        else
            Debug.Log("스킬 쿨타임 중");
    }

    private IEnumerator DashMovement(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = caster.position;
        float dashDuration = dashDistance / dashSpeed;  // 대쉬할 총 시간

        while (elapsedTime < dashDuration)
        {
            // 선형 보간(Lerp)을 사용하여 캐릭터를 서서히 이동
            caster.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 대쉬 끝에 정확하게 목표 위치에 도착
        caster.position = targetPosition;
    }
}

public class SpinAttack : Skill
{
    public int hitCount = 5;  // 타격 횟수
    public float hitInterval = 0.5f;  // 타격 간격
    public SpinAttack(float skillRange, float skillDamage, float skillCool, string skillName)
        : base(skillRange, skillDamage, skillCool, skillName) { }

    public override void UseSkill()
    {
        Debug.Log("DashAttack skill used!");
        SoundManager.Instance.PlaySE("오와리");
        SkillManager.Instance.anim.SetTrigger("360Attack");
        Collider[] colls = Physics.OverlapSphere(caster.position, skillRange);
        foreach (Collider coll in colls)
        {
            int damage = Random.Range(DataManager.Instance.minAtk + (int)skillDamage, DataManager.Instance.maxAtk + (int)skillDamage);
            bool isCriticalHit = Random.value < DataManager.Instance.criticalHitChance;

            Enemy enemy = coll.GetComponent<Enemy>();
            if (enemy != null)
            {
                SkillManager.Instance.StartCoroutine(DealMultipleHits(enemy));
            }
        }
        SkillManager.Instance.SkillCool(1);
    }
    private IEnumerator DealMultipleHits(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < hitCount; i++)
        {
            // 데미지 계산
            int damage = Random.Range(DataManager.Instance.minAtk + (int)skillDamage, DataManager.Instance.maxAtk + (int)skillDamage);
            bool isCriticalHit = Random.value < DataManager.Instance.criticalHitChance;

            if (isCriticalHit)
            {
                damage = (int)(damage * DataManager.Instance.criticalHitMultiplier);
                enemy.TakeDamage(damage, true);  // 치명타 적용
            }
            else
            {
                enemy.TakeDamage(damage, false);  // 일반 데미지 적용
            }

            yield return new WaitForSeconds(hitInterval);  // 각 타격 사이의 간격
        }
    }
}
public class UltimateAttack : Skill
{
    private float dashDistance = 5f;  // 대쉬할 거리
    public float dashDamageRadius = 1f; // 대쉬 경로의 폭

    public UltimateAttack(float skillRange, float soundRange, float skillCool, string skillName)
        : base(skillRange, soundRange, skillCool, skillName) { }

    public override void UseSkill()
    {
        if (IsOffCooldown())
        {
            Debug.Log("Dash skill used!");
            SkillManager.Instance.StartCoroutine(DashMovement());
            SkillManager.Instance.SkillCool(2);
        }
        else
            Debug.Log("스킬 쿨타임 중");
    }

    private IEnumerator DashMovement()
    {
        SoundManager.Instance.PlaySE("필살");
        SkillManager.Instance.anim.SetTrigger("Slash");
        // 대쉬 시작 위치
        Vector3 startPosition = caster.position;
        // 대쉬 목표 위치
        Vector3 dashDirection = caster.forward;
        Vector3 targetPosition = startPosition + dashDirection * dashDistance;
        yield return new WaitForSeconds(1f);
        caster.position = targetPosition;
        // 대쉬 경로에서 적에게 데미지를 입힙니다.
        DealDamageAlongPath(startPosition, targetPosition);
        
    }

    private void DealDamageAlongPath(Vector3 startPosition, Vector3 endPosition)
    {
        // 대쉬 경로에 있는 적들을 감지합니다.
        RaycastHit[] hits = Physics.SphereCastAll(startPosition, dashDamageRadius, (endPosition - startPosition).normalized, Vector3.Distance(startPosition, endPosition));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int damage = Random.Range(DataManager.Instance.minAtk, DataManager.Instance.maxAtk);
                    bool isCriticalHit = Random.value < DataManager.Instance.criticalHitChance;

                    if (isCriticalHit)
                    {
                        damage = (int)(damage * DataManager.Instance.criticalHitMultiplier);
                        enemy.TakeDamage(damage, true);
                    }
                    else
                    {
                        enemy.TakeDamage(damage, false);
                    }
                }
            }
        }
    }
}



