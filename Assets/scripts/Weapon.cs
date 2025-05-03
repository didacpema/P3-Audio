using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    public PlayerMovement player;
    public Camera playerCamera;

    //ADS
    public bool isAiming;

    //Shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2.0f;

    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    //Spread
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float ADSSpreadIntensity;

    //Bullets
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletSpeed = 30;
    public float bulletLifeTime = 3.0f;

    public GameObject muzzleEffect;
    public Animator animator;

    //Reload
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    // Sound
    public AudioSource audioSource;
    public AudioClip gunshotSound;
    public AudioClip gunNoAmmoSound;
    public AudioClip gunReloadBSound;
    public AudioClip gunReloadASound;

    // Recoil
    public float recoilIntensity = 2.0f;
    public float recoilDuration = 0.1f;

    public enum ShootingMode
    {
        Single,
        Auto,
        Burst
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        audioSource = GetComponent<AudioSource>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isReloading)
        {
            EnterADS();
        }
        else if (Input.GetMouseButtonUp(1) && isAiming)
        {
            ExitADS();
        }

        if (currentShootingMode == ShootingMode.Auto)
        {
            //Holding down left click
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single)
        {
            //Click left click
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && readyToShoot)
        {
            isAiming = false;
            Reload();
        }

        if (readyToShoot && isShooting && bulletsLeft > 0 && !isReloading)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

        if (bulletsLeft < 1 && isShooting)
        {
            // Play noAmmo sound
            audioSource.PlayOneShot(gunNoAmmoSound);
        }

        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
        }

        // Trigger walking animation based on player's movement
        if (player.isMoving && !isReloading && !isShooting)
        {
            animator.SetBool("MOVE", true);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("SPRINT", true);
            }
            else
            {
                animator.SetBool("SPRINT", false);
            }
        }
        else
        {
            animator.SetBool("SPRINT", false);
            animator.SetBool("MOVE", false);
        }

    }

    private void EnterADS()
    {
        if (isReloading) return;
        animator.SetTrigger("ENTERADS");
        isAiming = true;

        spreadIntensity = ADSSpreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("EXITADS");
        isAiming = false;

        spreadIntensity = hipSpreadIntensity;
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isAiming)
        {
            animator.SetTrigger("ADSFIRE");
        }
        else
        {
            animator.SetTrigger("FIRE");
        }

        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        //Instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        bullet.transform.forward = shootingDirection;

        //Shoot
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletSpeed, ForceMode.Impulse);

        // Play gunshot sound
        audioSource.PlayOneShot(gunshotSound);

        // Apply recoil
        StartCoroutine(ApplyRecoil());

        //Destroy
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));

        //Check if ended shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        //Burst mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        { //we already shot once
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        if (bulletsLeft < 1)
        {
            animator.SetTrigger("RELOADB");
            // Play reloadB sound
            audioSource.PlayOneShot(gunReloadBSound);
        }
        else if (bulletsLeft > 0)
        {
            animator.SetTrigger("RELOADA");
            // Play reloadA sound
            audioSource.PlayOneShot(gunReloadASound);
        }

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;

        // Re-enter ADS if right mouse button is still held down
        if (Input.GetMouseButton(1))
        {
            EnterADS();
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        //Shooting from the middle of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //Return direction and spread
        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private IEnumerator ApplyRecoil()
{
    float recoilEndTime = Time.time + recoilDuration;
    while (Time.time < recoilEndTime)
    {
        float pitch = Mathf.LerpAngle(playerCamera.transform.localEulerAngles.x, playerCamera.transform.localEulerAngles.x - recoilIntensity, (recoilEndTime - Time.time) / recoilDuration);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);
        yield return null;
    }

    // Smoothly return to the original position
    recoilEndTime = Time.time + recoilDuration;
    while (Time.time < recoilEndTime)
    {
        float pitch = Mathf.LerpAngle(playerCamera.transform.localEulerAngles.x, playerCamera.transform.localEulerAngles.x + recoilIntensity, (recoilEndTime - Time.time) / recoilDuration);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);
        yield return null;
    }
}
}