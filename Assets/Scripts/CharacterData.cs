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

    // ��� ���� �Ӽ� �߰�
    public Dictionary<string, Item> equipSlot = new Dictionary<string, Item>(); // ��: "Weapon", "Armor" �� ���� �� ���
    public int equipAtk;
    public int equipDps;
}