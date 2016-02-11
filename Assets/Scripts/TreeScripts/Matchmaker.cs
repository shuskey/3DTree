using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matchmaker
{
	public List<int> BachelorettePersonIndexList;
	public List<int> BachelorPersonIndexList;
	public List<Family> allFamilies;

	public Matchmaker()
	{
		BachelorPersonIndexList = new List<int>();
		BachelorettePersonIndexList = new List<int>();
		allFamilies = new List<Family>();
		Family zeroFamily = new Family(0,0,0,"");
		addToAllFamilies(zeroFamily);  // THis will serve as the root of the tree and Generation 0, Adam and Eve can be Generation 1
	}

	public void addToSinglesList(int newPersonIndex, TreePerson.PersonSex sex)
	{
		if (sex == TreePerson.PersonSex.Female)
			BachelorettePersonIndexList.Add(newPersonIndex);
		else
			BachelorPersonIndexList.Add(newPersonIndex);

	}
	public void removeFromSinglesList(int deadPersonIndex, TreePerson.PersonSex sex)
	{
		if (sex == TreePerson.PersonSex.Female)
			BachelorettePersonIndexList.Remove(deadPersonIndex);
		else
			BachelorPersonIndexList.Remove(deadPersonIndex);
		
	}

	public void doWeddings(int currentYear)
	{
		var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();

		//foreach( int BoyPersonIndex in BachelorPersonIndexList) DOES NOT work because we modify the list see Remove below
		for (int i=0; i< BachelorPersonIndexList.Count; i++)
		{
			FamilyEvent marriageevent = makeMatch(BachelorPersonIndexList[i], currentYear);
			if (marriageevent != null)
			{
				Family myFamily = new Family(marriageevent.Generation, marriageevent.BridePersonIndex, marriageevent.GroompersonIndex, marriageevent.Date);
				int familyIndex = addToAllFamilies(myFamily);
				marriageevent.FamilyIndex = familyIndex;
				myscript.myPeople.allPeople[marriageevent.BridePersonIndex].MarriedFamilyIndex = familyIndex;
				myscript.myPeople.allPeople[marriageevent.GroompersonIndex].MarriedFamilyIndex = familyIndex;
				myscript.myPeople.allPeople[marriageevent.BridePersonIndex].AddEvent(marriageevent);
				myscript.myPeople.allPeople[marriageevent.GroompersonIndex].AddEvent(marriageevent);

			}
		}
	}

	public FamilyEvent makeMatch(int BoyPersonIndex, int currentYear)
	{
		int ofAge = 18;
		var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();

		FamilyEvent retMarriageEvent = null;

		int iage = myscript.myPeople.allPeople[BoyPersonIndex].age (currentYear);
		if (iage < ofAge) return retMarriageEvent;
			
		int GirlPersonIndex = findAWife(BoyPersonIndex, iage, currentYear);
		if (GirlPersonIndex != 0)
		{
			//retMarriageEvent = new FamilyEvent(FamilyEvent.FamilyEventType.Marriage, currentYear.ToString(), GirlPersonIndex, BoyPersonIndex);

			BachelorettePersonIndexList.Remove(GirlPersonIndex);
			BachelorPersonIndexList.Remove(BoyPersonIndex);
		}
		return retMarriageEvent;
	}

	public int findAWife(int BoyPersonIndex, int primeAge, int currentYear)
	{
		int ofAge = 16;
		int reasonableDelta = 6;

		int retPersonIndex = 0;
		var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();
		
		foreach (int GirlPersonIndex in BachelorettePersonIndexList)
		{
			if (myscript.myPeople.allPeople[GirlPersonIndex].isAlive())
			{
				int iage = myscript.myPeople.allPeople[GirlPersonIndex].age (currentYear);
				if ((iage >= ofAge) && (Math.Abs (primeAge - iage) <= reasonableDelta))
				{

					if (myscript.myPeople.allPeople[GirlPersonIndex].AskToMarry()) 
					{
						// Only if she says yes, otherwise you are out of luck this year
						//retPerson = new Person(Person.PersonType.Null);
						retPersonIndex = GirlPersonIndex;
					}
					break;
				}
			}
		}
		return retPersonIndex;
	}

	public void beFruitfullAndMultiply(int currentYear)
	{
		var myscript = GameObject.Find("Myscript").GetComponent<Myscript>();

		foreach(Family myFamily in allFamilies)
		{
			if (myFamily.BabyTime(currentYear))
			{
				TreePerson MyChild = new TreePerson(TreePerson.PersonType.Unique, currentYear.ToString());
				int childPersonIndex = myscript.myPeople.addToAllPeople(MyChild);
				myFamily.AddChildIndex(childPersonIndex);
				addToSinglesList (childPersonIndex, MyChild.Sex);
				MyChild.BirthFamilyIndex = allFamilies.IndexOf(myFamily);
			}
		}
	}

	public int addToAllFamilies(Family family)
	{
		allFamilies.Add(family);
		return allFamilies.IndexOf(family);
	}

	public void displayAllFamilies(int currentYear)
	{
		foreach (Family myFamily in allFamilies)
			Debug.Log("#" + allFamilies.IndexOf(myFamily) + "\n" + myFamily.GetText(currentYear));
	}
}

