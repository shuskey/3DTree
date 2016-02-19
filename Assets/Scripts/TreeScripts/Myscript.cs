using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class Myscript : MonoBehaviour
{

    private bool isActive = false;

    public UnityEngine.Object MyPersonPlatformObject;
    public UnityEngine.Object MyHousePlatformObject;
    public UnityEngine.Object MyTransporterObject;
    public string ThreeDFamilyTreeFileName = ""; // if Null or empty use Adam and Eve, otherwise read from this file
    public Matchmaker myMatchMaker;
    public MyPeople myPeople;
    public CourtHouse MyCourtHouse;
    public Color BrideColor = new Color(0.855f, 0.439f, 0.839f);
    public Color GroomColor = new Color(0.118f, 0.565f, 1.0f);
    public Color GirlColor = new Color(0.847f, 0.749f, 0.847f);
    public Color BoyColor = new Color(0.392f, 0.584f, 0.929f);
    public Color BlankColor = new Color(.467f, 0.533f, 0.600f);

    private GameObject[] _personsBirthPlatformObjects;
    private GameObject[] _personsWeddingPlatformObjects;
    private GameObject[] _weddingDayDestinationObjects;
    private GameObject[] _birthDayDestinationObjects;

    private Transform[] _childrenTransforms;

    //public MySecondScript secondScriptqqqqqqwe;

    // private static UnityEngine.Object _personPrefab;

    private void Awake()
    {
        Debug.Log("Awake");
        myMatchMaker = new Matchmaker();
        myPeople = new MyPeople();
        _personsBirthPlatformObjects = new GameObject[1000];
        _personsWeddingPlatformObjects = new GameObject[1000];

        _weddingDayDestinationObjects = new GameObject[1000];

        _birthDayDestinationObjects = new GameObject[1000];

        // _personPrefab = Resources.Load("FirstPerson");

    }

    // Use this for initialization
    private void Start()
    {
        Debug.Log("I just started!");
        //secondScript.SortMyList();

        // TODO turn this (below) into a LIST
        int[] counterPerGeneration = new int[25];

        List<Family> allMyFamilies = new List<Family>();

        for (int i = 0; i < 25; i++) counterPerGeneration[i] = 0;
        TreePerson.timeSpan[] personTimeSpans = new TreePerson.timeSpan[25];

        #region MatchMaker

        if (string.IsNullOrEmpty(ThreeDFamilyTreeFileName))
        {
            Debug.Log("Using MatchMaker");
            allMyFamilies = myMatchMaker.allFamilies;
        }
            #endregion

        else
            #region 3DFamilyTreeFileUtility

        {
            Debug.Log("Using CourtHouse");
            MyCourtHouse = new CourtHouse();

            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(MyCourtHouse.GetType());
            // To read the file, create a FileStream.
            FileStream myFileStream = new FileStream(ThreeDFamilyTreeFileName, FileMode.Open);
            // Call the Deserialize method and cast to the object type.
            MyCourtHouse = (CourtHouse) serializer.Deserialize(myFileStream);

            myFileStream.Close();
            allMyFamilies = MyCourtHouse.allFamilies;
            myPeople = MyCourtHouse.myPeople;
        }

        #endregion

        Debug.Log(string.Format("Ive got {0} Families and {1} people to add", allMyFamilies.Count,
            myPeople.allPeople.Count));

        // Lets do a review of all the families we have and determine the Generational relationships
        // FamilyGenerationIndex = the relative generational index of this family within this generation
        // FamilyPersonIndex = the relative person index (bride, groom, then birth order) of this person within the family
        // As well as the bounding life span information for each family, to help build a HOUSE around them
        float[] previousHouseEdge = new float[25]; // Keep track of the previous house edges for each generation
        foreach (Family familyHome in allMyFamilies)
        {
            // This is the Gereation # as read from the file
            int generation = familyHome.Generation;
            if (generation != 0)
            {

                // Keep track of the # of Families that we have in this Generation
                int familyGenerationIndex = counterPerGeneration[generation]++;
                int peopleCount = familyHome.ChildrenPersonIndexList.Count + 2;


                // build the families house
                GameObject newHousePlatform =
                    (GameObject) Instantiate(MyHousePlatformObject, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);

                // if we are the first house build for this generation, set previous house edge to zero
                if (familyGenerationIndex == 0) previousHouseEdge[generation] = 0.0f;
                familyHome.PreviousHouseEdge = previousHouseEdge[generation];

                familyHome.FamilyGenerationIndex = familyGenerationIndex;
                string[] parts = myPeople.allPeople[familyHome.GroomPersonIndex].Name.Split(' ');
                familyHome.FamilyName = parts[parts.Length - 1];

                familyHome.NumberOfPeople = peopleCount;

                newHousePlatform.SendMessage("myInit", familyHome);

                int brideGroomMarriageDate = HELPER_DateToInt(familyHome.MarriageDate);
                if (brideGroomMarriageDate < 1)
                {
                    var gBirthDate = HELPER_DateToInt(myPeople.allPeople[familyHome.BridePersonIndex].Birth);
                    var bBirthDate = HELPER_DateToInt(myPeople.allPeople[familyHome.GroomPersonIndex].Birth);
                    brideGroomMarriageDate = Math.Max(gBirthDate, bBirthDate) + 16;  // The Max wards off zeros
                }

                TreePerson myTreePerson;

                for (int familyPersonIndex = 0; familyPersonIndex < peopleCount; familyPersonIndex++)
                {
                    int tempPersonIndex = 0;
                    int personMarriageDate = brideGroomMarriageDate;
                    GameObject tmpDestinationObject;
                    PersonPlatformScript.PlatformType platformType;

                    GameObject newPersonPlatform =
                        (GameObject)
                            Instantiate(MyPersonPlatformObject, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);

                    if (familyPersonIndex == 0) //Bride
                    {
                        tempPersonIndex = familyHome.BridePersonIndex;

                        tmpDestinationObject = new GameObject("WeddingDay");
                        platformType = PersonPlatformScript.PlatformType.Wedding;
                        _weddingDayDestinationObjects[tempPersonIndex] = tmpDestinationObject;
                        _personsWeddingPlatformObjects[tempPersonIndex] = newPersonPlatform;


                    }
                    else if (familyPersonIndex == 1) //Groom
                    {
                        tempPersonIndex = familyHome.GroomPersonIndex;
                        tmpDestinationObject = new GameObject("WeddingDay");
                        platformType = PersonPlatformScript.PlatformType.Wedding;
                        _weddingDayDestinationObjects[tempPersonIndex] = tmpDestinationObject;
                        _personsWeddingPlatformObjects[tempPersonIndex] = newPersonPlatform;


                    }
                    else // Children
                    {
                        tempPersonIndex = familyHome.ChildrenPersonIndexList[familyPersonIndex - 2];

                        personMarriageDate = HELPER_DateToInt(myPeople.allPeople[tempPersonIndex].marriageDate());
                        if (personMarriageDate < 1)
                        {
                            personMarriageDate = HELPER_DateToInt(myPeople.allPeople[tempPersonIndex].Birth) + 16;
                        }

                        if (personMarriageDate < 100)
                        {
                            Debug.Log("Marriage Date too small");
                        }
                        tmpDestinationObject = new GameObject("BirthDay");
                        platformType = PersonPlatformScript.PlatformType.Birth;
                        _birthDayDestinationObjects[tempPersonIndex] = tmpDestinationObject;
                        _personsBirthPlatformObjects[tempPersonIndex] = newPersonPlatform;


                    }
                    // Neither the Parentage nor the postion is set up for the Destination Object
                    // We will do this in the PersonPlatformScript Start routine.


                    myTreePerson = myPeople.allPeople[tempPersonIndex];
                    //myTreePerson.TransporterObject = myTransporterObject;
                    myTreePerson.treePersonIndex = tempPersonIndex;  // Just incase you do not already know
                    myTreePerson.FamilyPersonIndex = familyPersonIndex;
                    myTreePerson.Generation = generation;
                    myTreePerson.FamilyGenerationIndex = familyGenerationIndex;
                    myTreePerson.PreviousHouseEdge = previousHouseEdge[generation];
                    newPersonPlatform.SendMessage("myInit", myTreePerson);
                    newPersonPlatform.SendMessage("myInitType", platformType);
                    newPersonPlatform.SendMessage("myInitDestinationObject", tmpDestinationObject);
                    newPersonPlatform.SendMessage("myInitMarriageDate", personMarriageDate);


                } // for each family person

                _childrenTransforms = newHousePlatform.GetComponentsInChildren<Transform>();
                //_transformPreviousHouse = _childrenTransforms[4];
                previousHouseEdge[generation] = 0.0f;
                foreach (Transform childTransform in _childrenTransforms)
                {
                    if (childTransform.name.Contains("RightEdgeEnd"))
                    {
                        previousHouseEdge[generation] = childTransform.transform.position.x;
                    }
                }
            }
        }

        // Now Set up transporters
        foreach (Family familyHome in allMyFamilies)
        {
            // This is the Gereation # as read from the file
            int generation = familyHome.Generation;
            if (generation != 0)
            {
                int peopleCount = familyHome.ChildrenPersonIndexList.Count + 2;

                for (int familyPersonIndex = 0; familyPersonIndex < peopleCount; familyPersonIndex++)
                {
                    int tempPersonIndex = 0;
                    GameObject tmpDestinationObject;
                    GameObject tmpPersonsPlatform;

                    TransportMeScript.TransporterType transporterType;


                    if (familyPersonIndex == 0) //Bride
                    {
                        tempPersonIndex = familyHome.BridePersonIndex;
                        transporterType = TransportMeScript.TransporterType.Birth;
                        tmpDestinationObject = _birthDayDestinationObjects[tempPersonIndex];
                        // the Birth Transporter needs to be attached to the persons WeddingPlatform
                        tmpPersonsPlatform = _personsWeddingPlatformObjects[tempPersonIndex];
                    }
                    else if (familyPersonIndex == 1) //Groom
                    {
                        tempPersonIndex = familyHome.GroomPersonIndex;
                        transporterType = TransportMeScript.TransporterType.Birth;
                        tmpDestinationObject = _birthDayDestinationObjects[tempPersonIndex];
                        // the Birth Transporter needs to be attached to the persons WeddingPlatform
                        tmpPersonsPlatform = _personsWeddingPlatformObjects[tempPersonIndex];

                    }
                    else // Children
                    {
                        tempPersonIndex = familyHome.ChildrenPersonIndexList[familyPersonIndex - 2];
                        transporterType = TransportMeScript.TransporterType.Wedding;
                        tmpDestinationObject = _weddingDayDestinationObjects[tempPersonIndex];
                        // The Wedding Transporter needs to be attached to the persons BirthPlatform
                        tmpPersonsPlatform = _personsBirthPlatformObjects[tempPersonIndex];

                    }
                    if (tempPersonIndex == 2 || tempPersonIndex == 3)
                    {
                        Debug.Log(String.Format("PersonIndex # {0} Here", tempPersonIndex));
                    }

                    if (tmpDestinationObject != null)
                    {
                        GameObject myTransporterObject =
                            (GameObject)
                                Instantiate(MyTransporterObject, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);

                        myTransporterObject.SendMessage("myInitPersonIndex", tempPersonIndex);
                        myTransporterObject.SendMessage("myInitDestinationObject", tmpDestinationObject);
                        myTransporterObject.SendMessage("myInitType", transporterType);

                        // Now relay this information to the PersonPlatform - the parentage and position will be setup inside its Start() method
                        tmpPersonsPlatform.SendMessage("myInitTransporterObject",
                            myTransporterObject);
                    }

                }

            } // skip gen 0

        } // foreach Family 

    }


// Update is called once per frame
    private void Update()
    {

        //Debug.Log ("Updating!!");
    }

    #region HELPERS

    public int HELPER_DateToInt(string myStringDate)
    {
        int IntDate = 0;

        // Special handling for if this starts as a Year Only, example "1869"
        if (!Int32.TryParse(myStringDate, out IntDate))
        {
            //That Parse did not work, so lets convert the full date format like "January 1869" or "21 January 1869"
            try
            {
                DateTime convertedDate = DateTime.Parse(myStringDate);
                IntDate = convertedDate.Year;
            }
            catch (FormatException)
            {
                Debug.Log(string.Format("Unable to Parse this Date: '{0}'.", myStringDate));
            }
        }

        return IntDate;
    }

    #endregion

}
