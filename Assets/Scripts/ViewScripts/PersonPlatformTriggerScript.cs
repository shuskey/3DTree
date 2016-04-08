using UnityEngine;
using System.Collections;

public class PersonPlatformTriggerScript : MonoBehaviour
{

    public int treePersonIndex;  // This is Who I am
    private GUIManager gui;
    private PersonPlatformScript myPersonPlatformScript;

    //setup
    void Awake()
    {
        
    }

    // Use this for initialization
    void Start ()
    {
        gui = FindObjectOfType(typeof(GUIManager)) as GUIManager;
        
        //var myroot = this.transform.root;   // This will get us the PersonPlatform
        myPersonPlatformScript = FindParentWithTag(this.gameObject, "PersonPlatform").GetComponent<PersonPlatformScript>();
        // myroot.GetComponentsInChildren<PersonPlatformScript>();
        //if (personPlatformScript.Length == 1)
        treePersonIndex = myPersonPlatformScript.treePersonIndex;
    }

    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            if (gui)
            {
           
                gui.PersonPortrait = myPersonPlatformScript.personPortrait;
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
    private static GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }

}
