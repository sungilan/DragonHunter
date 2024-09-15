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
                anim.Play("NormalAttack_B");
            }
        if (comboStep == 3)
            {
                SoundManager.Instance.PlaySE("���");
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
            SoundManager.Instance.PlaySE("�̾�");
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

        // ��Ÿ�� �ؽ�Ʈ Ȱ��ȭ
        coolCount[_skillNum].gameObject.SetActive(true);

        while (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
            skillIcons[_skillNum].fillAmount = Mathf.Clamp01(elapsedTime / cooldown);

            // ���� �ð��� ����Ͽ� �ؽ�Ʈ�� ǥ��
            float remainingTime = cooldown - elapsedTime;
            coolCount[_skillNum].text = ((int)remainingTime).ToString();

            yield return null;
        }

        skillIcons[_skillNum].fillAmount = 1f;
        coolCount[_skillNum].text = "";  // ��Ÿ���� ������ �� �ؽ�Ʈ �����
        coolCount[_skillNum].gameObject.SetActive(false);  // ��Ÿ�� �ؽ�Ʈ ��Ȱ��ȭ
        coolCoroutines[_skillNum] = null; // ��Ÿ�� �Ϸ� �� �ڷ�ƾ ���� ����
    }

    public Skill[] GetSkill(int _type)
    {
        // ��ų �ʱ�ȭ ����
        switch (_type)
        {
            case 0:
                skillSet[0] = new Dash(1.2f, 3f, 2f, "�뽬");
                skillSet[1] = new SpinAttack(6f, 30f, 4f, "ȸõ");
                skillSet[2] = new UltimateAttack(6f, 5f, 6f, "�����ϼ�");
                SKillIconSet(0);
                Debug.Log(skillSet[0].skillName);
                Debug.Log(skillSet[1].skillName);
                Debug.Log(skillSet[2].skillName);
                break;
            case 1:
                skillSet[0] = new Dash(1.2f, 3f, 2f, "����");
                //skillSet[1] = new TalktoEnemy(1.2f, 0f, 4f, "����");
                //skillSet[2] = new ThrowSomething(6f, 3f, 6f, 1, "��ä��и�");
                SKillIconSet(1);
                break;
            case 2:
                skillSet[0] = new Dash(1.2f, 3f, 2f, "�Ϻ���");
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
            
            // ���� �̹� �������� �޾Ҵ��� Ȯ��
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
        this.lastUsedTime = -skillCool; // ó������ ��ų�� ����� �� �ֵ��� ����
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
    private float dashDistance = 5f;  // �뽬�� �Ÿ�
    private float dashSpeed = 10f;    // �뽬 �ӵ�
    public Dash(float skillRange, float soundRange, float skillCool, string skillName)
        : base(skillRange, soundRange, skillCool, skillName) { }

    public override void UseSkill()
    {
        if (IsOffCooldown())
        {
            Debug.Log("Dash skill used!");
            SoundManager.Instance.PlaySE("������");
            SkillManager.Instance.anim.SetTrigger("Dash");

            // ���� ĳ���Ͱ� �ٶ󺸰� �ִ� �������� �뽬
            Vector3 dashDirection = caster.forward;
            Vector3 targetPosition = caster.position + dashDirection * dashDistance;

            // �뽬 ����
            SkillManager.Instance.StartCoroutine(DashMovement(targetPosition));

            SkillManager.Instance.SkillCool(0);
        }
        else
            Debug.Log("��ų ��Ÿ�� ��");
    }

    private IEnumerator DashMovement(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = caster.position;
        float dashDuration = dashDistance / dashSpeed;  // �뽬�� �� �ð�

        while (elapsedTime < dashDuration)
        {
            // ���� ����(Lerp)�� ����Ͽ� ĳ���͸� ������ �̵�
            caster.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �뽬 ���� ��Ȯ�ϰ� ��ǥ ��ġ�� ����
        caster.position = targetPosition;
    }
}

public class SpinAttack : Skill
{
    public int hitCount = 5;  // Ÿ�� Ƚ��
    public float hitInterval = 0.5f;  // Ÿ�� ����
    public SpinAttack(float skillRange, float skillDamage, float skillCool, string skillName)
        : base(skillRange, skillDamage, skillCool, skillName) { }

    public override void UseSkill()
    {
        Debug.Log("DashAttack skill used!");
        SoundManager.Instance.PlaySE("���͸�");
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
            // ������ ���
            int damage = Random.Range(DataManager.Instance.minAtk + (int)skillDamage, DataManager.Instance.maxAtk + (int)skillDamage);
            bool isCriticalHit = Random.value < DataManager.Instance.criticalHitChance;

            if (isCriticalHit)
            {
                damage = (int)(damage * DataManager.Instance.criticalHitMultiplier);
                enemy.TakeDamage(damage, true);  // ġ��Ÿ ����
            }
            else
            {
                enemy.TakeDamage(damage, false);  // �Ϲ� ������ ����
            }

            yield return new WaitForSeconds(hitInterval);  // �� Ÿ�� ������ ����
        }
    }
}
public class UltimateAttack : Skill
{
    private float dashDistance = 5f;  // �뽬�� �Ÿ�
    public float dashDamageRadius = 1f; // �뽬 ����� ��

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
            Debug.Log("��ų ��Ÿ�� ��");
    }

    private IEnumerator DashMovement()
    {
        SoundManager.Instance.PlaySE("�ʻ�");
        SkillManager.Instance.anim.SetTrigger("Slash");
        // �뽬 ���� ��ġ
        Vector3 startPosition = caster.position;
        // �뽬 ��ǥ ��ġ
        Vector3 dashDirection = caster.forward;
        Vector3 targetPosition = startPosition + dashDirection * dashDistance;
        yield return new WaitForSeconds(1f);
        caster.position = targetPosition;
        // �뽬 ��ο��� ������ �������� �����ϴ�.
        DealDamageAlongPath(startPosition, targetPosition);
        
    }

    private void DealDamageAlongPath(Vector3 startPosition, Vector3 endPosition)
    {
        // �뽬 ��ο� �ִ� ������ �����մϴ�.
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



