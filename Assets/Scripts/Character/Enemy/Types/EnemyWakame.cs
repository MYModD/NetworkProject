using UnityEngine;

public class EnemyWakame : BaseEnemy
{
    [Header("Wakame固有設定")]
    [SerializeField]
    private BulletMove _bulletPrefab; // 弾丸プレハブ
    [SerializeField]
    private Transform _muzzle; // 銃口

    private float _lastAttackTime = -999f;
    private const float ATTACK_COOLDOWN = 2.0f; // 2秒のクールダウン

    public override void Initialize()
    {
        // Wakame固有の初期化処理があればここに記述
    }

    public override void AttackTarget()
    {
        // HasStateAuthorityのチェックは呼び出し元(EnemyAIBrain)で行う想定
        if (Runner.Tick > _lastAttackTime + (ATTACK_COOLDOWN * Runner.TickRate))
        {
             _lastAttackTime = Runner.Tick;

            // 弾を発射
            if (_bulletPrefab != null && _muzzle != null)
            {
                Vector3 targetDirection = (_muzzle.forward).normalized;
                Runner.Spawn(_bulletPrefab, _muzzle.position, Quaternion.LookRotation(targetDirection));
                // bullet.Init()のような初期化処理はBullet側で行う想定
            }
        }
    }
}
