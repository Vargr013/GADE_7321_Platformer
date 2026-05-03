using UnityEngine;

public abstract class AIEnemyBase : MonoBehaviour
{
    public float speed;
    public float size;

    public float detectionRange = 10f;
    public Transform player;


    public virtual void Initialize(float speed, float size)
    {
        this.speed = speed;
        this.size = size;

        transform.localScale = Vector3.one * size;
    }

    public abstract void Act(); // Each enemy behaves differently
}
