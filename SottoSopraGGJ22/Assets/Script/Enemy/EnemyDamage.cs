using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public void Hit()
    {
        Vector3 LocalPosition = transform.position;
        LocalPosition.x *= -1f;
        transform.localPosition = LocalPosition;
    }
}
