using UnityEngine;
using System.Collections;

public class SpinMeScript : MonoBehaviour
{
    public float XSpeed = 1.0f;
    public float YSpeed = 1.0f;
    public float ZSpeed = 1.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(XSpeed, YSpeed, ZSpeed);
	}
}