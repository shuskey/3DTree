using UnityEngine;
using System.Collections;

public class TransportMeScript : MonoBehaviour
{
    public int TreePersonIndex;
    public GameObject DestinationGameObject;
    public TransporterType Type = TransporterType.None;

    public enum TransporterType
    {
        None,
        Birth,
        Wedding
    }

    // Use this for initialization
    private void Start()
    {

    }


    private void myInitPersonIndex(int index)
    {
        TreePersonIndex = index;
    }

    private void myInitDestinationObject(GameObject destinationObject)
    {
        DestinationGameObject = destinationObject;
    }

    private void myInitType(TransporterType type)
    {
        Type = type;
    }

    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {

            if (Type == TransporterType.Wedding)
            {
                otherObject.transform.position = DestinationGameObject.transform.position;
                GetComponent<AudioSource>().Play();

            }
            if (Type == TransporterType.Birth)
            {
                otherObject.transform.position = DestinationGameObject.transform.position;
                GetComponent<AudioSource>().Play();
            }
            if (Type == TransporterType.None)
            {
                // just play the sound
                GetComponent<AudioSource>().Play();

            }
        }
    }

    // Update is called once per frame
    private void Update()
    {

    }
}

