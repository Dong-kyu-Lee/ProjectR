using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 설정 정보를 저장 및 정보 제공하는 클래스
/// </summary>
public static class GameSettingsSaver
{
    static GameSettingsSaver()
    {
        bool flag = false;
        if(!PlayerPrefs.HasKey("BGMValue"))
        {
            PlayerPrefs.SetFloat("BGMValue", 0.5f);
            flag = true;
        }
        if(!PlayerPrefs.HasKey("SFXValue"))
        {
            PlayerPrefs.SetFloat("SFXValue", 0.5f);
            flag = true;
        }
        if(!PlayerPrefs.HasKey("IsFullScreen"))
        {
            PlayerPrefs.SetInt("IsFullScreen", 1);
            flag = true;
        }
        if(flag) PlayerPrefs.Save();
    }

    // BGM 음량 변경(저장 X)
    public static void SetBGMValue(float value)
    {
        PlayerPrefs.SetFloat("BGMValue", value);
    }
    // SFX 음량 변경(저장 X)
    public static void SetSFXValue(float value)
    {
        PlayerPrefs.SetFloat("SFXValue", value);
    }
    // 전체 화면 여부 변경(저장 O)
    public static void SetFullScreen(bool value)
    {
        PlayerPrefs.SetInt("IsFullScreen", value ? 1 : 0);
        PlayerPrefs.Save();
    }
    // 사운드 세팅 값 저장(게임 세팅 UI 나가거나 로비로 이동 버튼 클릭 시 호출)
    public static void SaveSounds()
    {
        PlayerPrefs.Save();
    }

    public static float GetBGMValue()
    {
        return PlayerPrefs.GetFloat("BGMValue");
    }
    public static float GetSFXValue()
    {
        return PlayerPrefs.GetFloat("SFXValue");
    }
    public static bool GetFullScreen()
    {
        return PlayerPrefs.GetInt("IsFullScreen") == 1;
    }
}
