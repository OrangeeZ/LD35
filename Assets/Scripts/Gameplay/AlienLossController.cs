﻿using UnityEngine;
using System.Collections;
using Packages.EventSystem;
using UniRx;

public class AlienLossController : MonoBehaviour {

	private int _alienCount = 0;

	// Use this for initialization
	private void Awake() {

		EventSystem.Events.SubscribeOfType<EnemySpawner.Spawned>( OnEnemySpawn );
		EventSystem.Events.SubscribeOfType<XenoDeadStateInfo.Dead>( OnXenoDie );
	}

	private void OnXenoDie( XenoDeadStateInfo.Dead eventObject ) {

		_alienCount--;

		Debug.LogFormat( "Alien count: {0}", _alienCount );

		if ( _alienCount == 0 ) {

			Debug.Log( "All aliens dead" );
		}
	}

	private void OnEnemySpawn( EnemySpawner.Spawned eventObject ) {

		if ( eventObject.Character.Status.Info.name.Contains( "xen" ) ) {

			_alienCount++;
		}
	}

}