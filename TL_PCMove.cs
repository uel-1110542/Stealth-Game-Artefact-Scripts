using UnityEngine;

public class TL_PCMove : MonoBehaviour {

	//Variables
	private TL_LevelMediator LM_Script;
	public float fl_PCSpeed;
    private CharacterController cc_PC;


	
	void Start()
	{
        //Find the level map and obtain the script
		LM_Script = GameObject.Find ("Level_Map").GetComponent<TL_LevelMediator>();

        //Obtain the character controller
        cc_PC = GetComponent<CharacterController>();

        //Set default speed
		fl_PCSpeed = 4.5f;
	}
	
	void FixedUpdate()
	{
        //Method for moving the PC
		MovePC();
	}

	void MovePC()
	{
        //Declare vector3 variable
        Vector3 v3_MovePos;

        //Set the vector3 variable as the raw input from both vertical and horizontal keys as normalized
        v3_MovePos = new Vector3((Input.GetAxisRaw("Vertical")), 0f, (Input.GetAxisRaw("Horizontal"))).normalized;

        //Move the PC by the character controller
        cc_PC.Move(v3_MovePos * fl_PCSpeed * Time.deltaTime);
    }

	void OnTriggerStay(Collider col_obj)
	{
        //If the player has touched the end of the level
		if(col_obj.gameObject.tag == "EndGame")
		{
            //Set the end level to be true
            LM_Script.bl_EndLevel = true;

            //Freeze the game
            Time.timeScale = 0f;
        }
	}

}