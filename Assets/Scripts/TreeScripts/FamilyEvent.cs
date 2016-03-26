using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FamilyEvent
{
	public string Date = "";
	public FamilyEventType EventType = FamilyEventType.Null;
	public int BridePersonIndex = 0;
	public int GroompersonIndex = 0;
	public int FamilyIndex = 0;
	public int Generation = 0;

	public enum FamilyEventType
	{
		Null,
		Marriage,
		Divorce,
		Adoption
	}
	public FamilyEvent() 
	{ 
	
	
	}
    public FamilyEvent(MyPeople myPeople, List<Family> allFamilies, FamilyEventType type, string date, int bridePersonIndex, int groomPersonIndex)
    {
        Date = date;
        EventType = type;
        BridePersonIndex = bridePersonIndex;
        GroompersonIndex = groomPersonIndex;
        Generation = allFamilies[myPeople.allPeople[groomPersonIndex].BirthFamilyIndex].Generation + 1;
    }

 //   public FamilyEvent(FamilyEventType type, string date, int bridePersonIndex, int groomPersonIndex)
	//{
	//	var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();
	//	Date = date;
	//	EventType = type;
	//	BridePersonIndex = bridePersonIndex;
	//	GroompersonIndex = groomPersonIndex;
	//	Generation = myscript.myMatchMaker.allFamilies[myscript.myPeople.allPeople[groomPersonIndex].BirthFamilyIndex].Generation + 1;
	//}

    public string GetText()
	{
		string retString = "";
		switch (EventType)
		{
		case FamilyEventType.Marriage:
			retString = " Married on " + Date;
			break;
		case FamilyEventType.Divorce:
			retString = " Divorced on " + Date;
			break;
		}
	
		return retString;
	}

}

