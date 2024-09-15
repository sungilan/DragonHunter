using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Transform characterBody;
    [SerializeField] private Transform cameraArm;

    Animator animator;
    private float moveSpeed = 5f; // 이동 속도 변수

    void Start()
    {
        animator = characterBody.GetComponent<Animator>();
    }

    void Update()
    {
        FollowCamera();
    }

    public void Move(Vector2 inputDirection)
    {
        Vector2 moveInput = inputDirection;
        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isMove", isMove);

        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = moveDir;

            transform.position += moveDir * Time.deltaTime * moveSpeed;

            float speed = moveDir.magnitude * moveSpeed;
            animator.SetFloat("MoveSpeed", speed);
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0f);
        }
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void FollowCamera()
    {
        // 카메라 암이 캐릭터의 위치를 따라가지만 회전은 LookAround에서 처리
        Vector3 newPosition = new Vector3(characterBody.position.x, cameraArm.position.y, characterBody.position.z);
        cameraArm.position = newPosition;
    }
}
