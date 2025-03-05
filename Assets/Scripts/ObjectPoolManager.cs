using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [SerializeField] private GameObject damageTextPrefab; // DamageText 프리팹
    private ObjectPool<GameObject> damageTextPool;

    public int activeDamageTexts;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
            return;
        }

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

    private void FixedUpdate()
    {
        Debug.Log(activeDamageTexts);
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        damageTextPool.Clear();
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
