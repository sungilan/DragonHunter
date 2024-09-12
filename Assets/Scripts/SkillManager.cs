using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] private Animator anim;
    [SerializeField] Button[] skillBtns;
    private Transform mon;
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
        Debug.Log("ù ��° ��ų �ߵ�!");
        anim.SetTrigger("Attack");
    }

    void ActivateSkill2()
    {
        Debug.Log("�� ��° ��ų �ߵ�!");
        anim.SetTrigger("ThrowShuriken");
    }
    void ActivateSkill3()
    {
        Debug.Log("�� ��° ��ų �ߵ�!");
        anim.SetTrigger("Kiss");
    }

    void ActivateSkill4()
    {
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
}
