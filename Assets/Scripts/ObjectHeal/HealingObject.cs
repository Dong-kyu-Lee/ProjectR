using System;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HealingObject : MonoBehaviour
{
    [Header("회복 설정")] 
    [SerializeField] private bool healToFull = true;
    [SerializeField] private float healAmout = 30f;
    
    [Header("상호작용 문구")]
    [SerializeField] private string interactionMessage = 
        "'<color=yellow>E</color>'키를 눌러 체력을 회복";

    [Header("사용 후 연출")] 
    [SerializeField] private Animator animator;
    [SerializeField] private string usedAnimationParameter = "IsUsed";
    [SerializeField] private UnityEvent onUsed;

    private readonly HashSet<Collider2D> playerColliders = new();

    private PlayerStatus currentPlayerStatus;
    private bool isUsed;

    public bool IsUsed => isUsed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed || !other.CompareTag("Player"))
            return;

        PlayerStatus playerStatus = other.GetComponentInParent<PlayerStatus>();
        if (playerStatus == null)
            return;

        playerColliders.Add(other);
        currentPlayerStatus = playerStatus;

        ShowInteractionUI();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerColliders.Remove(other);
        
        //플레이어에게 콜라이더가 여러개 있어서 모든 콜라이더가 빠졌을때 UI를 닫는다

        if (playerColliders.Count == 0)
        {
            HideInteractionUI();
            currentPlayerStatus = null;
        }
    }

    private void ShowInteractionUI()
    {
        if (InGameUIManager.Instance == null)
            return;
        
        InGameUIManager.Instance.ShowWarpUI(
            interactionMessage,
            // 괄호가 없음 즉 나중에 실행할 함수 자체를 전달한다.
            UseHealingObject
        );
    }
    
    private void HideInteractionUI()
    {
        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.HideWarpUI();
        }
    }

    private void UseHealingObject()
    {
        // 안전장치
        if (isUsed || currentPlayerStatus == null)
            return;
        // 체력이 가득 찼을때는 사용횟수를 소모하지 않음.
        if (currentPlayerStatus.Hp >= currentPlayerStatus.MaxHp)
        {
            InGameUIManager.Instance?.ShowStatus("이미 체력이 가득 차 있습니다.");
            return;
        }
        
        // E키 중복 입력을 방지.
        // 회복보다 먼저 true로 바꾸는 이유
        // E키가 같은 순간 여러번 입력되도 중복 실행을 막기위해서.
        isUsed = true;

        if (healToFull)
        {
            currentPlayerStatus.Hp = currentPlayerStatus.MaxHp;
        }
        
        // heal to full이 체크되어 있지 않은 경우
        else
        {
            currentPlayerStatus.Hp = Mathf.Min(currentPlayerStatus.Hp + healAmout,
                currentPlayerStatus.MaxHp);
        }
        
        HideInteractionUI();
        playerColliders.Clear();

        if (animator != null && !string.IsNullOrEmpty(usedAnimationParameter))
        {
            animator.SetBool(usedAnimationParameter, true);
        }
        
        // 회복 이펙트, 사운드 등을 인스펙터에서 연결
        onUsed?.Invoke();
        
        InGameUIManager.Instance?.ShowStatus(" 분수대의 힘으로 체력을 회복했습니다.");

    }

    private void OnDisable()
    {
        playerColliders.Clear();
        currentPlayerStatus = null;
        HideInteractionUI();
    }
}
