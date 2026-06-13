using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 설정 정보를 저장 및 정보 제공하는 클래스
/// </summary>
public class GameSettingsSaver
{

    public GameSettingsSaver()
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

    public void SaveBGMValue(float value)
    {
        PlayerPrefs.SetFloat("BGMValue", value);
        PlayerPrefs.Save();
    }
    public void SaveSFXValue(float value)
    {
        PlayerPrefs.SetFloat("SFXValue", value);
        PlayerPrefs.Save();
    }
    public void SaveFullScreen(bool value)
    {
        PlayerPrefs.SetInt("IsFullScreen", value ? 1 : 0);
        PlayerPrefs.Save();
    
    }
    public float GetBGMValue()
    {
        return PlayerPrefs.GetFloat("BGMValue");
    }
    public float GetSFXValue()
    {
        return PlayerPrefs.GetFloat("SFXValue");
    }
    public bool GetFullScreen()
    {
        return PlayerPrefs.GetInt("IsFullScreen") == 1;
    }
}
