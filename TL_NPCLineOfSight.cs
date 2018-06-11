using UnityEngine;
using System.Collections.Generic;

public class TL_NPCLineOfSight : MonoBehaviour {

    //Variables
    private GameObject PC;
	private TL_NPC_FSM NPCScript;
	private TL_Inventory InventoryScript;
    private TL_RecordData DataScript;
    public List<Collider> ls_Colliders = new List<Collider>();
	public List<Collider> ls_Sighted = new List<Collider>();
    public Collider ClosestCol;
	private TL_LevelMediator LM_Script;
	private TL_PCStats PCScript;
	public float fl_ClosestDis;
	public Light li_Light;
    private float Angle;


    
    void Start()
	{
        //Locate the PC
        PC = GameObject.FindGameObjectWithTag("PC");

        //Obtain the script from the parent gameobject
        NPCScript = GetComponentInParent<TL_NPC_FSM>();

        //Locate the gameobjects and obtain their scripts
        PCScript = PC.GetComponent<TL_PCStats>();
        LM_Script = GameObject.Find ("Level_Map").GetComponent<TL_LevelMediator>();		
        DataScript = GameObject.Find("Data_Recorder").GetComponent<TL_RecordData>();
    }

    void Update()
    {
        //Method for calculating the angle between itself and the player
        PCDetection();

        //Method for calculating the nearest colliders
        CalculateNearestObj(ls_Colliders);
    }

    void PCDetection()
    {
        //Look at the player
        transform.LookAt(PC.transform);

        //Calculate the direction between itself and the PC
        Vector3 VisionAngle = transform.parent.position - PC.transform.position;

        // Retrieve the angle between the Vector3 and the parent of the transform facing forward
        Angle = Vector3.Angle(VisionAngle, transform.parent.forward);
    }

    Collider CalculateNearestObj(List<Collider> ListColliders)
    {
        //Set default values
        ClosestCol = null;
        fl_ClosestDis = Mathf.Infinity;
        Vector3 CurrentPos = transform.parent.position;

        //Loop through the list of colliders
        foreach (Collider Col in ListColliders)
        {
            //Calculate the direction from the collider's position with the current position
            Vector3 Direction = Col.transform.position - CurrentPos;

            //Square the direction
            float DistSquared = Direction.sqrMagnitude;

            //If the distance squared is less than the closest distance
            if (DistSquared < fl_ClosestDis)
            {
                //Set the closest distance to distance squared
                fl_ClosestDis = DistSquared;

                //Set the closest collider from the list of colliders
                ClosestCol = Col;
            }
        }

        //If the detected collider still exists
        if (ClosestCol != null)
        {
            //If the calculated angle is between 130 and 180, and the NPC's state isn't flee and the closest collider is the PC
            if (Angle >= 130f && Angle <= 180f && NPCScript.st_NPC_State != "Flee" && ClosestCol.tag == "PC")
            {
                //Obtain the inventory script from the player
                InventoryScript = PC.GetComponent<TL_Inventory>();

                //If the player is invisible
                if (PCScript.bl_Invis)
                {
                    //If the dynamic AI is selected and the NPC is not patrolling
                    if (LM_Script.bl_DynamicAI && NPCScript.st_NPC_State != "Patrol")
                    {
                        //Set the bool to false
                        NPCScript.SmokeGrenadeSeen = false;

                        //Set the bool equal to the player being invisible
                        NPCScript.bl_InvisSeen = PCScript.bl_Invis;

                        //Display the text from the text mesh
                        NPCScript.go_TextMeshClone.GetComponent<TextMesh>().text = "Ultraviolet Light Used";

                        //Change the color of the light
                        li_Light.color = new Color(0, 1, 0, 1);
                    }
                    else
                    {
                        //If the NPC is not patrolling
                        if (NPCScript.st_NPC_State != "Patrol")
                        {
                            //If the dynamic AI is not selected, set the text mesh to ?
                            NPCScript.go_TextMeshClone.GetComponent<TextMesh>().text = "?";

                            //Change state to return to post
                            NPCScript.st_NPC_State = "ReturnToPost";
                        }
                    }
                }
                else
                {
                    //If the dynamic AI is selected
                    if (LM_Script.bl_DynamicAI)
                    {
                        //If the active weapon is the smoke grenade
                        if (InventoryScript.go_ActiveWeapon != null && InventoryScript.go_ActiveWeapon.name == "pf_SmokeGrenade")
                        {
                            //Sett the bool to true
                            NPCScript.SmokeGrenadeSeen = true;
                        }
                    }
                    //If the player is not invisible and the NPC's state is either patrol or return to post
                    if (NPCScript.st_NPC_State == "Patrol" || NPCScript.st_NPC_State == "ReturnToPost")
                    {
                        //Increment amount ot times the PC has been detected
                        DataScript.in_PCDetected++;
                    }
                    //Change state to attack
                    NPCScript.SetNewState("Attack");

                    //Obtain the previous state
                    NPCScript.ObtainPrevState();
                }
            }
        }
        //Return the closest collider
        return ClosestCol;
    }

    void OnTriggerStay(Collider col_gameobj)
	{
        //If the entered collider does not exist in the list of colliders
		if (ls_Colliders.Find (x=> x == col_gameobj) == null)
		{
            //If the collided gameobject has the tag PC or Wall
            if (col_gameobj.transform.tag == "PC" || col_gameobj.transform.tag == "Wall")
            {
                //Add the collided gameobject in the list
                ls_Colliders.Add(col_gameobj);
            }
		}
	}

	void OnTriggerExit(Collider col_obj)
	{
        //Remove the collided gameobject from the list
        ls_Colliders.Remove(col_obj);
	}
}
