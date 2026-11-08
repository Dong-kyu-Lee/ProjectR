using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
[DefaultExecutionOrder(100)]
public class BlacksmithEnchantProgressUI : MonoBehaviour
{
    private const string PrefabPath = "Prefabs/UI/BlacksmithEnchantProgress";

    [SerializeField] private Canvas progressCanvas;
    [SerializeField] private UnityEngine.UI.Image fillImage;

    private BlacksmithAbilityV2 ability;
    private BlacksmithControllerV2 controller;

    public static void AttachTo(BlacksmithAbilityV2 target)
    {
        if (target == null) return;
        var existing = target.GetComponentInChildren<BlacksmithEnchantProgressUI>(true);
        if (existing != null)
        {
            existing.Bind(target);
            return;
        }

        var prefab = Resources.Load<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError("대장장이 강화 진행 바 프리팹을 찾을 수 없습니다.", target);
            return;
        }
        var instance = Instantiate(prefab, target.transform, false);
        instance.name = prefab.name;
        instance.GetComponent<BlacksmithEnchantProgressUI>().Bind(target);
    }

    private void Awake()
    {
        if (progressCanvas == null) progressCanvas = GetComponent<Canvas>();
        Hide();
    }

    private void OnEnable()
    {
        Bind(GetComponentInParent<BlacksmithAbilityV2>());
    }

    public void Bind(BlacksmithAbilityV2 target)
    {
        if (ability != null) ability.onAbilityUpdated.RemoveListener(Refresh);
        ability = target;
        controller = target != null ? target.GetComponent<BlacksmithControllerV2>() : null;
        if (ability != null && isActiveAndEnabled) ability.onAbilityUpdated.AddListener(Refresh);
        Refresh();
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (ability == null || !ability.isActiveAndEnabled || !ability.IsEnchanting || controller == null)
        {
            Hide();
            return;
        }

        // 독립 타이머 대신 실제 강화 모션을 읽어 일시정지/배속에도 표시가 일치한다.
        if (fillImage != null) fillImage.fillAmount = controller.EnchantProgress;
        if (progressCanvas != null) progressCanvas.enabled = true;
        transform.rotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        if (ability != null) ability.onAbilityUpdated.RemoveListener(Refresh);
        Hide();
    }

    private void Hide()
    {
        if (progressCanvas != null) progressCanvas.enabled = false;
        if (fillImage != null) fillImage.fillAmount = 0f;
    }
}
