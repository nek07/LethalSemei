using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private bool canDealDamage;
    private List<GameObject> hasDealtDamage;
    private IDamagable master;
    private BoxCollider hitbox; // Коллайдер для удара

    [SerializeField] private float weaponDamage = 10f;

    private void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
        hitbox = GetComponent<BoxCollider>();

        if (hitbox == null)
        {
            Debug.LogError("DamageDealer: BoxCollider отсутствует! Добавь его на объект с этим скриптом.");
        }
        else
        {
            hitbox.isTrigger = true;
            hitbox.enabled = false; // Отключаем, пока не атакуем
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return; // Если не в атаке — ничего не делаем
        master = GetComponentInParent<IDamagable>();
        if (other.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable) 
            && !hasDealtDamage.Contains(other.gameObject)
            && master != damagable)
        {
            damagable.TakeDamage(weaponDamage);
            damagable.HitVFX(other.transform.position);
            Debug.Log("Dealing damage to: " + other.gameObject.name);
            hasDealtDamage.Add(other.gameObject);
            return;
        }

        IDamagable damagableParent = other.gameObject.GetComponentInParent<IDamagable>();
        
        if(damagableParent != null &&
           damagableParent != master &&
           !hasDealtDamage.Contains(other.gameObject))
        {
            damagableParent.TakeDamage(weaponDamage);
            damagableParent.HitVFX(other.transform.position);
            Debug.Log("Dealing damage to: " + other.gameObject.name);
            hasDealtDamage.Add(other.gameObject);
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
        hitbox.enabled = true; // Включаем hitbox
        Debug.Log("Damage hitbox активирован");
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
        hitbox.enabled = false; // Отключаем hitbox
        Debug.Log("Damage hitbox отключен");
    }
}