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
        this.lastUsedTime = -skillCool; // 처음부터 스킬을 사용할 수 있도록 설정

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
            int index = i; // 클로저 문제를 피하기 위해 임시 변수 사용
            skillBtns[i].onClick.AddListener(() => UseSkill(index));
        }
    }
    void UseSkill(int index)
    {
        // index를 사용하여 어떤 스킬을 사용할지 결정
        Debug.Log($"Skill {index} 사용!");
        switch (index)
        {
            case 0:
                // 첫 번째 스킬 실행
                ActivateSkill1();
                break;
            case 1:
                // 두 번째 스킬 실행
                ActivateSkill2();
                break;
            case 2:
                // 세 번째 스킬 실행
                ActivateSkill3();
                break;
            case 3:
                // 네 번째 스킬 실행
                ActivateSkill4();
                break;
            default:
                Debug.Log("Unknown skill.");
                break;
        }
    }

    void ActivateSkill1()
    {
        SoundManager.instance.PlaySE("이얏");
        Debug.Log("첫 번째 스킬 발동!");
        anim.SetTrigger("Attack");
    }

    void ActivateSkill2()
    {
        SoundManager.instance.PlaySE("테잇");
        Debug.Log("두 번째 스킬 발동!");
        anim.SetTrigger("ThrowShuriken");
    }
    void ActivateSkill3()
    {
        SoundManager.instance.PlaySE("토우");
        Debug.Log("세 번째 스킬 발동!");
        anim.SetTrigger("Kiss");
    }

    void ActivateSkill4()
    {
        SoundManager.instance.PlaySE("필살");
        Debug.Log("네 번째 스킬 발동!");
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
                //skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "닌자도"); //1.3f
                //skillSet[1] = new Shurican(6f, 6f, 4f, "수리검");
                //skillSet[2] = new ThrowSomething(6f, 5f, 6f, 0, "돌던지기");
                SKillIconSet(0);
                break;
            case 1:
                //skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "쿠나이");
                //skillSet[1] = new TalktoEnemy(1.2f, 0f, 4f, "변장");
                //skillSet[2] = new ThrowSomething(6f, 3f, 6f, 1, "재채기분말");
                SKillIconSet(1);
                break;
            case 2:
                //skillSet[0] = new MeleeAttack(1.2f, 3f, 2f, "일본도"); //1.4f
                //skillSet[1] = new SlashBlade(2f, 3f, 18f, "바람가르기");
                //skillSet[2] = new ThrowSomething(6f, 0f, 0f, 2, "술병");
                SKillIconSet(2);
                break;
            default:
                print("잘못된 스킬 타입입니다.");
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
