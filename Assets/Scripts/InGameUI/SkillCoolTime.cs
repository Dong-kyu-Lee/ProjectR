using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTime : MonoBehaviour
{
    [SerializeField]
    private GameObject hideImg;
    private float hideImgFill;
    private bool canUseSkill = true;
    private float coolTime = 5f;
    private float starTime = 0f;


    private void Start()
    {
        hideImgFill = hideImg.GetComponent<Image>().fillAmount;
        hideImg.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)&&canUseSkill)
        {
            canUseSkill = false;
            StartCoroutine("SkillTimeChk");
            StartCoolTime();
        }
    }

    public void StartCoolTime()
    {
        hideImg.SetActive(true);
        canUseSkill=false;
    }

    IEnumerator SkillTimeChk()
    {
        yield return null;

        if (starTime > 0)
        {
            starTime -= Time.deltaTime;

            if (starTime < 0)
            {
                starTime = 0;
                hideImg.SetActive(false);
                starTime = coolTime;
                canUseSkill = true;
            }
            float time = starTime / coolTime;
            hideImgFill = time;

        }

    }


}