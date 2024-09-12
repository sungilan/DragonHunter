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
        Debug.Log("첫 번째 스킬 발동!");
        anim.SetTrigger("Attack");
    }

    void ActivateSkill2()
    {
        Debug.Log("두 번째 스킬 발동!");
        anim.SetTrigger("ThrowShuriken");
    }
    void ActivateSkill3()
    {
        Debug.Log("세 번째 스킬 발동!");
        anim.SetTrigger("Kiss");
    }

    void ActivateSkill4()
    {
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
}
