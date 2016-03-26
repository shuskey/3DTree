using UnityEngine;
using System.Collections;

public class PersonPlatformTriggerScript : MonoBehaviour
{

    public int treePersonIndex;  // This is Who I am
    private GUIManager gui;

    //setup
    void Awake()
    {
        
    }

    // Use this for initialization
    void Start ()
    {
        gui = FindObjectOfType(typeof(GUIManager)) as GUIManager;
        var myroot = this.transform.root;   // This will get us the PersonPlatform
        treePersonIndex = myroot.GetComponent<PersonPlatformScript>().treePersonIndex;
    }

    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            if (gui)
            {
                var myroot = this.transform.root;   // This will get us the PersonPlatform                
                gui.PersonPortrait = myroot.GetComponent<PersonPlatformScript>().personPortrait;
                gui.treePersonIndex = treePersonIndex;
                gui.showPersonInformation = true;
            }
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            if (gui)
            {
                gui.showPersonInformation = false;
            }

        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
