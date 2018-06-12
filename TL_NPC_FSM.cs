using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class TL_NPC_FSM : MonoBehaviour {

	//Variables for NPC
	public string st_NPC_State;
	public string st_NPC_PrevState;
	public float fl_NPC_MaxHealth;
	public float fl_NPC_Health;
	public float fl_NPC_Attack;
    public float fl_NPC_FleeingDist;
	private float fl_NPC_Delay = 1.25f;
	private float fl_NPC_AttackCooldown;

    //Materials
	public Material mt_Alerted;
	public Material mt_Patrolling;
	public Material mt_ReturningToPost;
	public Material mt_Fleeing;

	//Variables for NPC movement
	public Vector3 v3_NPC_TargetPos;
	public bool bl_NPCMoving;
	
	//Variables for range
	private float fl_Min_Range;
	private float fl_Max_Range;
	private float fl_Attack_Range;

	private TL_LevelMediator LM_Script;
	public GameObject go_PC;
	public GameObject go_Bullet;
	private TL_PCStats PCScript;

	//Variables for Waypoints
	public int in_CurrentWaypoint;
	public int in_Trail_ID;
	public List<GameObject> WP_List;
	public bool bl_WP_Moving = false;
	public bool bl_Increment = true;
	private Vector3 v3_OriginalTargetPos;
    
	public GameObject go_SmokeAOE;
	public GameObject go_TextMesh;
	public GameObject go_TextMeshClone;
    public TL_NPCLineOfSight LineOfSightScript;
	private TL_RecordData DataScript;
	public bool bl_InvisSeen = false;
	private GameObject go_Light;
	public Light li_Light;
    private NavMeshAgent nm_Agent;
    private bool IsPathObstructed;
    public bool SmokeGrenadeSeen;



    void Start()
	{
		//Set initial values
		st_NPC_State = "Patrol";
		st_NPC_PrevState = st_NPC_State;
		fl_NPC_MaxHealth = 8f;
		fl_NPC_Health = fl_NPC_MaxHealth;
		fl_NPC_Attack = 2f;
		fl_Attack_Range = 4f;
		in_CurrentWaypoint = 0;

        //Create the text mesh and set the position
		go_TextMeshClone = Instantiate(go_TextMesh, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity);

        //Set the rotation
        go_TextMeshClone.transform.eulerAngles = new Vector3(90, -90, 0);

        //Obtain the Nav Mesh Agent component
        nm_Agent = GetComponent<NavMeshAgent>();

        //Set default target position
        v3_NPC_TargetPos = transform.position;

        //Locate gameobjects and obtain their components
        LineOfSightScript = GetComponentInChildren<TL_NPCLineOfSight>();
        LM_Script = GameObject.Find ("Level_Map").GetComponent<TL_LevelMediator>();
		DataScript = GameObject.Find ("Data_Recorder").GetComponent<TL_RecordData>();
        go_PC = GameObject.Find("pf_PC(Clone)");
		PCScript = go_PC.GetComponent<TL_PCStats>();
	}

	void Update()
	{
        //Moves and animates the NPC
		MoveNPC();

        //Handles the state triggers
		StateTriggers();

        //If the text mesh exists
		if(go_TextMeshClone != null)
		{
            //Set the position of the text mesh
			go_TextMeshClone.transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
		}        
    }

    //Method for setting the ID's for trails
	public void ID_TrailAssignment(int in_ID)
	{
		in_Trail_ID = in_ID;
	}
    
	//Method for setting a new state
	public void SetNewState(string st_state)
	{
		st_NPC_State = st_state;
	}

	//Method for obtaining a previous state
	public string ObtainPrevState()
	{
		return st_NPC_State;
	}

	void StateTriggers()
	{
		switch(st_NPC_State)
		{
		case "Patrol":
            //Set default minimum and maximum range values
            fl_Min_Range = 0f;
			fl_Max_Range = 40f;
            
            //Set default material color to green
			transform.GetComponent<Renderer>().material = mt_Patrolling;
            
            //Set the text to blank
			go_TextMeshClone.GetComponent<TextMesh>().text = "";
            
            //Move the NPC around patrol points
			PatrolPoints();
			break;

		case "Pursue":
			//Resumes the movement of the NPC
			nm_Agent.isStopped = false;
            
            //Set default minimum and maximum range values
            fl_Min_Range = 4f;
			fl_Max_Range = 8f;
            
            //If the distance between the PC and the NPC is less than or equal to the attack range and the raycast
            //does not hit a wall while in pursuit while the PC is still alive
            if (go_PC != null)
            {
                //Set the Target Position to the PC
                v3_NPC_TargetPos = go_PC.transform.position;
                
                //If the PC is within attacking range and the closest collider is the PC
                if (Vector3.Distance(go_PC.transform.position, transform.position) <= fl_Attack_Range && LineOfSightScript.ClosestCol != null && LineOfSightScript.ClosestCol.tag == "PC")
                {
                    //Set the previous state as the current one
                    st_NPC_PrevState = st_NPC_State;
                    
                    //Obtain the previous state
                    ObtainPrevState();
                    
                    //Sets the Attack state
                    SetNewState("Attack");
                }
            }

            //If the PC moves outside the maximum range of the pursuit
			if(Vector3.Distance(go_PC.transform.position, transform.position) > fl_Max_Range)
			{
                //Increase amount of times the player eluded the guard
				DataScript.in_GuardsEluded++;

				//Sets the ReturnToPost state
				SetNewState("ReturnToPost");
			}
            //Set the material to a red color
            transform.GetComponent<Renderer>().material = mt_Alerted;
			break;

		case "Attack":
            //Set minimum range for pursuit
			fl_Min_Range = 4f;
            
            //Set the material to a red color
            transform.GetComponent<Renderer>().material = mt_Alerted;
            
            //If the PC moves outside the minimum range or the raycast doesn't collide with the player
            if (Vector3.Distance(transform.position, go_PC.transform.position) > fl_Min_Range || LineOfSightScript.ClosestCol != null && LineOfSightScript.ClosestCol.tag != "PC")
			{
				//Sets the Pursue state
				SetNewState("Pursue");
			}
			else
			{
                //Stops the NPC
                nm_Agent.isStopped = true;
                
                //Attack the PC
                AttackPC();	
            }
            break;

		case "ReturnToPost":
			//Resumes the movement of the NPC
			nm_Agent.isStopped = false;
            
            //Set default minimum and maximum range values
            fl_Min_Range = 0f;
			fl_Max_Range = 15f;
            
            //Set the material to a yellow color
			transform.GetComponent<Renderer>().material = mt_ReturningToPost;
            
            //Set light color to default
			li_Light.color = Color.white;

            //Set the Target Position to the current waypoint
            v3_NPC_TargetPos = WP_List[in_CurrentWaypoint].transform.position;

            //If the distance of the waypoint is nearby, set the NPC's state to Patrol
            if (Vector3.Distance (WP_List[in_CurrentWaypoint].transform.position, transform.position) <= 1f)
			{
				//Sets the Patrol state
				SetNewState("Patrol");
			}
			break;

		case "Flee":
            //Set default minimum and maximum range values
            fl_Min_Range = 0f;
			fl_Max_Range = 5f;

            //Set the text to blank
            go_TextMeshClone.GetComponent<TextMesh>().text = "";

            //Make the NPC run away from the smoke grenade
            Fleeing();
            
            //Set material to a cyan color
			transform.GetComponent<Renderer>().material = mt_Fleeing;
			break;
		}
	}

	void Fleeing()
	{
        //If the smoke grenade does not exist anymore
		if(go_SmokeAOE == null)
		{
			//Sets the ReturnToPost state
			SetNewState("ReturnToPost");
		}
		else if(!SmokeGrenadeSeen)      //If the smoke grenade hasn't been seen
        {
            //Calculate the distance between itself and the smoke grenade while ignoring the Y position
            float Distance = Vector3.Distance(new Vector3(transform.position.x, 1f, transform.position.z), new Vector3(go_SmokeAOE.transform.position.x, 1f, go_SmokeAOE.transform.position.z));

            //If the distance is less than the maximum fleeing distance
            if (Distance < fl_NPC_FleeingDist)
            {
                //Calculate the direction between itself and the smoke grenade while ignoring the Y position
                Vector3 Direction = new Vector3(transform.position.x, 1f, transform.position.z) - new Vector3(go_SmokeAOE.transform.position.x, 1f, go_SmokeAOE.transform.position.z);
                
                //Calculate the new position
                Vector3 NewPosition = new Vector3(transform.position.x, 1f, transform.position.z) + Direction;

                //Set the target position to the new position
                v3_NPC_TargetPos = NewPosition;
            }
            //Create local variable for setting a new path
            NavMeshPath NewPath = new NavMeshPath();

            //Calculate a path for the target position and store it in a variable
            nm_Agent.CalculatePath(v3_NPC_TargetPos, NewPath);

            //Replace the current path as the stored path
            nm_Agent.path = NewPath;
        }
    }

	void MoveNPC()
	{
        //Set the destination of the Nav Mesh Agent
        nm_Agent.SetDestination(v3_NPC_TargetPos);
    }

    //Method for assigning the NPC's waypoints
	public void WP_Assignment(List<GameObject> go_WP)
	{
		WP_List = go_WP;
	}

	void PatrolPoints()
	{
        //If the NPC is patrolling
		if(st_NPC_State == "Patrol")
		{
            //Set the current target position as the current waypoint
			v3_NPC_TargetPos = WP_List[in_CurrentWaypoint].transform.position;

            //If the NPC reaches the waypoint
			if(Vector3.Distance (transform.position, WP_List[in_CurrentWaypoint].transform.position) < nm_Agent.stoppingDistance)
			{
                //If the current waypoint starts at 0
				if(in_CurrentWaypoint == 0)
				{
					bl_Increment = true;
				}
				else if(in_CurrentWaypoint == WP_List.Count - 1)
				{
                    //If the current waypoint reaches the maximum index
					bl_Increment = false;
				}
				
				if(bl_Increment)
				{
                    //Increment the index of the current waypoint
					in_CurrentWaypoint++;
				}
				else
				{
                    //Decrement the index of the current waypoint
                    in_CurrentWaypoint--;
				}
			}
		}
	}

	void AttackPC()
	{
        //Rotates the NPC towards the PC
        transform.LookAt(go_PC.transform.position);

        //If the attack cooldown is less than time
		if(fl_NPC_AttackCooldown < Time.time)
		{
            //Instantiate the bullet
            GameObject go_BulletClone = Instantiate(go_Bullet, transform.TransformPoint(Vector3.forward), transform.rotation) as GameObject;
			
			//Adds forward force onto the rigidbody of the NPC bullet
			go_BulletClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 500f);
			
			//Adds the cooldown of the NPC attack
			fl_NPC_AttackCooldown = fl_NPC_Delay + Time.time;
		}
	}

	public void ReceiveDamage(float fl_damage)
	{
        //Reduce the NPC's health by the damage received
		fl_NPC_Health -= fl_damage;

        //Record the damage dealt by the PC
		DataScript.in_PCDamageDealt += (int)fl_damage;

        //Record the damage received from the PC
		DataScript.in_NPCDamageReceived += (int)fl_damage;

        //If the NPC's current health is less than or equal to 0
		if(fl_NPC_Health <= 0f)
		{
            //Record the amount of times the PC has killed a guard
			DataScript.in_GuardsKilled++;

            //Destroy the text mesh and the NPC
			Destroy (go_TextMeshClone);
			Destroy (gameObject);
		}
	}

    void OnParticleCollision(GameObject go)
    {
        //If the NPC collides with the smoke particles
        if (go.tag == "SmokeAOE")
        {
            //Set the collided gameobject as the area of effect smoke
            go_SmokeAOE = go;

            //If the player has selected the dynamic AI
            if (LM_Script.bl_DynamicAI)
            {
                //If the seen weapon is a smoke grenade
                if (SmokeGrenadeSeen)
                {
                    //Set text to show gas mask equipped
                    go_TextMeshClone.GetComponent<TextMesh>().text = "Gas Mask Equipped";
                }
                else if (PCScript.bl_Invis)
                {
                    //If the player has gone invisible, set text to ?
                    go_TextMeshClone.GetComponent<TextMesh>().text = "?";
                }
                else
                {
                    //Reset the NPC's path
                    nm_Agent.ResetPath();

                    //Set the NPC's state to flee
                    SetNewState("Flee");
                }
            }
            else
            {
                //Reset the NPC's path
                nm_Agent.ResetPath();

                //Set the NPC's state to flee
                SetNewState("Flee");
            }
        }
    }

    void OnCollisionEnter(Collision Col)
    {
        //When the NPC hits a smoke grenade or a pistol bullet from the player
        if (Col.gameObject.tag == "SmokeGrenade" || Col.gameObject.tag == "PistolBullet")
        {
            //Set the smoke grenade's velocity to 0
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

}
