﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

    public GameObject enemy;
    public Transform spawnPlane;
    public GameObject player;
    public GUIController gUIController;

    public EnemyProperties[] minionTypes;
    public EnemyProperties[] bossTypes;

    private List<GameObject> waveEnemies;

    public float timeBetweenWaves = 5f;
    public float startCountdown = 2f;
    public float timeBetweenEnemies = .5f;
    public float minimumDistanceToPlayer = 2f;

    private int waveNumber = 0;

    private Bounds bounds;

    private bool isWaveSpawning = false;
    private bool bossAlive = false;

    // Use this for initialization
    void Start () {
        Mesh planeMesh = spawnPlane.GetComponent<MeshFilter>().mesh;
        bounds = planeMesh.bounds;
    }

    // Update is called once per frame
    void Update() {
        if (startCountdown < 0f) {
            isWaveSpawning = true;
            StartCoroutine(SpawnWave());
            startCountdown = timeBetweenWaves;
        }

        if(!isWaveSpawning && !bossAlive) startCountdown -= Time.deltaTime;
    }

    IEnumerator SpawnWave() {

        float waveStrength = (float) Math.Log10(waveNumber) + 0.1f;

        if (waveNumber % 5 == 0 && waveNumber > 0) waveStrength = SpawnEnemy(waveStrength, true);

        while (waveStrength >= 0.1f) {
            waveStrength = SpawnEnemy(waveStrength);
            yield return new WaitForSeconds(timeBetweenEnemies);
        }

        isWaveSpawning = false;
        waveNumber++;
    }

    private float SpawnEnemy(float strength, bool boss = false) {

        EnemyProperties[] props = boss ? bossTypes : minionTypes;
        EnemyProperties[] filteredProps = FilterByStrength(props, strength);

        EnemyProperties prop;

        if (boss)
            prop = StrongestBoss(filteredProps);
        else
            prop = RandomEnemyProperty(filteredProps);
        

        if (prop == null) return strength;

        Vector3 pos = spawnPlane.position;
        float scaleX = (spawnPlane.localScale.x * bounds.size.x) / 2;
        float scaleZ = (spawnPlane.localScale.z * bounds.size.z) / 2;

        Vector3 spawnPoint;

        do {
            float x = UnityEngine.Random.Range(pos.x - scaleX, pos.x + scaleX);
            float z = UnityEngine.Random.Range(pos.z - scaleZ, pos.z + scaleZ);

            spawnPoint = new Vector3(x, prop.scale / 2, z);
        } while ( (player.transform.position - spawnPoint).magnitude < minimumDistanceToPlayer);


        GameObject newEnemy = Instantiate(enemy, spawnPoint, new Quaternion());

        newEnemy.transform.localScale = new Vector3(prop.scale, prop.scale, prop.scale);
        newEnemy.GetComponent<SphereCollider>().radius = .5f;
        newEnemy.GetComponent<EnemyController>().speed = prop.speed;
        newEnemy.GetComponent<EnemyController>().player = player;
        newEnemy.GetComponent<EnemyPropertyController>().properties = prop;
        newEnemy.GetComponent<EnemyController>().GUI = gUIController.GetComponent<GUIController>();

        if (boss) {
            bossAlive = true;
            newEnemy.GetComponent<EnemyController>().callback = BossKilledCallback;
        }

        //newEnemy.transform.SetParent(transform);

        return strength - prop.strengthIndicator;
    }

    private EnemyProperties StrongestBoss(EnemyProperties[] filteredProps) {
        EnemyProperties max = filteredProps[0];

        for (int i = 1; i < filteredProps.Length; i++) {
            if (max.strengthIndicator < filteredProps[i].strengthIndicator)
                max = filteredProps[i];
            else if (max.strengthIndicator == filteredProps[i].strengthIndicator) {
                if (UnityEngine.Random.value <= 0.5f) max = filteredProps[i];
            }
        }

        return max;
    }

    private void BossKilledCallback() {
        bossAlive = false;

        Debug.Log("Yep, that is how you can use callbacks in C#");
    }

    private EnemyProperties RandomEnemyProperty(EnemyProperties[] filteredProps) {
        return filteredProps[(int) (UnityEngine.Random.Range(0, filteredProps.Length))];
    }

    private EnemyProperties[] FilterByStrength(EnemyProperties[] props, float strength) {

        List<EnemyProperties> newProps = new List<EnemyProperties>();

        foreach (EnemyProperties p in props) {
            if (p.strengthIndicator <= strength) newProps.Add(p);
        }

        return newProps.ToArray();
    }

}
