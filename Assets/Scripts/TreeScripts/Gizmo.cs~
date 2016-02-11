using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour {

	public float gizmoSize = 0.75f;

	public Color gizmoColor = Color.yellow;

	void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		Gizmos.DrawWireSphere(transform.position, gizmoSize);
	}
}
