using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPeople
{
	public List<TreePerson> allPeople;
	
	public MyPeople()
	{
		allPeople = new List<TreePerson>();
	}
    public void init()
    {
        allPeople.Clear();
    }
	

    public int addToAllPeople(TreePerson treePerson)
	{
		allPeople.Add (treePerson);
		return allPeople.IndexOf(treePerson);
	}
	

}


