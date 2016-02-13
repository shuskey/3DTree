using UnityEngine;
using System.Collections;

public class TreePersonIndexHolder : MonoBehaviour
{
    public int treePersonIndex;
    // Use this for initialization

    void Start () {
        var myroot = this.transform.root;   // This will get us the PersonPlatform 
        treePersonIndex = myroot.GetComponent<PersonPlatformScript>().treePersonIndex;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
