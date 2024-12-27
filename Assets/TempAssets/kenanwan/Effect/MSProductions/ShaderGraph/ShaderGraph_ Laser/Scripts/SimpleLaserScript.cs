using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLaserScript : MonoBehaviour
{
    public GameObject laserPrefab;
    public GameObject firePoint;
    public float maximumLength;

    private LineRenderer lr;
    private GameObject spawnedLaser;

    void Start()
    {
        spawnedLaser = Instantiate(laserPrefab, firePoint.transform) as GameObject;
        lr = spawnedLaser.transform.GetChild(0).GetComponent<LineRenderer>();
        DisableLaser();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EnableLaser();
            UpdateLaser();
        }

        if (Input.GetMouseButton(0))
        {
            UpdateLaser();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DisableLaser();
        }
    }

    void EnableLaser() {
        spawnedLaser.SetActive(true);
    }

    void UpdateLaser() {
        var firePointPos = firePoint.transform.position;
        if (firePoint != null)
        {
            spawnedLaser.transform.position = firePointPos;
        }

        RaycastHit hit;
        var ray = new Ray(firePointPos, firePoint.transform.forward * maximumLength);
        if (Physics.Raycast(ray, out hit, maximumLength))
        {
            lr.SetPosition(1, new Vector3 (0, 0, (hit.point - firePointPos).magnitude));
        }
        else
        {
            lr.SetPosition(1, new Vector3(0, 0, maximumLength));
        }
    }

    void DisableLaser()
    {
        spawnedLaser.SetActive(false);
    }
}
