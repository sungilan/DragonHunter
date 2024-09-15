using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingSprite : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Vector3 moveDirection;
    private float timer = 0;

    /// How fast sprite moves
    public float speed = 3;

    /// How long the sprite is visible
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
    /// Called when floating sprite is created
    /// </summary>
    public void Init(Vector3 startPosition, Sprite itemSprite)
    {
        this.transform.position = startPosition;

        canvasGroup = this.GetComponent<CanvasGroup>();

        Image image = this.GetComponent<Image>();

        // ��������Ʈ ����
        image.sprite = itemSprite;

        // ������ ���� ���� (������ �ణ�� ������ ���� �̵�)
        moveDirection = new Vector3(Random.Range(-0.2f, 0.2f), 0.5f, Random.Range(-0.2f, 0.2f)).normalized;
    }
}
