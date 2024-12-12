using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    [HideInInspector] public FirstPersonController playerController;
    [HideInInspector] public Transform camerTransform;

    private float currentAmmo = 0f;
    private float nextTimeToFire = 0f;
    private float inventory = 0f;
    
    private Vector3 targetRecoil = Vector3.zero;
    [HideInInspector] public Vector3 currentRecoil = Vector3.zero;

    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = gunData.magazineSize;
        inventory = gunData.inventory;

        playerController = transform.root.GetComponent<FirstPersonController>();
        camerTransform = playerController.playerCamera.transform;
    }

    public virtual void Update()
    {
        playerController.ResetRecoil(gunData);
        ResetDirectionalRecoil();
    }

    public void TryReload()
    {
        if (!isReloading && currentAmmo < gunData.magazineSize && inventory > 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log(gunData.gunName +  " is reloading");
        yield return new WaitForSeconds(gunData.reloadTime);

        isReloading = false;
        float prevAmmo = currentAmmo;
        if (inventory >= gunData.magazineSize)
        {
            currentAmmo = gunData.magazineSize;
        }
        else
        {
            currentAmmo = inventory;
        }

        inventory -= currentAmmo - prevAmmo;
        Debug.Log(gunData.gunName +  "reloaded, Inventory: " + inventory);
    }

    public void TryShoot()
    {
        if (isReloading)
        {
            Debug.Log(gunData.gunName +  " is reloading");
            return;
        }

        if (currentAmmo <= 0f)
        {
            Debug.Log(gunData.gunName +  " no bullets in magazine");
            return;
        }

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1 / gunData.fireRate);
            HandleShoot();
        }
    }

    private void HandleShoot()
    {
        currentAmmo--;
        Debug.Log(gunData.gunName + " Shot!!!, Bullets: " + currentAmmo);
        Shoot();
        
        playerController.ApllyRecoil(gunData);
        ApllyDirectionalRecoil();
    }

    public abstract void Shoot();
    
    
    private void ApllyDirectionalRecoil()
    {
        float recoilX = Random.Range(-gunData.d_maxRecoil.x, gunData.d_maxRecoil.x) * gunData.d_recoilAmount;
        float recoilY = Random.Range(-gunData.d_maxRecoil.y, gunData.d_maxRecoil.y) * gunData.d_recoilAmount;

        targetRecoil += new Vector3(recoilX, recoilY, 0);

        currentRecoil = Vector3.MoveTowards(currentRecoil, targetRecoil, Time.deltaTime * gunData.d_recoilSpeed);
    }

    private void ResetDirectionalRecoil()
    {
        currentRecoil = Vector3.MoveTowards(currentRecoil, Vector3.zero, Time.deltaTime * gunData.d_resetRecoilSpeed);
        targetRecoil = Vector3.MoveTowards(targetRecoil, Vector3.zero, Time.deltaTime * gunData.d_resetRecoilSpeed);

    }
}
