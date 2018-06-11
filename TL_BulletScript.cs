using UnityEngine;

public class TL_BulletScript : MonoBehaviour {

	private TL_NPC_FSM NPCScript;
	private TL_RecordData DataScript;


	void Start()
	{
        //Locate the gameobjects and obtain their scripts
		NPCScript = GameObject.Find ("pf_NPCGuard(Clone)").GetComponent<TL_NPC_FSM>();
		DataScript = GameObject.Find("Data_Recorder").GetComponent<TL_RecordData>();

        //Destroy the bullet gameobject 1.5 seconds later
		Destroy(gameObject, 1.5f);
	}

	void OnCollisionEnter(Collision collide)
	{
        //If the collided gameobject is the player
		if(collide.gameObject.tag == "PC")
		{
            //Record the damage dealt to the PC
			DataScript.in_NPCDamageDealt += (int)NPCScript.fl_NPC_Attack;

            //Send the message to the PC
			collide.gameObject.SendMessage("ReceiveDamage", NPCScript.fl_NPC_Attack, SendMessageOptions.DontRequireReceiver);
		}
        //Destroy the gameobject
		Destroy(gameObject);
	}

}