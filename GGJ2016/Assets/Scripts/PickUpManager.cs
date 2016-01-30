
public enum PickUps{
	None,
	Toothbrush,
	Milk,
	Cereal,
	Bowl,
	Coffee,
	Clothes,
	Newspaper,
	Keys,
	Wallet,
	Briefcase,
	Shoes
}

public class PickUpable{
	PickUps pickUps;

	public PickUpable(PickUps pickUps){
		this.pickUps = pickUps;
	}

	public PickUps getName(){
		return this.pickUps;
	}
}
