using UnityEngine;

public class EnemyTest: MonoBehaviour, IDamagable
{
    public void TakeDamage(float amount)
    {
        Debug.Log(amount + "Больно урод");
    }

    public void HitVFX(Vector3 hitPosition)
    {
        Debug.Log(hitPosition);
    }
    
}