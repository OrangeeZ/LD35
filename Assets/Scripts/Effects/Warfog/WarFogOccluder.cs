﻿using UnityEngine;
using System.Collections;

public class WarFogOccluder : MonoBehaviour {

	[SerializeField]
	private Bounds _bounds;

	private void Reset() {

		var renderer = GetComponentInChildren<Renderer>( includeInactive: true );
		if ( renderer != null ) {

			_bounds = renderer.bounds;
		} else {

			var collider = GetComponentInChildren<Collider>( includeInactive: true );
			if ( collider != null ) {

				_bounds = collider.bounds;
			}
		}
	}

	public bool IsAffectingPoint( Vector3 point ) {

		var localPoint = transform.worldToLocalMatrix.MultiplyPoint3x4(point);
		//localPoint = Quaternion.Inverse( transform.rotation ) * localPoint;

		var inverseScale = transform.localScale;
		inverseScale.x = 1f / inverseScale.x;
		inverseScale.y = 1f / inverseScale.y;
		inverseScale.z = 1f / inverseScale.z;

		//localPoint.Scale( transform.localScale );

		return new Bounds(Vector3.zero, Vector3.one).Contains( localPoint );//_bounds.Contains( localPoint );
	}

	//void OnDrawGizmos() {

	//	Gizmos.matrix = transform.localToWorldMatrix;
	//	Gizmos.DrawWireCube( _bounds.center, _bounds.size );
	//}

}