using System;
using UnityEngine;
using System.Collections;


public enum InteractableType
{
    BED,
    REFRIDGERATOR,
}

public class Interactable{
    public int gx { get; private set; }
    public int gy { get; private set; }
    private Action interactAction;

	public Interactable(int gx,int gy, Action interactAction)
	{
	    this.interactAction = interactAction;
		this.gx = gx;
		this.gy = gy;
	}

    public void Interact()
    {
        interactAction();
    }
}	


