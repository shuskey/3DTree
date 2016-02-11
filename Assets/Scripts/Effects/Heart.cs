using UnityEngine;

//class to add to collectible coins
public class Heart : MonoBehaviour 
{

	//public Vector3 rotation = new Vector3(0, 80, 0);		//idle rotation of coin
	public float Amp;
	//setup
	
	private Vector3 scale;
	
	//move coin toward player when he is close to it, and increase the spin/speed of the coin
	void Update()
	{
		//transform.Rotate (rotation * Time.deltaTime, Space.World);
		scale.x = scale.y = scale.z = 1 + Amp * Mathf.Sin (Time.time * 4);
		transform.localScale = scale;
			
	}
	

}

