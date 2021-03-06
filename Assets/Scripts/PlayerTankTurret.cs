﻿using UnityEngine;

public class PlayerTankTurret : MonoBehaviour
{
    [SerializeField] private Transform barrel = default;

    private Transform targetPoint;
    private Transform cam;
    [SerializeField] private Transform  cam_pos = default;
    [SerializeField] private Transform  turret = default;


    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        targetPoint = GameObject.Find("TargetPoint").transform;
        this.GetComponent<Rigidbody>().position = turret.position;
    }

    void Update()
    {
        UpdateCamera();
        RotateTurretWithCamera();
        RotateBarrelToController();
    }



    private void RotateTurretWithCamera()
    {
        //find the vector pointing from our position to the target
        var direction = ((cam.position + cam.forward*100) - transform.position);
        direction.y = 0;
        direction = direction.normalized;

        //create the rotation we need to be in to look at the target
        var lookRotation = Quaternion.LookRotation(direction);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4);
    }

    private void RotateBarrelToController()
    {
        //find the vector pointing from our position to the target
        var direction = (targetPoint.position - barrel.position).normalized;
        //create the rotation we need to be in to look at the target
        var lookRotation = Quaternion.LookRotation(direction);

        //rotate us over time according to speed until we are in the required rotation
        barrel.rotation = Quaternion.Slerp(barrel.rotation, lookRotation, Time.deltaTime * 4);
    }

    private void UpdateCamera()
    {
        CameraRigSetPosition.Instance.transform.position = cam_pos.position;
    }
}
