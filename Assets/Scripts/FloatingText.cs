using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 챔피언이 데미지를 입을 때 뜨는 텍스트 제어
/// </summary>
public class FloatingText : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Vector3 moveDirection;
    private float timer = 0;

    ///How fast text moves
    public float speed = 3;

    ///How long the text is visible
    public float fadeOutTime = 1f;

    /// Update is called once per frame
    void Update()
    {
        this.transform.position = this.transform.position + moveDirection * speed * Time.deltaTime;


        timer += Time.deltaTime;
        float fade = (fadeOutTime - timer) / fadeOutTime;

        canvasGroup.alpha = fade;

        if (fade <= 0)
            Destroy(this.gameObject);
    }
    /// <summary>
    /// Called when floating text is created
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="v"></param>
    public void Init(Vector3 startPosition, float damage, bool isCritical)
    {
        this.transform.position = startPosition;

        canvasGroup = this.GetComponent<CanvasGroup>();

        TextMeshPro textMesh = this.GetComponent<TextMeshPro>();

        // 텍스트에 표시할 데미지를 설정
        textMesh.text = Mathf.Round(damage).ToString();

        // 치명타일 경우 빨간색 텍스트로 설정
        if (isCritical)
        {
            textMesh.color = Color.red;
        }
        else
        {
            textMesh.color = Color.white; // 일반 데미지는 흰색
        }

        moveDirection = new Vector3(Random.Range(-0.2f, 0.2f), 0.5f, Random.Range(-0.2f, 0.2f)).normalized;
    }
}
