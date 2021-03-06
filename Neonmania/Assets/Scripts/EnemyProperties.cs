﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyType", menuName = "Enemy Type")]
public class EnemyProperties : ScriptableObject
{

	[Header("Movement Properties")]
	[Range(0f, 4f)]
	public float speed = 2f;
	public EnemyController movementLogic;

	[Header("Strength")]
	[Range(0f, 1f)]
	// higher is weaker
	public float weakness = 1f;
    public int level = 1;
    public float attackDamage = 1f;
	[Range(0.3f, 5f)]
	// higher is greater
	public float scale = 0.8f;
	[Range(0f, 1f)]
	// higher is stronger 
	public float strengthIndicator = 0.1f;

	[Header("Boss Properties")]
	// if true, players
	public bool isBoss = false;
    public int bossAttackType = 0;
}
