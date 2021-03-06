using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public GameObject player;

    public float speed = 1f;
    public float checkFrequency = 1f;
    public GUIController GUI;

    public float timeToDie = 1f;
    private float timeDead = 0f;
    private bool dead = false;
   
    private float lastCheck = 0f;
    private Vector3 direction;

    private Rigidbody rb;
    private EnemyProperties enemy;

    private float lifetime;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<EnemyPropertyController>().properties;
        lifetime = 0f;
    }

    internal Action callback;

    // Update is called once per frame
    void Update () {

        if(dead) {

            timeDead += Time.deltaTime;

            GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("Vector1_30FACB43"), timeToDie - timeDead);

            if (timeToDie - timeDead <= 0) {
                if(callback != null) callback();

                Destroy(this.gameObject);
            }

            return;
        }

        lastCheck += Time.deltaTime;

        lifetime += Time.deltaTime;

        GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("Vector1_30FACB43"), lifetime);

        if (lastCheck >= checkFrequency) {
            lastCheck = 0;

            UpdateEnemyPath();
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            rb.AddForce(-(player.transform.position - transform.position).normalized * 5, ForceMode.Impulse);
            player.GetComponent<PlayerControl>().AddDamage(enemy.attackDamage);
        }
    }

    void UpdateEnemyPath() {
        direction = (player.transform.position - transform.position).normalized;
    }

    public void OnDeath() {
        GUI.AddScore(1);
        dead = true;
        Debug.Log("Dead");
    }
}
