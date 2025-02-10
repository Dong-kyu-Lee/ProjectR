using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [SerializeField] private GameObject damageTextPrefab; // DamageText 프리팹
    private ObjectPool<GameObject> damageTextPool;

    public int activeDamageTexts;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        damageTextPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(damageTextPrefab);
                obj.SetActive(false);
                return obj;
            },
            actionOnGet: obj => 
            {
                obj.SetActive(true);
                activeDamageTexts++;
            },
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    public GameObject GetDamageText()
    {
        return damageTextPool.Get();
    }

    public void ReturnDamageText(GameObject damageText)
    {
        damageTextPool.Release(damageText);
    }
}
