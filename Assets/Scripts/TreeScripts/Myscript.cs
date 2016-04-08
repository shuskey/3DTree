using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.TreeScripts;
using UnityEngine;

public class Myscript : MonoBehaviour
{

    private bool isActive = false;

    public UnityEngine.Object MyPersonPlatformObject;
    public UnityEngine.Object MyHousePlatformObject;
    public UnityEngine.Object MyTransporterObject;
    public string ThreeDFamilyTreeFileName = ""; // if Null or empty use Adam and Eve, otherwise read from this file

    //this sets up the over all scaling of our world
    public float PersonWidth = 50.0f;  // The width of a person platform
    public float InterPersonSpacing = 5.0f;  // The spacing between person platforms
    public float InterHouseSpacing = 20.0f;  // The spacing between Houses
    public float ZScale = 2.0f;  // Number of meters per year
    public float GenerationGap = 200.0f;   // Y axis offset for each generation

    private ScriptScale myScriptScale;
    public MyPeople myPeople;
    public CourtHouse MyCourtHouse;

    private GameObject[] _personsBirthPlatformObjects;
    private GameObject[] _personsWeddingPlatformObjects;
    private GameObject[] _weddingDayDestinationObjects;
    private GameObject[] _birthDayDestinationObjects;

    private Dictionary<int, GenerationInformation> _generationInformationDictionary; 

    private Transform[] _childrenTransforms;

    private GUIManager gui;

    private void Awake()
    {
        Debug.Log("Awake");
        myScriptScale = new ScriptScale();
        myScriptScale.PersonWidth = PersonWidth;
        myScriptScale.InterPersonSpacing = InterPersonSpacing;
        myScriptScale.InterHouseSpacing = InterHouseSpacing;
        myScriptScale.GenerationGap = GenerationGap;
        myScriptScale.ZScale = ZScale;

        // Let the GUI Manager know about our ZScale
        gui = FindObjectOfType(typeof(GUIManager)) as GUIManager;
        gui.SendMessage("myInitZScale", ZScale);

        myPeople = new MyPeople();
        _generationInformationDictionary = new Dictionary<int, GenerationInformation>();

        _personsBirthPlatformObjects = new GameObject[1500];
        _personsWeddingPlatformObjects = new GameObject[1500];

        _weddingDayDestinationObjects = new GameObject[1500];

        _birthDayDestinationObjects = new GameObject[1500];

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

        #region CheckArguments for Filename of Family History Information XML file
        string[] arguments = Environment.GetCommandLineArgs();
        if (ThreeDFamilyTreeFileName == "" && arguments.Count() > 1)
        {
            ThreeDFamilyTreeFileName = arguments[1];
        }
        
        if (string.IsNullOrEmpty(ThreeDFamilyTreeFileName))
        {
            Debug.Log("No 3D Family File was provided!");

            gui.dialogInformationMessage = "You have not provided a Family Inforamtion File.  There is nothing to play.";
        }    //TODO should probably check to see if the file exists
        #endregion
        else
        #region READ From 3DFamilyTreeFileUtility File

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
        foreach (Family familyHome in allMyFamilies)
        {
            // This is the Gereation # as read from the file
            int generation = familyHome.Generation;
            if (generation != 0)
            {
                if (!_generationInformationDictionary.ContainsKey(generation))
                    _generationInformationDictionary.Add(generation, new GenerationInformation());
               
                // Keep track of the # of Families that we have in this Generation
                int familyGenerationIndex = _generationInformationDictionary[generation].IncrementGenerationCounter(); 
                int peopleCount = familyHome.ChildrenPersonIndexList.Count + 2;

                // build the families house
                GameObject newHousePlatform =
                    (GameObject) Instantiate(MyHousePlatformObject, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);
                
                newHousePlatform.transform.parent = _generationInformationDictionary[generation].GenerationGameObject.transform;

                // add this houseplatform to the new generationIndfromation Game Object

                // if we are the first house build for this generation, set previous house edge to zero
                // This works because new GenerationInformation initializes Previous House Edge to Zero
                familyHome.PreviousHouseEdge = _generationInformationDictionary[generation].PreviousHouseEdge; 

                familyHome.FamilyGenerationIndex = familyGenerationIndex;
                string[] parts = myPeople.allPeople[familyHome.GroomPersonIndex].Name.Split(' ');
                familyHome.FamilyName = parts[parts.Length - 1];

                familyHome.NumberOfPeople = peopleCount;

                newHousePlatform.SendMessage("myInitScale", myScriptScale);

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

                    // This parantage (used to) cause problems with obtaining the tree index in PersonPlatformTriggerScript Line 21 & 33
                    // FIXED it by giving the Person Platform a tag and then I search for the parent with this tag in the TriggerMat
                    newPersonPlatform.transform.parent = newHousePlatform.transform;

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
                    myTreePerson.PreviousHouseEdge = _generationInformationDictionary[generation].PreviousHouseEdge; 
                    newPersonPlatform.SendMessage("myInitScale", myScriptScale);
                    newPersonPlatform.SendMessage("myInit", myTreePerson);
                    newPersonPlatform.SendMessage("myInitType", platformType);
                    newPersonPlatform.SendMessage("myInitDestinationObject", tmpDestinationObject);
                    newPersonPlatform.SendMessage("myInitMarriageDate", personMarriageDate);

                } // for each family person

                _childrenTransforms = newHousePlatform.GetComponentsInChildren<Transform>();
                //_transformPreviousHouse = _childrenTransforms[4];
                foreach (Transform childTransform in _childrenTransforms)
                {
                    if (childTransform.name.Contains("RightEdgeEnd"))
                    {
                        _generationInformationDictionary[generation].PreviousHouseEdge = childTransform.transform.position.x;
                    }
                }
                // now move it all
               // _generationInformationDictionary[generation].GenerationGameObject.transform.position = new Vector3((generation-1) * 100.0f, 0.0f, 0.0f);

            }
        }
        #region Transporters
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
        #endregion
    }


// Update is called once per frame
    private void Update()
    {

        //Debug.Log ("Updating!!");
        //_generationInformationDictionary[1].GenerationGameObject.transform.Rotate(Vector3.down * Time.deltaTime );

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
