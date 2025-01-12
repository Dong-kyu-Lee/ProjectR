using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStatus : MonoBehaviour
{
    private int skillPoint;
    private int force;
    private int indurance;
    private int critical;
    private int dexterity;
    private int mystery;

    public int SkillPoint { get { return skillPoint; } set { skillPoint = value; } }
    public int Force { get { return force; } set { force = value; } }
    public int Indurance { get { return indurance; } set { indurance = value; } }
    public int Critical { get { return critical; } set { critical = value; } }
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }
    public int Mystery { get { return mystery; } set { mystery = value; } }

    void Awake()
    {
        skillPoint = 0;
        force = 0;
        indurance = 0;
        critical = 0;
        dexterity = 0;
        mystery = 0;
    }
}