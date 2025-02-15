using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropableItem : MonoBehaviour
{
    public List<BasicItemData> dropableItem;

    public void addItme(BasicItemData item)
    {
        dropableItem.Add(item);
    }
    public void removeItem(BasicItemData item)
    {
        dropableItem.Remove(item);
    }
}
