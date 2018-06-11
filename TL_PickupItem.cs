using UnityEngine;

public class TL_PickupItem : MonoBehaviour {

	//Variables
	public string st_Tooltip;
	public Sprite sp_Icon;
	public GameObject go_Item;
    
    
	void Update()
	{
        //Rotate the gameobject on the Z position at 45 multiplied by delta time
		transform.Rotate (0, 0, 45 * Time.deltaTime);
	}

}
