using Fusion;
using UnityEngine;

public class EnemyAIBrain : NetworkBehaviour
{
    // AIの状態定義
    enum AIState
    {
        Idle,    // 待機
        Chase,   // 追跡
        Attack,  // 攻撃
    }

    [Header("AI設定")]
    [SerializeField] private float _chaseRange = 15f; // 追跡開始距離
    [SerializeField] private float _attackRange = 5f; // 攻撃開始距離
    [SerializeField] private LayerMask _targetLayer; // ターゲットのレイヤー

    // 内部コンポーネントと状態
    private BaseEnemy _enemy;
    private Transform _target;

    [Networked]
    private AIState _currentState { get; set; }

    public override void Spawned()
    {
        _enemy = GetComponent<BaseEnemy>();
        if (_enemy == null)
        {
            Debug.LogError("BaseEnemy component not found on this GameObject.", this);
        }
        _currentState = AIState.Idle;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        // ターゲットがいない場合は探す
        if (_target == null)
        {
            SearchForTarget();
        }

        // 状態に応じた行動を実行
        switch (_currentState)
        {
            case AIState.Idle:
                UpdateIdleState();
                break;
            case AIState.Chase:
                UpdateChaseState();
                break;
            case AIState.Attack:
                UpdateAttackState();
                break;
        }
    }

    // --- 状態ごとの処理 ---

    private void UpdateIdleState()
    {
        // ターゲットを発見したら追跡状態に移行
        if (_target != null)
        {
            _currentState = AIState.Chase;
        }
    }

    private void UpdateChaseState()
    {
        if (_target == null)
        {
            _currentState = AIState.Idle;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        // 攻撃範囲に入ったら攻撃状態に移行
        if (distanceToTarget <= _attackRange)
        {
            _currentState = AIState.Attack;
        }
        else
        {
            // ターゲットに向かって移動
            Vector3 direction = (_target.position - transform.position).normalized;
            _enemy.MoveTo(direction);
            DoRotation(direction);
        }
    }

    private void UpdateAttackState()
    {
        if (_target == null)
        {
            _currentState = AIState.Idle;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        // 攻撃範囲から出たら追跡状態に戻る
        if (distanceToTarget > _attackRange)
        {
            _currentState = AIState.Chase;
        }
        else
        {
            // ターゲットの方向を向く
            Vector3 direction = (_target.position - transform.position).normalized;
            DoRotation(direction);
            // 攻撃実行
            _enemy.AttackTarget();
        }
    }

    // --- 補助メソッド ---

    private void SearchForTarget()
    {
        //索敵範囲内のターゲットを検索
        Collider[] targets = Physics.OverlapSphere(transform.position, _chaseRange, _targetLayer);
        if (targets.Length > 0)
        {
            // 最初に見つけたターゲットを設定
            _target = targets[0].transform;
        }
    }

    private void DoRotation(Vector3 targetDirection)
    {
        targetDirection.y = 0f;
        if (targetDirection.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(targetDirection.normalized);
        }
    }
}
