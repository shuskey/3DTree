using System;
using UnityEngine;
using System.Collections;

public class HousePlatformScript : MonoBehaviour {
    public GameObject MyPlatform;
    public int MyFamilyIndex;
    public float StartYear = 0.0f;
    public float EndYear = 0.0f;
    public int NumberOfPeople = 2;
    public string FamilyName = "Family Name Here";
    public int Generation = 0;      //what generation is this birth family
    public int FamilyGenerationIndex = 0;  // For this generation, is this the first, second, or nth family

    public float PersonWidth = 2.0f;
    public float InterPersonSpacing = 1.0f;
    public float PreviousHouseEdge = 0.0f;
    public float InterHouseSpacing = 5.0f;
    public float ZDivider = 2.0f;
    public float GenerationGap = 20.0f;

    private Renderer[] childrenRenderers;
    private Transform[] childrenTransforms;
    private Transform ZTransformBackBoard;
    private Renderer BackBoard;
    private Renderer TextPanel;


    private void myInit(Family myFamily)
    {
        FamilyName = myFamily.FamilyName;
        var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();

        TreePerson.timeSpan[] personTimeSpans = new TreePerson.timeSpan[25];
        // TODO replace this with a LIST, use Add instead of a limited 0 .. 25 initialization
        for (int i = 0; i < 25; i++) personTimeSpans[i].Start = personTimeSpans[i].End = 0;
        int currentYear = DateTime.Today.Year;
        Generation = myFamily.Generation;
        FamilyGenerationIndex = myFamily.FamilyGenerationIndex;
        NumberOfPeople = myFamily.NumberOfPeople;
        PreviousHouseEdge = myFamily.PreviousHouseEdge;

        

        int startDate = HELPER_DateToInt(myFamily.MarriageDate);
        if (startDate < 1.0f)
        { 
            var gBirthDate = HELPER_DateToInt(myscript.myPeople.allPeople[myFamily.BridePersonIndex].Birth);
            var bBirthDate = HELPER_DateToInt(myscript.myPeople.allPeople[myFamily.GroomPersonIndex].Birth);
            startDate = Math.Max(gBirthDate, bBirthDate) + 16; // the Max wards off Zeros
        }
        int endDate = startDate;
        int peopleCount = myFamily.ChildrenPersonIndexList.Count + 2;
        if (Generation != 0)
        {
            #region Get endDate for Family

            for (int p = 0; p < peopleCount; p++)
            {
                if (p == 0) // Bride
                {
                    TreePerson mySpouse = myscript.myPeople.allPeople[myFamily.BridePersonIndex];
                    personTimeSpans[p].Start = startDate;
                    if (mySpouse.Death == "")
                    {
                        // Not Dead
                        personTimeSpans[p].End = currentYear; // I am ALIVE, and not married
                    }
                    else
                    {
                        // Dead
                        personTimeSpans[p].End = HELPER_DateToInt(mySpouse.Death);
                    }
                }
                else if (p == 1) // Groom
                {
                    TreePerson mySpouse = myscript.myPeople.allPeople[myFamily.GroomPersonIndex];
                    personTimeSpans[p].Start = startDate;
                    if (mySpouse.Death == "")
                    {
                        // Not Dead
                        personTimeSpans[p].End = currentYear; // I am ALIVE, and not married
                    }
                    else
                    {
                        // Dead
                        personTimeSpans[p].End = HELPER_DateToInt(mySpouse.Death);
                    }
                }
                else //Children get their timespan in the house
                {
                    TreePerson mychild = myscript.myPeople.allPeople[myFamily.ChildrenPersonIndexList[p - 2]];
                    personTimeSpans[p].Start = HELPER_DateToInt(mychild.Birth);
                    //TODO Incorporate Marriage Dates
                    //   if (!mychild.isMarried())
                    //   {
                    if (mychild.Death == "")
                    {
                        // Not Married, Not Dead
                        personTimeSpans[p].End = currentYear; // I am ALIVE, and not married
                    }
                    else
                    {
                        //Not Married, But Dead
                        personTimeSpans[p].End = HELPER_DateToInt(mychild.Death);
                    }

                    //  }
                    //  else
                    //  { // Married!
                    //      personTimeSpans[p].End = HELPER_DateToInt(mychild.marriageDate());
                    //  }
                }
                // who is the last left in the house?
                if (personTimeSpans[p].End > endDate) endDate = personTimeSpans[p].End;
            }

            #endregion

            #region Draw House

            StartYear = startDate;
            EndYear = endDate;

            float zscale = (float) (endDate - StartYear);
            float xscale = (float) peopleCount*(PersonWidth+InterPersonSpacing);

            float y = ((Generation-1)*GenerationGap) - 0.01f;
            float x = PreviousHouseEdge + InterHouseSpacing;
            float z = StartYear;

            // Mark the corners
            HELPER_MarkThisPoint("LeftEdgeStart", x, y, z/ZDivider);
            HELPER_MarkThisPoint("LeftEdgeEnd", x, y, (z + zscale)/ZDivider);
            HELPER_MarkThisPoint("RightEdgeStart", x + xscale, y, (z)/ZDivider);
            HELPER_MarkThisPoint("RightEdgeEnd", x + xscale, y, (z + zscale)/ZDivider);


            //GameObject newHouse = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject newHouse =
                (GameObject) Instantiate(MyPlatform, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);

            newHouse.transform.parent = this.transform;
            newHouse.transform.position = new Vector3(x, y, z / ZDivider);

            childrenTransforms = GetComponentsInChildren<Transform>();
            //ZTransformBackBoard = childrenTransforms[8];

            foreach (Transform childTransform in childrenTransforms)
            {
                if (childTransform.name.Contains("TransformBackBoard"))
                {
                    childTransform.transform.localScale = new Vector3(xscale, 1.0f, zscale/ZDivider);
                }
                if (childTransform.name.Contains("TransformFamilyName"))
                {

                    childTransform.transform.localScale = new Vector3(1.0f,1.0f, 2.0f);
                }

            }

            childrenRenderers = GetComponentsInChildren<Renderer>();
            // BackBoard = childrenRenderers[0];
            // TextPanel = childrenRenderers[1];
            foreach (Renderer childRender in childrenRenderers)
            {
                if (childRender.name.Contains("SignBoard"))
                {
                    // Nothing for now


                }
                if (childRender.name.Contains("FamilyName"))
                {
                    childRender.GetComponent<TextMesh>().text = "The " + FamilyName + " Family";
                }
            }
            
            #endregion
        }
    }

    // Use this for initialization
     void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int HELPER_DateToInt(string myStringDate)
    {
        int retIntDate = 0;
        // Special handling for if this starts as a Year Only, example "1869"
        if (!Int32.TryParse(myStringDate, out retIntDate))
        {  //That Parse did not work, so lets convert the full date format like "January 1869" or "21 January 1869"
            try
            {
                DateTime convertedDate = DateTime.Parse(myStringDate);
                retIntDate = convertedDate.Year;
            }
            catch (FormatException)
            {
                Debug.Log(string.Format("Unable to Parse this Date: '{0}'.", myStringDate));
            }
        }

        return retIntDate;
    }
    public void HELPER_MarkThisPoint(string name, float X, float Y, float Z)
    {

        float Xscale = 0.1f;
        float Yscale = 0.2f;
        float Zscale = 0.1f;

        GameObject newHouse = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //GameObject newHouse = (GameObject) Instantiate(mycube, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);

        newHouse.transform.parent = this.transform;
        newHouse.transform.position = new Vector3(X, Y, Z); // + 5*(peopleCount-1)
        newHouse.transform.localScale = new Vector3(Xscale, Yscale, Zscale);
        newHouse.name = name;
        newHouse.GetComponent<Renderer>().material.color = Color.white;
    }
}
