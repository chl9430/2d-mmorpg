using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        // 초기 화살의 방향 설정
        switch (_lastDir)
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

        base.Init();
    }

    protected override void UpdateAnimation()
    {
    }

    protected override void UpdateIdle()
    {
        if (_dir != MoveDir.None)
        {
            Vector3Int destPos = CellPos;
            switch (_dir)
            {
                case MoveDir.Up:
                    destPos += Vector3Int.up;
                    break;
                case MoveDir.Down:
                    destPos += Vector3Int.down;
                    break;
                case MoveDir.Left:
                    destPos += Vector3Int.left;
                    break;
                case MoveDir.Right:
                    destPos += Vector3Int.right;
                    break;
            }

            State = CreatureState.Moving;
            // 가게 될 곳이 갈 수 있는지 확인한다.
            if (Managers.Map.CanGo(destPos))
            {
                GameObject go = Managers.Object.Find(destPos);
                // 해당 좌표에 다른 오브젝트(몬스터)가 있는지 확인한다.
                if (go == null)
                {
                    CellPos = destPos;
                }
                else
                {
                    // 피격 판정
                    Debug.Log(go.name);
                    Managers.Resource.Destroy(gameObject);
                }
            }
            else
            {
                Managers.Resource.Destroy(gameObject);
            }
        }
    }
}
