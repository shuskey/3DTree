using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TreePerson
{
	public string Name = "";
	public PersonSex Sex = PersonSex.NotSet;
	public string Birth = "";
	public string Death = "";
	public List<FamilyEvent> myEvents;
	public int BirthFamilyIndex = 0;
	public int MarriedFamilyIndex = 0;
   // public Scene BirthEntryScene;  // Helpfull when to want to go home to where you were born
   // public GameObject BirthEntryObject;
   // public Scene WeddingEntryScene;  // Helps you transport to your married family
   // public GameObject WeddingEntryObject;
   // public GameObject TransporterObject;
    /// <summary>
    ///  TODO
    /// Create an Events list
    /// Each Event (modeled after the current Marriage event)has:
    /// Name (Marriage, Divorce, Adoption)
    /// Date
    /// Generation
    /// BridePersonIndex, GroomPersonIndex, FamilyIndex
    /// 
    /// </summary>
    public int Generation = 0;      //what generation is this birth family
    public int FamilyGenerationIndex = 0;  // For this generation, is this the first, second, or nth family
    public int FamilyPersonIndex = 0; // for this person, are they the First, second, of nth in the family
    public float PreviousHouseEdge;  // X coodinate. helpful for inter house spacing.  If this is the 1st Family in this Generation then this is 0


    private string[] MaleNames = new string[] { "Adam", "Barney", "Charlie" };

		
	private string[] FemaleNames = new string[] { "Eve", "Beth", "Cindy" };

	public enum PersonType
	{
		Null,
		Unique,
		Adam,
		Eve,
		FamilySearch
	}

	public enum PersonSex
	{
		NotSet,
		Male,
		Female
	}	

	public struct timeSpan
	{
		public int Start;
		public int End;
	}
    public TreePerson() : this(PersonType.Null, "", null) { }

    public TreePerson(PersonType Type, string birth = "", TreePerson familySearchTreePerson = null)

    {
        switch (Type)
		{
		case PersonType.Null:
			Name = Death = "";
			Sex = PersonSex.NotSet;
			Birth = birth;
			break;

		case PersonType.Unique:
			if (Random.Range(0, 2) == 0)
			{
				Sex = PersonSex.Male;
				Name = MaleNames[Random.Range(0,MaleNames.Length)];
				Birth = birth;
			}
			else
			{
				Sex = PersonSex.Female;
				Name = FemaleNames[Random.Range(0,FemaleNames.Length)];
				Birth = birth;
			}
			break;

		case PersonType.Adam:
			Sex = PersonSex.Male;
			Name = "Adam";
			Birth = birth;
			BirthFamilyIndex = 0;  // from Generation 0
			break;

		case PersonType.Eve:
			Sex = PersonSex.Female;
			Name = "Eve";
			Birth = birth;
			BirthFamilyIndex = 0; // from Generation 0
			break;

            case PersonType.FamilySearch:
		        break;
		}

			myEvents = new List<FamilyEvent>();
	}

	public void setBirthFamilyIndex (int index)
	{
		BirthFamilyIndex = index;
	}

	public void AddEvent(FamilyEvent newEvent)
	{
		
		myEvents.Add(newEvent);
		
	}
	public void setMarriedFamilyIndex (int index)
	{
		MarriedFamilyIndex = index;
	}

	public bool AskToMarry()
	{
		bool retAnswer = false;
		int iRand = Random.Range(0, 10);
		if (iRand > 3) retAnswer = true;
		//Debug.Log (Name + " Got asked to Marry. iRand =" + iRand.ToString());
		//Debug.Log ("She said " + (retAnswer ? "Yes" : "No") + "!!!!!!!!!!!!!!!!!!!!!!!!!");
		return retAnswer;
	}
	public bool isDeathEvent( int mortalityRate, int currentYear)  // Chance out of 100000
	{
		bool retAnswer = false;
		int iRand = Random.Range(0, 100000);
		if (iRand < mortalityRate) 
		{
			retAnswer = true;
			//Debug.Log (Name + " Just Died at age " + ageText(currentYear) + ", I am sorry. mortalityRate=" + mortalityRate + ", iRand =" + iRand.ToString());
		}
		return retAnswer;
	}

	public string GetSex()
	{
		string retSex = "NotSet";
		if (Sex == PersonSex.Male) retSex = "Male";
		if (Sex == PersonSex.Female) retSex = "Female";
		return retSex;
	}
	public string ageText(int currentYear)
	{
		int ibirth;
		string age = "No Birth Year";
		if (int.TryParse(this.Birth, out ibirth))
		{
			age = (currentYear - ibirth).ToString();
		}
		return age;
	}

	public int age(int currentYear)
	{
		int ibirth;
		int age = 0;
		if (int.TryParse(this.Birth, out ibirth))
		{
			age = currentYear - ibirth;
		}
		return age;
	}
	public bool isMarried()
	{
		bool retMarried = false;
		foreach (FamilyEvent chkEvent in myEvents)
		{
			if (chkEvent.EventType == FamilyEvent.FamilyEventType.Marriage)
				retMarried = true;
		}
		return retMarried;
	}
	public string marriageDate()
	{
		string marriage = "NO MARRIAGE EVENT"; 
		foreach (FamilyEvent chkEvent in myEvents)
		{	
			if (chkEvent.EventType == FamilyEvent.FamilyEventType.Marriage)
			{
                marriage = chkEvent.Date;                    
			}
		}
		return marriage;
	}
	
	public int ageAtDeath()
	{
		int ibirth = 0;
		int ideath = 0;
		int retAgeAtDeath = 0;

        try
        {
            DateTime convertedDate = DateTime.Parse(this.Birth);
            ibirth = convertedDate.Year;
        }
        catch (FormatException)
        {
            Debug.Log(string.Format("Unable to Parse this Birth Date: '{0}'.", this.Birth));
        }
        try
        {
            DateTime convertedDate = DateTime.Parse(this.Death);
            ideath = convertedDate.Year;
        }
        catch (FormatException)
        {
            Debug.Log(string.Format("Unable to Parse this Death Date: '{0}'.", this.Death));
        }

		retAgeAtDeath = ideath - ibirth;

		return retAgeAtDeath;
	}

	public bool isDead()
	{
		bool retDeath = false;
		if (this.Death != "")
			retDeath = true;
		return retDeath;
	}

	public bool isAlive()
	{
		bool retAlive = false;
		if (this.Death == "")
			retAlive = true;
		return retAlive;
	}

	public string GetText(int currentYear, int personIndex)
	{
		string age = this.ageText(currentYear);

		string retString = "Name=" + this.Name + "(" + personIndex + "), Sex=" + this.GetSex() + ", Birth=" + this.Birth + ", Death=" + 
			(this.Death=="" ? ("Living, Age=" + age) : (this.Death + " age at death=" + ageAtDeath())) + "\n";
		foreach (FamilyEvent chkEvent in myEvents)
		{
			retString += chkEvent.GetText() + "\n";
		}	
		return retString;
	}

}

