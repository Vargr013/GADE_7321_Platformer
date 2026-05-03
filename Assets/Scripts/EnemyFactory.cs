using UnityEngine;

public abstract class EnemyFactory
{
    public abstract AIEnemyBase CreateEnemy(Vector3 position);
}
