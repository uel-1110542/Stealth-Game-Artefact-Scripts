using UnityEngine;

public class TL_CameraFollowPC : MonoBehaviour {

	//Variables
	private GameObject go_PC;
	private TL_PCStats PCScript;



	void Start()
	{
        //Find the PC
		go_PC = GameObject.Find ("pf_PC(Clone)");

        //Obtain the script from the PC
		PCScript = go_PC.GetComponent<TL_PCStats>();
	}

	void Update()
	{
        //If the PC's health is above 0
		if(PCScript.fl_Health > 0)
		{
            //Move the camera position to centre the player's position
			transform.position = new Vector3(go_PC.transform.position.x + 2f, go_PC.transform.position.y + 13f, go_PC.transform.position.z);
		}
	}

}
