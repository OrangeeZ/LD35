﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Packages.EventSystem;
using UniRx;

public class Room : MonoBehaviour {

	public enum Type {

		Default,
		MedicalBay,
		Reactor,
		Workshop,
		ControlRoom,
		SecurityRoom,
		AlienMotherRoom

	}

	public struct EveryoneDied : IEventBase {

		public Room Room;

	}

	[SerializeField]
	private Type _roomType;

	[SerializeField]
	private Bounds _bounds;

	[SerializeField]
	private EnemySpawner[] _npcSpawners;

	[SerializeField]
	private Transform[] _ventilationHatches;

	[SerializeField]
	private EnvironmentObjectSpot[] _objectSpots;

	private static List<Room> _instances = new List<Room>();

	private List<Character> _charactersInRoom = new List<Character>();

	private void Awake() {

		_instances.Add( this );
	}

	private void Start() {

		EventSystem.Events.SubscribeOfType<Character.Died>( OnCharacterDie );
	}

	private void OnDestroy() {

		_instances.Remove( this );
	}

	public void Initialize() {

		foreach ( var each in _npcSpawners ) {

			each.Initialize();

			_charactersInRoom.Add( each.GetLastSpawnedCharacter() );
		}
	}

	public static void InitializeAll() {

		foreach ( var each in _instances ) {

			each.Initialize();
		}
	}

	public static Room FindRoomForPosition( Vector3 position ) {

		return _instances.FirstOrDefault( _ => _.Contains( position ) );
	}

	public static Room RandomRoomExcept( Room roomToAvoid ) {

		return _instances.Where( _ => _ != roomToAvoid ).RandomElement();
	}

	public bool Contains( Vector3 position ) {

		return _bounds.Contains( transform.worldToLocalMatrix.MultiplyPoint3x4( position ) );
	}

	public Vector3 FindClosestVentilationHatchPosition( Vector3 position ) {

		return _ventilationHatches.MinBy( _ => Vector3.SqrMagnitude( position - _.position ) ).position;
	}

	public EnvironmentObjectSpot FindFarthestObjectSpot( Vector3 position ) {

		if ( _objectSpots.All( _ => _.GetState() != EnvironmentObjectSpot.State.Empty ) ) {

			return null;
		}

		return _objectSpots.Where( _ => _.GetState() == EnvironmentObjectSpot.State.Empty )
			.MaxBy( each => Vector3.SqrMagnitude( each.transform.position - position ) );
	}

	public Type GetRoomType() {

		return _roomType;
	}

	private void OnCharacterDie( Character.Died diedEvent ) {

		if ( _charactersInRoom.Remove( diedEvent.Character ) ) {

			if ( _charactersInRoom.IsEmpty() ) {

				EventSystem.RaiseEvent( new EveryoneDied {Room = this} );
			}
		}
	}

	private void OnDrawGizmos() {

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube( _bounds.center, _bounds.size );
	}

	[ContextMenu( "Hook spots" )]
	private void HookSpots() {

		_ventilationHatches = transform.OfType<Transform>().Where( _ => _.name.ToLower().Contains( "hatch" ) ).ToArray();
		_objectSpots = GetComponentsInChildren<EnvironmentObjectSpot>( includeInactive: true );
		_npcSpawners = GetComponentsInChildren<EnemySpawner>( includeInactive: true );
	}

}