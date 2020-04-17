using System.Collections;
using System.Collections.Generic;
using Characters;
using Combat.Characters;
using UnityEngine;

public class Client : CharacterBase
{
    public Characters.Equipment[] equipment;
    public Stats currentStats;

    public Client() { }

    public Client(CharacterData data, int[] skillIndex, string clientName)
    {
        Character = data;
        SkillWheel = skillIndex;
        _name = clientName;

        // client equipment = base equipment from class
        equipment = new Characters.Equipment[2];
        equipment[0] = Character.baseEquipment[0];     //weapon
        equipment[1] = Character.baseEquipment[1];     //armor
    }

    public void UpdateStats()
    {
        currentStats.atq = Character.baseStats.atq + equipment[0].atqMod + equipment[1].atqMod;
        currentStats.prec = Character.baseStats.prec * (equipment[0].precMod + equipment[1].precMod + 1);
        currentStats.crit = Character.baseStats.crit * (equipment[0].critMod + equipment[1].critMod + 1);
        currentStats.maxHp = Character.baseStats.maxHp + equipment[0].endMod + equipment[1].endMod;
        currentStats.prot = Character.baseStats.prot * (equipment[0].protMod + equipment[1].protMod + 1);
    }
}
