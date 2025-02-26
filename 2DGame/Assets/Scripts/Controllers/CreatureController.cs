using UnityEngine;
using static Define;
using static UnityEngine.Rendering.DebugUI;

public class CreatureController : MonoBehaviour
{
    public float _speed = 5.0f;

    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    protected CreatureState _state = CreatureState.Idle;
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnimation();
        }
    }

    protected MoveDir _lastDir = MoveDir.Down;
    protected MoveDir _dir = MoveDir.Down;

    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;

            _dir = value;
            if (value != MoveDir.None)
                _lastDir = value;
            else
            {
                int a = 0;
            }

            UpdateAnimation();
        }
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
    {
        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();

        // 오브젝트 위치를 그리드 좌표의 0에 적용시킨다.
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                break;
            case CreatureState.Dead:
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
    }

    // 부드럽게 이동하는 기능을 처리한다.
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;

        // 플레이어가 그 다음 칸에 도착했다면
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {
        if (_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }

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

        // 가게 될 곳이 갈 수 있는지 확인한다.
        if (Managers.Map.CanGo(destPos))
        {
            // 해당 좌표에 다른 오브젝트(몬스터)가 있는지 확인한다.
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged()
    {

    }
}
