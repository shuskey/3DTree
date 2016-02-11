using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CourtHouse
{
    public List<Family> allFamilies = new List<Family>();
    public MyPeople myPeople = new MyPeople();

    public CourtHouse()
    {

    }

    public void init()
    {
        allFamilies.Clear();
        Family zeroFamily = new Family(0, 0, 0, "");
        addToAllFamilies(zeroFamily);
        // This will serve as the root of the tree and Generation 0, Adam and Eve can be Generation 1
        myPeople.init();
    }


    public int StartFamily(int treePersonIndex, int treePersonSpouceIndex, string year)
    {
        Family myFamily = null;
        int familyIndex = -1;

        FamilyEvent marriageevent = MakeMarriageEvent(treePersonIndex, treePersonSpouceIndex, year ?? "");
        if (marriageevent != null)
        {
            myFamily = new Family(marriageevent.Generation, marriageevent.BridePersonIndex,
                marriageevent.GroompersonIndex, marriageevent.Date);
            familyIndex = addToAllFamilies(myFamily);
            marriageevent.FamilyIndex = familyIndex;
            myPeople.allPeople[marriageevent.BridePersonIndex].MarriedFamilyIndex = familyIndex;
            myPeople.allPeople[marriageevent.GroompersonIndex].MarriedFamilyIndex = familyIndex;
            myPeople.allPeople[marriageevent.BridePersonIndex].AddEvent(marriageevent);
            myPeople.allPeople[marriageevent.GroompersonIndex].AddEvent(marriageevent);
        }

        return familyIndex;
    }

    public FamilyEvent MakeMarriageEvent(int treePersonIndex, int treePersonSpouceIndex, string year)
    {
        FamilyEvent retMarriageEvent = null;

        // Okay, who is the Man, and who is the woman, we need to know
        // Assume this for now
        var boyPersonIndex = treePersonIndex;
        var girlPersonIndex = treePersonSpouceIndex;

        if (myPeople.allPeople[treePersonIndex].Sex == TreePerson.PersonSex.Female ||
            myPeople.allPeople[treePersonSpouceIndex].Sex == TreePerson.PersonSex.Male)
        {
            boyPersonIndex = treePersonSpouceIndex;
            girlPersonIndex = treePersonIndex;
        }

        if (girlPersonIndex != 0)
        {
            retMarriageEvent = new FamilyEvent(myPeople, allFamilies, FamilyEvent.FamilyEventType.Marriage, year, girlPersonIndex,
                boyPersonIndex);
        }
        return retMarriageEvent;
    }

    public int addToAllFamilies(Family family)
    {
        allFamilies.Add(family);
        return allFamilies.IndexOf(family);
    }

    public string getAllFamiliesText(int currentYear)
    {
        string retString = "";
        foreach (Family myFamily in allFamilies)
            retString += "#" + allFamilies.IndexOf(myFamily) + "\r\n" + myFamily.GetTextCourtHouse(myPeople, currentYear);

        return retString;
    }


}
