using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이한 한 판에 대한 데이터를 기록/저장하는 싱글톤 클래스
/// </summary>
public class GameStatisticsTracker
{
    private static GameStatisticsTracker instance;
    public static GameStatisticsTracker Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameStatisticsTracker();
            }
            return instance;
        }
    }
    // 게임 결과 관련 변수
    private TimeSpan totalPlayTimeInSeconds; // 총 플레이 시간(초)
    public TimeSpan TotalPlayTimeInSeconds { get => totalPlayTimeInSeconds; }
    private int totalKillCount = 0; // 총 처치한 적 수
    public int TotalKillCount { get => totalKillCount; }
    private float maximumDamage = 0;
    public float MaximumDamage { get => maximumDamage; }

    // 플레이 시간 관련 변수
    private DateTime playStartTime;
    private bool isPlayTimeRunning = false;

    public GameStatisticsTracker()
    {
        if(instance != null) Debug.LogError("GameStatisticsTracker 인스턴스가 이미 존재합니다.");
        else instance = this;
    }

    // 총 적 처치 수 증가
    public void AddTotalKillCount(int count)
    {
        totalKillCount += count;
    }

    // 플레이 시간 측정 시작/종료 함수
    public void PlayTimeStart()
    {
        if (isPlayTimeRunning) return;
        playStartTime = DateTime.Now;
        isPlayTimeRunning = true;
    }

    public void PlayTimeStop()
    {
        if (!isPlayTimeRunning) return;
        totalPlayTimeInSeconds = DateTime.Now - playStartTime;
        isPlayTimeRunning = false;
    }

    // 플레이어가 낸 최대 데미지를 저장하는 함수
    public void SetMaximumDamage(float damage)
    {
        if (damage > maximumDamage)
        {
            maximumDamage = damage;
        }
    }

    // 게임 결과 초기화
    public void ResetStatistics()
    {
        totalPlayTimeInSeconds = TimeSpan.Zero;
        totalKillCount = 0;
        maximumDamage = 0;
    }
}
