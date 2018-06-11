using UnityEngine;

public class TL_PistolBulletScript : MonoBehaviour {

	//Variables
	private TL_NPC_FSM NPCScript;
	private TL_RecordData DataScript;
	private float fl_Attack = 2f;


	void Start()
	{
        //Locate the gameobject and obtain the script
		DataScript = GameObject.Find ("Data_Recorder").GetComponent<TL_RecordData>();
	}

	void Update()
	{
        //Destroy the gameobject within a fraction of a second
		Destroy(gameObject, 0.4f);
	}

	void OnCollisionEnter(Collision Col)
	{
        //If the projectile collides with the NPC
		if(Col.gameObject.tag == "NPC")
		{
            //Obtain the script from the collided gameobject
			NPCScript = Col.gameObject.GetComponent<TL_NPC_FSM>();

            //Send a message to the NPC
            Col.gameObject.SendMessage("ReceiveDamage", fl_Attack, SendMessageOptions.DontRequireReceiver);

            //If the NPC is either in the patrol, return to post or the flee state
            if (NPCScript.st_NPC_State == "Patrol" || NPCScript.st_NPC_State == "ReturnToPost" || NPCScript.st_NPC_State == "Flee")
			{
                //Increment the player detected variable
				DataScript.in_PCDetected++;
			}
            //Change state to pursue
			NPCScript.st_NPC_State = "Pursue";
		}
        //Destroy the gameobject
		Destroy(gameObject);
	}

}
