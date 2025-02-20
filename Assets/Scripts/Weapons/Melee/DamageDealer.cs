using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private bool canDealDamage;
    private List<GameObject> hasDealtDamage;

    [SerializeField] private float weaponLength = 1.5f;
    [SerializeField] private float weaponDamage = 10f;

    private void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    private void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;
            int layerMask = 1 << 9; // Маска слоя врагов
            Debug.Log("Deal govna1");
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength))
            {
                Debug.Log("Deal govna2" + hit.collider.gameObject.name);
                if (hit.collider.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable) && !hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    damagable.TakeDamage(weaponDamage); // Вызываем метод интерфейса
                    Debug.Log("Dealing damage");
                    hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
        Debug.Log("Can dealing true");
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
        Debug.Log("Can dealing false");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}