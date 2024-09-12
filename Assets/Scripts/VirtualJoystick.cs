using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TPSCharacterController controller;
    [SerializeField] private RectTransform lever;
    private RectTransform rectTransform;
    [SerializeField, Range(10, 150)] private float leverRange;
    [SerializeField] Button[] skillBtns;
    [SerializeField] private Animator anim;

    private Vector2 inputDirection;
    private bool isInput;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        for (int i = 0; i < skillBtns.Length; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 임시 변수 사용
            skillBtns[i].onClick.AddListener(() => UseSkill(index));
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        ControllJoystickLever(eventData);
        isInput = true;
    }
    

    public void OnDrag(PointerEventData eventData)
    {
        ControllJoystickLever(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero;
        isInput = false;
        controller.Move(Vector2.zero);
    }

    private void ControllJoystickLever(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector;
        inputDirection = inputVector / leverRange;
    }

    private void InputControlVector()
    {
        controller.Move(inputDirection);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isInput)
        {
            InputControlVector();
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
        anim.SetTrigger("Slash");
    }
}
