﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    private Rigidbody rb;
    private PlayerColorChanger colorChanger;
    private GameObject projectilePrefab;

    public float acceleration = 10f;
    public float maxSpeed = 30f;
    public float deceleration = 2f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        colorChanger = GetComponent<PlayerColorChanger>();
        projectilePrefab = Resources.Load("Projectile") as GameObject;
    }

    // Before performing physics calculation
    void FixedUpdate() {

        float moveHorizontal = Input.GetAxis("LeftJoystickHorizontal");
        float moveVertical = Input.GetAxis("LeftJoystickVertical");

        Vector3 v3 = new Vector3(moveHorizontal * acceleration, 0f, moveVertical * acceleration);

        if (moveHorizontal==0 && moveVertical==0)
        {
            //The player is not moving LeftJoystick any buttons, adding the mirrored vector to slow down
            Vector3 mirrorDirection = rb.velocity * (-1);
            rb.AddForce(mirrorDirection.normalized * deceleration);
        }
        else
        {
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
            else
            {
                rb.AddForce(v3);
            }
        }

        
    }

    // Update is called once per frame
    void Update () {
        float shootHorizontal = Input.GetAxis("RightJoystickHorizontal");
        float shootVertical = Input.GetAxis("RightJoystickHorizontal");        

        if (shootHorizontal != 0 || shootHorizontal != 0) {
            if(Input.GetAxis("AButton") != 0){
                Vector3 aimVector = new Vector3(shootHorizontal, 10f, shootVertical);
                GameObject projectile = Instantiate(projectilePrefab, transform) as GameObject;
                ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
                projectileController.color = colorChanger.GetColor();
                projectileController.direction = aimVector();
                projectileController.velocity = 40f;
            }
        }
		
	}
}
