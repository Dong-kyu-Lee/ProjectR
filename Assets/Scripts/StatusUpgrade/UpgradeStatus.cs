using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStatus : MonoBehaviour
{
    private int statPoint;
    private int force;
    private int indurance;
    private int critical;
    private int dexterity;
    private int mystery;
    private int curse;

    public int StatPoint { get { return statPoint; } set { statPoint = value; } }
    public int Force { get { return force; } set { force = value; } }
    public int Indurance { get { return indurance; } set { indurance = value; } }
    public int Critical { get { return critical; } set { critical = value; } }
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }
    public int Mystery { get { return mystery; } set { mystery = value; } }
    public int Curse { get { return curse; } set { curse = value; } }

    void Awake()
    {
        statPoint = 0;
        force = 0;
        indurance = 0;
        critical = 0;
        dexterity = 0;
        mystery = 0;
        curse = 0;
    }
}