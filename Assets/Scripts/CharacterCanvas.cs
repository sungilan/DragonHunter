using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI atkText;
    [SerializeField] TextMeshProUGUI defText;
    [SerializeField] TextMeshProUGUI criticalHitChanceText;
    [SerializeField] TextMeshProUGUI criticalHitMultiplierText;

    private void Update()
    {
        nameText.text = DataManager.Instance.nickname.ToString();
        hpText.text = DataManager.Instance.hp.ToString();
        atkText.text = $"{DataManager.Instance.minAtk} - {DataManager.Instance.maxAtk}";
        defText.text = DataManager.Instance.def.ToString();
        criticalHitChanceText.text = DataManager.Instance.criticalHitChance.ToString();
        criticalHitMultiplierText.text = DataManager.Instance.criticalHitMultiplier.ToString();
    }
}
