using VRM;
using UnityEngine;

public class VRoidExpressionController : MonoBehaviour
{
    public VRMBlendShapeProxy blendShapeProxy;

    void Update()
    {
        // 웃는 표정 적용
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            blendShapeProxy.SetValue(BlendShapePreset.Joy, 1.0f);
        }

        // 화난 표정 적용
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            blendShapeProxy.SetValue(BlendShapePreset.Angry, 1.0f);
        }

        // 표정 초기화
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Joy, 0f);
            blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Angry, 0f);
        }
    }
}
