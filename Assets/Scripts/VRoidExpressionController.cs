using VRM;
using UnityEngine;

public class VRoidExpressionController : MonoBehaviour
{
    public VRMBlendShapeProxy blendShapeProxy;

    void Update()
    {
        // ���� ǥ�� ����
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            blendShapeProxy.SetValue(BlendShapePreset.Joy, 1.0f);
        }

        // ȭ�� ǥ�� ����
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blendShapeProxy.SetValue(BlendShapePreset.Angry, 1.0f);
        }

        // ǥ�� �ʱ�ȭ
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Joy, 0f);
            blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Angry, 0f);
        }
    }
}
