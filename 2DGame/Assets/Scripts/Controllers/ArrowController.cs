using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{
    protected override void Init()
    {
        // 초기 화살의 방향 설정
        switch (Dir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        State = CreatureState.Moving;

        base.Init();
    }

    protected override void UpdateAnimation()
    {
    }
}
