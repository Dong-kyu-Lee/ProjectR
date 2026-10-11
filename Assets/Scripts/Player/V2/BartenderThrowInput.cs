// 클릭 한 번의 조준/투척 상태. 취소되거나 쿨타임 중 시작한 클릭은
// 버튼을 계속 누르고 있어도 새 공격으로 이어지지 않는다.
public sealed class BartenderThrowInput
{
    public bool IsAiming { get; private set; }

    // 유효한 클릭을 놓은 프레임에만 true를 반환한다.
    public bool Update(bool pressed, bool held, bool released, bool canAim)
    {
        if (!canAim)
        {
            Cancel();
            return false;
        }

        if (pressed) IsAiming = true;
        if (!IsAiming) return false;

        if (released)
        {
            Cancel();
            return true;
        }

        // 포커스 전환 등으로 버튼 해제 이벤트가 누락된 경우에도 정리한다.
        if (!held) Cancel();
        return false;
    }

    public void Cancel()
    {
        IsAiming = false;
    }
}
