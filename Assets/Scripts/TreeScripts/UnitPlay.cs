using UnityEngine;
using System.Collections;

public class UnitPlay : Unit {

	float cameraRotX = 0f;
	public float cameraPitchMax = 45f;
	public float jumpForce = 500.0f;
	public float gravity = 10.0f;

	// Use this for initialization
	public override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	public override void Update () {

		//Rotation
		transform.Rotate (0f, Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime, 0f);

		cameraRotX = -Input.GetAxis ("Mouse Y");
		cameraRotX = Mathf.Clamp(cameraRotX, -cameraPitchMax, cameraPitchMax);
		Camera.main.transform.Rotate (cameraRotX , 0f, 0f);

		//Movement
		move = new Vector3(Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		//move.Normalize();

		move = transform.TransformDirection (move);

		base.Update();
		//jump
		if(Input.GetKeyDown (KeyCode.Space))
		{
			move.y = jumpForce;
		}
		
		move.y -= gravity * Time.deltaTime;
		control.Move(move * Time.deltaTime);
	}
}
