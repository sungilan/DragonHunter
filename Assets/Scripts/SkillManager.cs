using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill
{
    [Header("skill info")]
    public string skillName;
    public float skillRange;
    public float soundRange;
    public float skillCool;
    protected float lastUsedTime;
    //protected NinjaController caster;
    protected Transform casterTr;
    public Skill(float skillRange, float soundRange, float skillCool, string skillName)
    {
        this.skillName = skillName;
        this.skillRange = skillRange;
        this.soundRange = soundRange;
        this.skillCool = skillCool;
        this.lastUsedTime = -skillCool; // ó������ ��ų�� ����� �� �ֵ��� ����

        //caster = DBManager.instance.myCon;
        //casterTr = caster.transform;
    }

    public bool IsOffCooldown()
    {
        return Time.time >= lastUsedTime + skillCool;
    }

    public abstract void UseSkill();
}

public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] private Animator anim;
    [SerializeField] Button[] skillBtns;
    private Transform mon;
    [SerializeField] private Image[] skillIcons = new Image[3];
    [SerializeField] private GameObject cooltext;
    private Coroutine coolCor = null;
    public Skill[] skillSet = new Skill[3];
    private void Awake()
    {
        for (int i = 0; i < skillBtns.Length; i++)
        {
            int index = i; // Ŭ���� ������ ���ϱ� ���� �ӽ� ���� ���
            skillBtns[i].onClick.AddListener(() => UseSkill(index));
        }
    }
    void UseSkill(int index)
    {
        // index�� ����Ͽ� � ��ų�� ������� ����
        Debug.Log($"Skill {index} ���!");
        switch (index)
        {
            case 0:
                // ù ��° ��ų ����
                ActivateSkill1();
                break;
            case 1:
                // �� ��° ��ų ����
                ActivateSkill2();
                break;
            case 2:
                // �� ��° ��ų ����
                ActivateSkill3();
                break;
            case 3:
                // �� ��° ��ų ����
                ActivateSkill4();
                break;
            default:
                Debug.Log("Unknown skill.");
                break;
        }
    }

    void ActivateSkill1()
    {
        SoundManager.instance.PlaySE("�̾�");
        Debug.Log("ù ��° ��ų �ߵ�!");
        anim.SetTrigger("Attack");
    }

    void ActivateSkill2()
    {
        SoundManager.instance.PlaySE("����");
        Debug.Log("�� ��° ��ų �ߵ�!");
        anim.SetTrigger("ThrowShuriken");
    }
    void ActivateSkill3()
    {
        SoundManager.instance.PlaySE("���");
        Debug.Log("�� ��° ��ų �ߵ�!");
        anim.SetTrigger("Kiss");
    }

    void ActivateSkill4()
    {
        SoundManager.instance.PlaySE("�ʻ�");
        Debug.Log("�� ��° ��ų �ߵ�!");
        StartCoroutine(OneShot());
    }
    IEnumerator OneShot()
    {
        anim.SetTrigger("Slash");
        mon = GameObject.FindGameObjectWithTag("Monster").transform;
        Vector3 dir = mon.position - this.gameObject.transform.position;
        dir.y = 0;
        dir.Normalize();
        Vector3 targetPos = mon.position + (dir * 1.2f);
        this.gameObject.transform.forward = dir;
        yield return new WaitForSecondsRealtime(1f);
        //mon.GetComponent<Animator>().SetBool("Dead",true);
        this.gameObject.transform.position = targetPos;
    }
    IEnumerator CoolTimeText()
    {
        cooltext.SetActive(true);
        yield return new WaitForSeconds(3);
        cooltext.SetActive(false);
    }
    public Skill[] GetSkill(int _type)
    {
        //ninjaCon = DBManager.instance.myCon;
        switch (_type)
        {
            case 0:
                //skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "���ڵ�"); //1.3f
                //skillSet[1] = new Shurican(6f, 6f, 4f, "������");
                //skillSet[2] = new ThrowSomething(6f, 5f, 6f, 0, "��������");
                SKillIconSet(0);
                break;
            case 1:
                //skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "����");
                //skillSet[1] = new TalktoEnemy(1.2f, 0f, 4f, "����");
                //skillSet[2] = new ThrowSomething(6f, 3f, 6f, 1, "��ä��и�");
                SKillIconSet(1);
                break;
            case 2:
                //skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "�Ϻ���"); //1.4f
                //skillSet[1] = new SlashBlade(2f, 3f, 18f, "�ٶ�������");
                //skillSet[2] = new ThrowSomething(6f, 0f, 0f, 2, "����");
                SKillIconSet(2);
                break;
            default:
                print("�߸��� ��ų Ÿ���Դϴ�.");
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
            //if (_type == 1 && i == 1)
                //lostKimono = true;
        }
    }
    public void SkillCool(int _skillNum)
    {
        skillIcons[_skillNum].fillAmount = 0;
        if (coolCor != null)
            StopCoroutine(coolCor);
        coolCor = StartCoroutine(FillCool(_skillNum));
    }
    private IEnumerator FillCool(int _skillNum)
    {
        while (skillIcons[_skillNum].fillAmount <= 1)
        {
            skillIcons[_skillNum].fillAmount += Time.deltaTime / skillSet[_skillNum].skillCool;
            yield return null;
        }
    }

}
