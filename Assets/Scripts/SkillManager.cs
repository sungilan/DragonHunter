using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : Singleton<SkillManager>
{
    public Animator anim;
    [SerializeField] private Button attackBtn;
    [SerializeField] private Button[] skillBtns;
    [SerializeField] private Image[] skillIcons = new Image[3];
    [SerializeField] private GameObject cooltext;
    private Coroutine[] coolCoroutines;
    public Skill[] skillSet = new Skill[3];

    private bool comboPossible;
    private int comboStep;
    [SerializeField] private BoxCollider weaponCol;

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
                SoundManager.instance.PlaySE("테잇");
                anim.Play("NormalAttack_B");
            }
        if (comboStep == 3)
            {
                SoundManager.instance.PlaySE("테잇");
                anim.Play("NormalAttack_C");
            }
        if (comboStep == 4)
            {
                SoundManager.instance.PlaySE("토우");
                anim.Play("NormalAttack_D");
            }
        if (comboStep == 5)
            {
                SoundManager.instance.PlaySE("필살");
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
            SoundManager.instance.PlaySE("이얏");
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
    }
    public void AttackEnd()
    {
        weaponCol.enabled = false;
    }
    public void PlayAttackSound()
    {
        SoundManager.instance.PlaySE("검");
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

        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
            skillIcons[_skillNum].fillAmount = Mathf.Clamp01(elapsedTime / cooldown);
            yield return null;
        }

        skillIcons[_skillNum].fillAmount = 1f;
        coolCoroutines[_skillNum] = null; // 쿨타임 완료 후 코루틴 참조 제거
    }

    public Skill[] GetSkill(int _type)
    {
        // 스킬 초기화 예시
        switch (_type)
        {
            case 0:
                skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "닌자도");
                skillSet[1] = new DashAttack(6f, 6f, 4f, "수리검");
                skillSet[2] = new UltimateAttack(6f, 5f, 6f, "돌던지기");
                SKillIconSet(0);
                Debug.Log(skillSet[0].skillName);
                Debug.Log(skillSet[1].skillName);
                Debug.Log(skillSet[2].skillName);
                break;
            case 1:
                skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "쿠나이");
                //skillSet[1] = new TalktoEnemy(1.2f, 0f, 4f, "변장");
                //skillSet[2] = new ThrowSomething(6f, 3f, 6f, 1, "재채기분말");
                SKillIconSet(1);
                break;
            case 2:
                skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "일본도");
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
        cooltext.SetActive(true);
        yield return new WaitForSeconds(1f);
        cooltext.SetActive(false);
    }
}
public abstract class Skill
{
    [Header("skill info")]
    public string skillName;
    public float skillRange;
    public float soundRange;
    public float skillCool;
    protected float lastUsedTime;
    protected TPSCharacterController caster;

    public Skill(float skillRange, float soundRange, float skillCool, string skillName)
    {
        this.skillName = skillName;
        this.skillRange = skillRange;
        this.soundRange = soundRange;
        this.skillCool = skillCool;
        this.lastUsedTime = -skillCool; // 처음부터 스킬을 사용할 수 있도록 설정
        caster = DataManager.Instance.player;
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
    public abstract void ApproachUseSkill();
}

public class MeleeAttack : Skill
{
    public MeleeAttack(float skillRange, float soundRange, float skillCool, string skillName)
        : base(skillRange, soundRange, skillCool, skillName) { }

    public override void UseSkill()
    {
        if (IsOffCooldown())
        {
            Debug.Log("MeleeAttack skill used!");
            SoundManager.instance.PlaySE("테잇");
            SkillManager.Instance.anim.SetTrigger("Attack");
            SkillManager.Instance.SkillCool(0);
        }
        else
            Debug.Log("스킬 쿨타임 중");
    }

    public override void ApproachUseSkill()
    {
        if (IsOffCooldown())
            Debug.Log("스킬 사용 가능");
        else
            Debug.Log("스킬 쿨타임 중");
    }
}

public class DashAttack : Skill
{
    public DashAttack(float skillRange, float soundRange, float skillCool, string skillName)
        : base(skillRange, soundRange, skillCool, skillName) { }

    public override void UseSkill()
    {
        Debug.Log("DashAttack skill used!");
        SoundManager.instance.PlaySE("토우");
        SkillManager.Instance.anim.SetTrigger("Attack");
        SkillManager.Instance.SkillCool(1);
    }

    public override void ApproachUseSkill()
    {
        if (IsOffCooldown())
            Debug.Log("스킬 사용 가능");
        else
            Debug.Log("스킬 쿨타임 중");
    }
}
public class UltimateAttack : Skill
{
    public UltimateAttack(float skillRange, float soundRange, float skillCool, string skillName)
        : base(skillRange, soundRange, skillCool, skillName) { }

    public override void UseSkill()
    {
        Debug.Log("UltimateAttack skill used!");
        SkillManager.Instance.StartCoroutine(OneShot());
        SkillManager.Instance.SkillCool(2);
    }
    IEnumerator OneShot()
    {
        SoundManager.instance.PlaySE("필살");
        SkillManager.Instance.anim.SetTrigger("Slash");
        Transform mon = GameObject.FindGameObjectWithTag("Monster").transform;
        Vector3 dir = mon.position - caster.transform.position;
        dir.y = 0;
        dir.Normalize();
        Vector3 targetPos = mon.position + (dir * 1.2f);
        caster.transform.forward = dir;
        yield return new WaitForSecondsRealtime(1f);
        caster.transform.position = targetPos;
    }

    public override void ApproachUseSkill()
    {
        if (IsOffCooldown())
            Debug.Log("스킬 사용 가능");
        else
            Debug.Log("스킬 쿨타임 중");
    }
}

