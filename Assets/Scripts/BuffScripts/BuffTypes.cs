public enum BuffType
{
    None,                       //Hp회복용

    AttackDamageIncrease,       //공격력 증가
    DamageReductionIncrease,    //피해 감소량 증가
    Bless,                      //축복 버프
    Raging,                     //광분 버프
    CritDamageIncrease,         //크리티컬 데미지 증가
    CritPercentIncrease,        //크리티컬 확률 증가
    PriceAdditionalIncrease,    //재화 획득량 증가
    AttackSpeedIncrease,        //공격 속도 증가
    MoveSpeedIncrease,          //이동 속도 증가
    ExtremeSpeed,               //신속 버프
    EagleEye,                   //매의 눈 버프
    BulkUp,                     //벌크 업 버프
    IronBody,                   //강철 몸 버프
    Destruction,                //파괴 버프
    Reflection,                 //반사 버프
    Invincible,                 //무적 버프

    Force7,                     //무력 7레벨 버프
    Force16,                    //무력 16레벨 버프
    Critical4,                  //치명 4레벨 버프
    Critical7,                  //치명 7레벨 버프
    Dexterity7,                 //재주 7레벨 버프
    Dexterity13,                //재주 13레벨 버프

    Poison,                      //독 디버프
    Burn,                        //화상 디버프
    Freeze,                      //빙결 디버프
    Slow,                        //둔화 디버프
    Sleep,                       //수면 디버프
    Buzzed,                      //취기 디버프
    Drunken,                     //만취 디버프
    Bleeding,                    //출혈 디버프
    Stun,                         //기절 디버프
    StoneCurse,                   //석화 디버프
    Confusion,                    //혼란 디버프
    Curse,                        //저주 디버프

    // 포션 전용 버프타입
    Potion_AttackSpeedIncrease,
    Potion_AttackDamageIncrease,
    Potion_MoveSpeedIncrease,
    Potion_CritPercentIncrease,
    Potion_CritDamageIncrease,
    Potion_DamageReductionIncrease,
}