using UnityEngine;
using System.Collections;

public class Interactable{
	string name;
	int gx;
	int gy;
	bool used;

	public Interactable(string name,int gx,int gy){
		this.name = name;
		this.gx = gx;
		this.gy = gy;
		this.used = false;
	}

	public int getX(){
		return this.gx;
	}

	public int getY(){
		return this.gy;
	}

	public bool isUsed(){
		return this.used;
	}
}	


