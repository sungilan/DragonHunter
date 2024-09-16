using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WorldSpace에 HealthBar 및 FloatingText Canvas UI를 표시하는 도우미 클래스
/// </summary>
public class WorldCanvasController : MonoBehaviour
{
    public GameObject worldCanvas;
    public GameObject floatingTextPrefab;
    public GameObject floatingSpritePrefab;

    /// <summary>
    /// For Creating a new FloatingText
    /// </summary>
    /// <param name="position"></param>
    /// <param name="v"></param>
    public void AddDamageText(Vector3 position, float damage, bool isCritical)
    {
        GameObject go = Instantiate(floatingTextPrefab);
        go.transform.SetParent(worldCanvas.transform);

        go.GetComponent<FloatingText>().Init(position, damage, isCritical);
    }
    public void DropItem(Vector3 dropPosition, Sprite itemSprite)
    {
        GameObject floatingSpriteObj = Instantiate(floatingSpritePrefab, dropPosition, Quaternion.identity);
        floatingSpriteObj.transform.SetParent(worldCanvas.transform);
        FloatingSprite floatingSprite = floatingSpriteObj.GetComponent<FloatingSprite>();
        floatingSprite.Init(dropPosition, itemSprite);
    }

}
