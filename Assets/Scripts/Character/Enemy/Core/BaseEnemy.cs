using System;
using Fusion;
using UnityEngine;

/// <summary>
/// ネットワーク対応の敵キャラクターの基底クラス
/// </summary>
public abstract class BaseEnemy : NetworkBehaviour
{
    [Header("基本ステータス")]
    [SerializeField] protected float _maxHealth = 100f; // 最大HP
    [SerializeField] protected float _moveSpeed = 3f;   // 移動速度
    [SerializeField] protected float _attackDamage = 10f; // 攻撃力

    [Networked] public bool IsAlive { get; set; }

    // AI状態管理
    protected Transform _targetBattleship; // 戦艦のターゲット
    protected EnemyAIBrain _enemyAI; // AI行動制御
    protected Rigidbody _rigidbody;

    // 死亡時のイベント
    public event Action<BaseEnemy, Vector3> OnDeath; // 死亡イベント

    protected virtual void Awake()
    {
        _enemyAI = GetComponent<EnemyAIBrain>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public abstract void Initialize(); // 敵固有の初期化
    public abstract void AttackTarget(); // 攻撃実行

    /// <summary>
    /// 指定された方向へ移動します。
    /// </summary>
    /// <param name="direction">移動する方向</param>
    public void MoveTo(Vector3 direction)
    {
        if (_rigidbody != null)
        {
            _rigidbody.velocity = direction * _moveSpeed;
        }
        else
        {
            // Rigidbodyがない場合はTransformを直接操作（フォールバック）
            transform.position += direction * _moveSpeed * Runner.DeltaTime;
        }
    }

    public void Death()
    {
        OnDeath?.Invoke(this, transform.position); // 死亡イベントを発火
    }
}
