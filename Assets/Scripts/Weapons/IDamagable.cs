using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float amount);
    void HitVFX(Vector3 hitPosition);
}