using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Family
{
	public int Generation;
	public int BridePersonIndex;
	public int GroomPersonIndex;

    // FamilyGenerationIndex = the relative generational index of this family within this generation
    public int FamilyGenerationIndex;
    public string FamilyName;
    public int NumberOfPeople;
    public float PreviousHouseEdge;  // X coodinate. helpful for inter house spacing.  If this is the 1st Family in this Generation then this is 0

    public string MarriageDate;
	public List<int> ChildrenPersonIndexList;

	static private int _childBearingAge = 49;
	static private int _babySpacingYears = 2;
	static private int _firstChildAfterMarriage = 1;
	static private int _chanceForABaby = 30; //percent

    /// <summary>
    /// TODO
    /// For the purposes of clustering and creating an 'infinite runner' display list
    /// we need to be able to ask the family object the following:
    /// Give me the list of down generation families (where mom and dad came from)
    /// Give me the list of up generation families (where the children married into)
    /// Give me the list of side generation families (marriage, adoption)

    public Family() : this(0, 0, 0, "") { }

    public Family(int generation, int bridePersonIndex,int groomPersonIndex, string marriageDate) 
	{
		Generation = generation;
		BridePersonIndex = bridePersonIndex;
		GroomPersonIndex = groomPersonIndex;
		MarriageDate = marriageDate;
		ChildrenPersonIndexList = new List<int>();

	}

	public bool BabyTime(int currentYear)
	{
		//Debug.Log ("Are we having a baby?");
		bool retGotaBaby = false;
		var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();
		// consider Marriage date
		int iMarriageDate;
		if (!int.TryParse(MarriageDate, out iMarriageDate)) return retGotaBaby;
		if ((currentYear - iMarriageDate) <= _firstChildAfterMarriage) return retGotaBaby; 
		//Debug.Log ("Marriage Date looks good");
		// consider we need both a Bride and Groom
		if (myscript.myPeople.allPeople[GroomPersonIndex].isDead()) return retGotaBaby;
		if (myscript.myPeople.allPeople[BridePersonIndex].isDead()) return retGotaBaby;
		//Debug.Log ("We have a Bride and Groom!");
		// consider age of Bride

		if (myscript.myPeople.allPeople[BridePersonIndex].age (currentYear) >= _childBearingAge) return retGotaBaby;
		//Debug.Log ("The Bride is not too old.");
		// consider yougest childs birth year/age
		int iYoungestChildBirth = 0;
		if (ChildrenPersonIndexList.Count != 0)
		{
			foreach (int ChildPersonIndex in ChildrenPersonIndexList)
			{
				int iChildBirth;
				if(!int.TryParse(myscript.myPeople.allPeople[ChildPersonIndex].Birth, out iChildBirth)) return retGotaBaby;
				if (iChildBirth > iYoungestChildBirth) iYoungestChildBirth = iChildBirth;
			}
			if ((currentYear - iYoungestChildBirth) < _babySpacingYears) return retGotaBaby;
		}
		//Debug.Log ("There has been enough time since the last child!");
		// Okay all is well - lets also throw in a random chance
		int iRand = Random.Range(0, 100);
		if (iRand < _chanceForABaby) retGotaBaby = true;
		//Debug.Log (Bride.Name + " & " + Groom.Name + " Are going to have a baby! iRand =" + iRand.ToString());
		
		return retGotaBaby;
	}

	public void AddChildIndex(int childPersonIndex)
	{
		ChildrenPersonIndexList.Add(childPersonIndex);

		//Debug.Log ("New baby is a " + ((child.Sex == Person.PersonSex.Female) ? "Girl" : "Boy") + "!!");
		//Debug.Log ("The baby is named " + child.Name);
	}
    public string GetTextCourtHouse(MyPeople myPeople, int currentYear)
    {
        string retString = "Family (HOME): Generation " + Generation + "\r\n";
        if (Generation == 0)
        {
            retString += "\tGENERATION ZERO\r\n";
        }
        else
        {
            retString +=
                "\tBride: " + myPeople.allPeople[BridePersonIndex].GetText(currentYear, BridePersonIndex) +
                "\r\n" +
                "\t\tWife's parents family index: " +
                myPeople.allPeople[BridePersonIndex].BirthFamilyIndex + "\r\n" +
                "\tGroom: " + myPeople.allPeople[GroomPersonIndex].GetText(currentYear, GroomPersonIndex) +
                "\r\n" +
                "\t\tHusband's parents family index: " +
                myPeople.allPeople[GroomPersonIndex].BirthFamilyIndex + "\r\n" +
                "\tMarriage: " + MarriageDate + "\r\n";

        }
        foreach (int childPersonIndex in ChildrenPersonIndexList)
        {
            retString += "\t\tChild: " +
                         myPeople.allPeople[childPersonIndex].GetText(currentYear, childPersonIndex) + "\r\n";
        }
        return retString;
    }
    public string GetText(int currentYear)
	{
		var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();
		string retString = "Family (HOME): Generation " + Generation +"\n";
		if (Generation == 0)
		{
			retString += "\tGENERATION ZERO\n";
		}
		else
		{
			retString +=
			"\tBride: " + myscript.myPeople.allPeople[BridePersonIndex].GetText(currentYear, BridePersonIndex) + "\n"+
			"\t\tBride's parents family index: " + myscript.myPeople.allPeople[BridePersonIndex].BirthFamilyIndex + "\n"+
			"\tGroom: " + myscript.myPeople.allPeople[GroomPersonIndex].GetText(currentYear, GroomPersonIndex) + "\n"+
			"\t\tBride's parents family index: " + myscript.myPeople.allPeople[GroomPersonIndex].BirthFamilyIndex + "\n"+
			"\tMarriage: " + MarriageDate + "\n";

		}
		foreach (int childPersonIndex in ChildrenPersonIndexList)
		{
			retString += "\t\tChild: " + myscript.myPeople.allPeople[childPersonIndex].GetText(currentYear,childPersonIndex) + "\n";
		}
		return retString;
	}
}

