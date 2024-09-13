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
        coolCoroutines = new Coroutine[skillIcons.Length]; // �� ��ų �����ܿ� ���� ��Ÿ���� ������ �ڷ�ƾ �迭 �ʱ�ȭ

        for (int i = 0; i < skillBtns.Length; i++)
        {
            int index = i; // Ŭ���� ������ ���ϱ� ���� �ӽ� ���� ���
            skillBtns[i].onClick.AddListener(() => UseSkill(index));
        }
        attackBtn.onClick.AddListener(() => NormalAttack());
    }
    // �ִϸ��̼� �̺�Ʈ�� �߰��� �Լ���

    // �޺��� ������(�޺� �Է��� �����ϰ� ����)
    public void ComboPossible()
    {
        comboPossible = true;
    }
    public void NextAtk()
    {
        if (comboStep == 2)
            {
                SoundManager.instance.PlaySE("����");
                anim.Play("NormalAttack_B");
            }
        if (comboStep == 3)
            {
                SoundManager.instance.PlaySE("����");
                anim.Play("NormalAttack_C");
            }
        if (comboStep == 4)
            {
                SoundManager.instance.PlaySE("���");
                anim.Play("NormalAttack_D");
            }
        if (comboStep == 5)
            {
                SoundManager.instance.PlaySE("�ʻ�");
                anim.Play("NormalAttack_E");
            }
    }
    // �޺� �ܰ踦 �ʱ�ȭ ���ִ� �Լ�
    public void ResetCombo()
    {
        comboPossible = false;
        comboStep = 0;
    }

    // �⺻ ���� ���
    private void NormalAttack()
    {
        if (comboStep == 0)
        {
            // ���� �Է½� ù��° ���� �ִϸ��̼��� ���
            SoundManager.instance.PlaySE("�̾�");
            anim.Play("NormalAttack_A");
            comboStep = 1;
            return;
        }
        if (comboStep != 0)
        {
            if (comboPossible)
            {
                // ������ �Է��� �����ϱ� ���� false�� ����
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
        SoundManager.instance.PlaySE("��");
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

        // ��Ÿ�� �ڷ�ƾ�� �̹� ���� ���̸� ���� �ڷ�ƾ�� fillAmount�� ������Ʈ
        if (coolCoroutines[_skillNum] != null)
        {
            // �缳�� �ʿ� �����Ƿ� return
            return;
        }

        skillIcons[_skillNum].fillAmount = 0; // ��Ÿ�� ���� �� �̹��� �ʱ�ȭ
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
        coolCoroutines[_skillNum] = null; // ��Ÿ�� �Ϸ� �� �ڷ�ƾ ���� ����
    }

    public Skill[] GetSkill(int _type)
    {
        // ��ų �ʱ�ȭ ����
        switch (_type)
        {
            case 0:
                skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "���ڵ�");
                skillSet[1] = new DashAttack(6f, 6f, 4f, "������");
                skillSet[2] = new UltimateAttack(6f, 5f, 6f, "��������");
                SKillIconSet(0);
                Debug.Log(skillSet[0].skillName);
                Debug.Log(skillSet[1].skillName);
                Debug.Log(skillSet[2].skillName);
                break;
            case 1:
                skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "����");
                //skillSet[1] = new TalktoEnemy(1.2f, 0f, 4f, "����");
                //skillSet[2] = new ThrowSomething(6f, 3f, 6f, 1, "��ä��и�");
                SKillIconSet(1);
                break;
            case 2:
                skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "�Ϻ���");
                //skillSet[1] = new SlashBlade(2f, 3f, 18f, "�ٶ�������");
                //skillSet[2] = new ThrowSomething(6f, 0f, 0f, 2, "����");
                SKillIconSet(2);
                break;
            default:
                Debug.Log("�߸��� ��ų Ÿ���Դϴ�.");
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
        this.lastUsedTime = -skillCool; // ó������ ��ų�� ����� �� �ֵ��� ����
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
            SoundManager.instance.PlaySE("����");
            SkillManager.Instance.anim.SetTrigger("Attack");
            SkillManager.Instance.SkillCool(0);
        }
        else
            Debug.Log("��ų ��Ÿ�� ��");
    }

    public override void ApproachUseSkill()
    {
        if (IsOffCooldown())
            Debug.Log("��ų ��� ����");
        else
            Debug.Log("��ų ��Ÿ�� ��");
    }
}

public class DashAttack : Skill
{
    public DashAttack(float skillRange, float soundRange, float skillCool, string skillName)
        : base(skillRange, soundRange, skillCool, skillName) { }

    public override void UseSkill()
    {
        Debug.Log("DashAttack skill used!");
        SoundManager.instance.PlaySE("���");
        SkillManager.Instance.anim.SetTrigger("Attack");
        SkillManager.Instance.SkillCool(1);
    }

    public override void ApproachUseSkill()
    {
        if (IsOffCooldown())
            Debug.Log("��ų ��� ����");
        else
            Debug.Log("��ų ��Ÿ�� ��");
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
        SoundManager.instance.PlaySE("�ʻ�");
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
            Debug.Log("��ų ��� ����");
        else
            Debug.Log("��ų ��Ÿ�� ��");
    }
}

