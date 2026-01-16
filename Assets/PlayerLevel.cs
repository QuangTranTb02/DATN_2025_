using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillType
{
    Woodcutting,
    Minning,
    Fighting
}
[Serializable]
public  class Skill
{
    public int level;
    public int experience;
    public int NextLevel
    {
        get
        {
            return level * 1000;
        }
    }
    public SkillType skillType;
    public Skill(SkillType skillType)
    {
        level = 1; experience = 0;
        this.skillType = skillType;
    }

    internal void AddExp(int exp)
    {
        experience += exp;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if(experience >= NextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        experience -= NextLevel;
        level += 1;
    }
}

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] Skill WoodCutting;
    [SerializeField] Skill Mining;
    [SerializeField] Skill Fighting;

    private void Start()
    {
        WoodCutting = new Skill(SkillType.Woodcutting);
        Mining = new Skill(SkillType.Minning);
        Fighting = new Skill(SkillType.Fighting);
    }

    public int GetLevel(SkillType skillType)
    {
        Skill s = GetSkill(skillType);
        if (s == null) { return -1; }
        return s.level;
    }
    public void AddExperience(SkillType skillType, int exp)
    {
        Skill s = GetSkill(skillType);
        s.AddExp(exp);
    }
    public Skill GetSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Woodcutting:
                return WoodCutting;
            case SkillType.Minning:
                return Mining;
            case SkillType.Fighting:
                return Fighting;
        }
        return null;
    }
}
