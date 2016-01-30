using UnityEngine;
using System.Collections;

public class Interactable{
	string name;
    public int gx { get; private set; }
    public int gy { get; private set; }
    bool used;

	public Interactable(string name,int gx,int gy){
		this.name = name;
		this.gx = gx;
		this.gy = gy;
		this.used = false;
	}

	public bool isUsed(){
		return this.used;
	}

    public void Interact()
    {
        Debug.Log("Interact!");
    }
}	


