﻿using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.TreeScripts;

public class PersonPlatformScript : MonoBehaviour
{
    public GameObject MyPlatform;
    public PlatformType MyPlatformType;
    public GameObject MyDestinationGameObject;
    public GameObject MyTransporterGameObject; 

    public float Birth = 0.0f;
    public float Death = 0.0f;
    public float Marriage = 0.0f;
    public string Name = "";
    public string PortraitURI = "";
    public Texture personPortrait;
    public TreePerson.PersonSex Sex = TreePerson.PersonSex.NotSet;
    public int treePersonIndex;

    public Material BoyMaterial;
    public Material GirlMaterial;

    public Texture BoyTetTexture;
    public Texture GirlTexture;


    public int Generation = 0;      //what generation is this birth family
    public int FamilyGenerationIndex = 0;  // For this generation, is this the first, second, or nth family
    public int FamilyPersonIndex = 0; // for this person, are they the First, second, of nth in the family

    public float PreviousHouseEdge = 0.0f;
    private ScriptScale _myScriptScale = new ScriptScale();

    private Renderer[] childrenRenderers;
    private Transform[] childrenTransforms;
    private Transform ZTransformBackBoard;
    private Renderer BackBoard;
    private Renderer TextPanel;
    public enum PlatformType
    {
        None,
        Birth,
        Wedding
    }

    void myInitScale(ScriptScale myScaleScript)
    {
        _myScriptScale = myScaleScript;
    }

    void myInit(TreePerson myTreePerson)
    {
        treePersonIndex = myTreePerson.treePersonIndex;
        Birth = HELPER_DateToInt(myTreePerson.Birth);
        Death = HELPER_DateToInt(myTreePerson.Death);
        if (Death < 1.0f) Death = DateTime.Today.Year;
        Name = myTreePerson.Name;
        PortraitURI = myTreePerson.PortraitURI;
        Sex = myTreePerson.Sex;
        Generation = myTreePerson.Generation;
        FamilyGenerationIndex = myTreePerson.FamilyGenerationIndex;
        FamilyPersonIndex = myTreePerson.FamilyPersonIndex;
        PreviousHouseEdge = myTreePerson.PreviousHouseEdge;
    }

    void myInitType(PlatformType type)
    {
        MyPlatformType = type;
    }
    void myInitDestinationObject(GameObject destinationGameObject)
    {
        MyDestinationGameObject = destinationGameObject;
    }

    void myInitTransporterObject(GameObject transporterGameObject)
    {
        MyTransporterGameObject = transporterGameObject;
    }
    void myInitMarriageDate(int marriageDate)
    {
        Marriage = (float) marriageDate;
    }


    /// <summary>
    /// 
    /// </summary>
    // Use this for initialization
    IEnumerator Start () {
        if (Birth < 1.0f)
        {
            Debug.Log(string.Format("This person '{0}' has a birth of 0", Name));
        }
        else
        {
            Debug.Log("A person is alive!");

            float zscale = (Death - Birth) * _myScriptScale.ZScale;
            float xscale = _myScriptScale.PersonWidth;

            float y = (Generation-1)* _myScriptScale.GenerationGap;
            float x = PreviousHouseEdge + _myScriptScale.InterHouseSpacing;

            //GameObject newChild = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject newPlatform =
                (GameObject) Instantiate(MyPlatform, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);

            newPlatform.transform.parent = this.transform;
            newPlatform.transform.localPosition = new Vector3(x + ((xscale + _myScriptScale.InterPersonSpacing) * FamilyPersonIndex), y, Birth * _myScriptScale.ZScale);

            if (MyDestinationGameObject != null) MyDestinationGameObject.transform.parent = this.transform;
            if (MyTransporterGameObject != null) MyTransporterGameObject.transform.parent = this.transform;

            if (MyPlatformType == PlatformType.Birth)   //We have a Transporter located at the Wedding Day and a Destination located at the Birthday
            {  // note: these link two different Person Platforms
                if (MyDestinationGameObject != null) MyDestinationGameObject.transform.localPosition = new Vector3(x + xscale/ 2.0f + ((xscale + _myScriptScale.InterPersonSpacing) * FamilyPersonIndex), y, (Birth + 0.5f) * _myScriptScale.ZScale);
                if (MyTransporterGameObject != null) MyTransporterGameObject.transform.localPosition = new Vector3(x + xscale / 2.0f + ((xscale + _myScriptScale.InterPersonSpacing) * FamilyPersonIndex), y, Marriage * _myScriptScale.ZScale);

            }

            if (MyPlatformType == PlatformType.Wedding)  //We have a Transporter located at the Birthday and a Destination located at the Wedding Day
            {  // note: these link two different Person Platforms
                if (MyDestinationGameObject != null) MyDestinationGameObject.transform.localPosition = new Vector3(x + xscale / 2.0f + ((xscale + _myScriptScale.InterPersonSpacing) * FamilyPersonIndex), y, Marriage * _myScriptScale.ZScale);
                if (MyTransporterGameObject != null) MyTransporterGameObject.transform.localPosition = new Vector3(x + xscale / 2.0f + ((xscale + _myScriptScale.InterPersonSpacing) * FamilyPersonIndex), y, (Birth + 0.5f) * _myScriptScale.ZScale);
            }

            childrenTransforms = GetComponentsInChildren<Transform>();
//            ZTransformBackBoard = childrenTransforms[4];
            foreach (Transform childTransform in childrenTransforms)
            {
                if (childTransform.name.Contains("Z TRANSFORM"))
                {
                    childTransform.transform.localScale = new Vector3(xscale, 1.0f, zscale);
                } 
            }

            childrenRenderers = GetComponentsInChildren<Renderer>();
           // BackBoard = childrenRenderers[0];
           // TextPanel = childrenRenderers[1];
            foreach (Renderer childRender in childrenRenderers)
            {
                if (childRender.name.Contains("Back Board"))
                {
                    childRender.material = ((Sex == TreePerson.PersonSex.Female) ? GirlMaterial : BoyMaterial);
                    
                }
                if (childRender.name.Contains("TxtMyName"))
                {
                    childRender.GetComponent<TextMesh>().text = Name;
                }
                if (childRender.name.Contains("PortraitScreen"))
                {
                    if (PortraitURI != "")
                    {

                        //  childRender.material.mainTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);

                        // Start a download of the given URL
                        var www = new WWW(PortraitURI);

                        // wait until the download is done
                        yield return www;

                        // assign the downloaded image to the main texture of the object
                        personPortrait = childRender.material.mainTexture = www.texture;
                        

                    }
                    else
                    {
                        personPortrait = childRender.material.mainTexture = ((Sex == TreePerson.PersonSex.Female)
                            ? GirlTexture
                            : BoyTetTexture);
                    }
                }
            }
        }
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

}


