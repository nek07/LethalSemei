using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    public Transform gunMuzzle;
    public GameObject bulletHolePrefab;
    public GameObject bulletParticlePrefab;

    [HideInInspector] public FirstPersonController playerController;
    [HideInInspector] public Transform camerTransform;

    private float currentAmmo = 0f;
    private float nextTimeToFire = 0f;
    private float inventory = 0f;
    
    private Vector3 d_targetRecoil = Vector3.zero;
    [HideInInspector] public Vector3 d_currentRecoil = Vector3.zero;

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
        // playerController.ResetRecoil(gunData);
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
        //
        // playerController.ApplyRecoil(gunData);
        ApllyDirectionalRecoil();
    }

    public abstract void Shoot();
    
    
    private void ApllyDirectionalRecoil()
    {
        float recoilX = Random.Range(-gunData.d_maxRecoil.x, gunData.d_maxRecoil.x) * gunData.d_recoilAmount;
        float recoilY = Random.Range(-gunData.d_maxRecoil.y, gunData.d_maxRecoil.y) * gunData.d_recoilAmount;

        d_targetRecoil += new Vector3(recoilX, recoilY, 0);

        d_currentRecoil = d_targetRecoil;
    }

    private void ResetDirectionalRecoil()
    {
        d_currentRecoil = Vector3.MoveTowards(d_currentRecoil, Vector3.zero, Time.deltaTime * gunData.d_resetRecoilSpeed);
        d_targetRecoil = Vector3.MoveTowards(d_targetRecoil, Vector3.zero, Time.deltaTime * gunData.d_resetRecoilSpeed);

    }
}
