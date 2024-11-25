using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFactory
{
    private GameObject targetObject;

    public BuffFactory(GameObject target)
    {
        targetObject = target;
    }

    public Buff GenerateBuff(BuffType type, float duration = 0.0f)
    {
        Buff buff = null;

        switch (type)
        {
            case BuffType.attackDamageIncrease:
                buff = new AtkDmgIncBuff(duration, targetObject);
                break;
        }

        return buff;
    }
}
