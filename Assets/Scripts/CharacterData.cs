using static UnityEditor.Progress;
using System.Collections.Generic;

[System.Serializable]
public class CharacterData : RawData
{
    public string nickName;
    public int minAtk;
    public int maxAtk;
    public float criticalHitChance;
    public float criticalHitMultiplier;
    public List<string> skillList;

    // 장비 관련 속성 추가
    public Dictionary<string, Item> equipSlot = new Dictionary<string, Item>(); // 예: "Weapon", "Armor" 등 슬롯 별 장비
    public int equipAtk;
    public int equipDps;
}