using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;


public class TL_LevelMediator : MonoBehaviour {

	//Variables
	public GameObject[,] go_LevelArea = new GameObject[39, 16];
	public GameObject go_Wall;
	public GameObject go_PC;
	public GameObject go_Cam;
	public GameObject go_NPC;
	public GameObject go_HealthPill;
	public GameObject go_Pistol;
	public GameObject go_Ammo;
	public GameObject go_SmokeGrenade;
	public GameObject go_EndGame;
	private GameObject go_NPCClone;

	private TL_NPC_FSM NPCScript;
	private TL_PCStats PCScript;
	private TL_RecordData DataScript;
	public GameObject go_WP;
	private GameObject go_WPClone;
	public bool bl_EndLevel = false;
	public bool bl_DynamicAI;
	public bool bl_Selected = false;
	public bool bl_SavedData = false;

	//List of waypoints
	public List<GameObject> WP_List_A = new List<GameObject>();
	public List<GameObject> WP_List_B = new List<GameObject>();
	public List<GameObject> WP_List_C = new List<GameObject>();
	public List<GameObject> WP_List_D = new List<GameObject>();
	public List<GameObject> WP_List_E = new List<GameObject>();
	public List<GameObject> WP_List_F = new List<GameObject>();
	public List<GameObject> WP_List_G = new List<GameObject>();
	public List<GameObject> WP_List_H = new List<GameObject>();
	public List<GameObject> WP_List_I = new List<GameObject>();
	private List<List<GameObject>> Master_List = new List<List<GameObject>>();

	private int in_Master_List_Index = 0;
	public int[,] in_LevelMap;
	public int in_XPos;
	public int in_ZPos;
    private List<int> PreviousValues = new List<int>();

    public GameObject GetLevelAreaObj()
	{
		return go_LevelArea[in_XPos, in_ZPos];
	}
	
	public void SetLevelAreaObj (GameObject go)
	{
		go_LevelArea[in_XPos, in_ZPos] = go;
	}

	void Awake()
	{
		DataScript = GameObject.Find("Data_Recorder").GetComponent<TL_RecordData>();

        //Set the layout of the level with a 2D int array
		in_LevelMap = new int[,]
		{
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 17, 17, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 1, 0, 13, 0, 1, 1, 1, 0, 0, 1, 1},
			{1, 7, 2, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 1, 1},
			{1, 0, 0, 1, 1, 2, 1, 1, 1, 11, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 10, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 0, 0, 0, 1, 15, 1, 0, 0, 0, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 10, 1, 1, 1, 11, 1, 1, 0, 0, 1, 1},
			{1, 0, 0, 1, 1, 0, 1, 1, 1, 2, 1, 1, 0, 0, 1, 1},
			{1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 2, 8, 1, 1},
			{1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 16, 0, 1, 1},
			{1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 12, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 0, 2, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 1},
			{1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1},
			{1, 0, 0, 1, 1, 1, 1, 0, 13, 1, 1, 1, 1, 0, 0, 1},
			{1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1},
			{1, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 2, 0, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 6, 0, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1},
			{1, 1, 1, 3, 0, 0, 0, 0, 0, 3, 1, 1, 1, 0, 5, 1},
			{1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1},
			{1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1},
			{1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1},
			{1, 1, 1, 2, 0, 0, 0, 0, 4, 0, 1, 1, 1, 0, 2, 1},
			{1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1},
			{1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1},
			{1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1},
			{1, 1, 1, 0, 0, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 1},
			{1, 1, 1, 0, 0, 1, 1, 1, 4, 0, 0, 0, 0, 4, 0, 1},
			{1, 1, 1, 3, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1},
			{1, 9, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1},
			{1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 5, 14, 5, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		};
		
        //Loop through the 2D int array
		for(int x = 0; x < in_LevelMap.GetLength(0); x++)
		{
			for(int z = 0; z < in_LevelMap.GetLength(1); z++)
			{
                //If the gameobject level area is null in the 2D array
                //Map out in the 2D int array, which gameobject gets spawned in the scene depending on what value it is
				if(go_LevelArea[x, z] == null)
				{
                    //For example, if the value is 1 in the 2D int array, it spawns a wall in the level
					if(in_LevelMap[x, z] == 1)
					{
						GameObject go_WallClone;
						go_WallClone = Instantiate (go_Wall, new Vector3(x, 1f, z), Quaternion.identity);
						go_LevelArea[x, z] = go_WallClone;
					}

					if(in_LevelMap[x, z] == 2)
					{
                        //Declare local variable for the nav mesh agent
                        NavMeshAgent NPCAgent;

                        //Instantiate the NPC
						go_NPCClone = Instantiate (go_NPC, new Vector3(x, 1f, z), Quaternion.identity);

                        //Set the NPC to the gameobject array
                        go_LevelArea[x, z] = go_NPCClone;

                        //Obtain the nav mesh agent from the NPC
                        NPCAgent = go_LevelArea[x, z].GetComponent<NavMeshAgent>();

                        //Set a random number between 1 to 100 to the avoidance priority
                        NPCAgent.avoidancePriority = RandomizeNumber(1, 100);
                    }

					if(in_LevelMap[x, z] == 3)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_A.Add (go_WPClone);
						WP_List_A.Reverse();
					}

					if(in_LevelMap[x, z] == 4 )
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_B.Add (go_WPClone);
					}

					if(in_LevelMap[x, z] == 5)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_C.Add (go_WPClone);
					}

					if(in_LevelMap[x, z] == 6)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_D.Add (go_WPClone);
					}

					if(in_LevelMap[x, z] == 7)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_E.Add (go_WPClone);
						WP_List_E.Reverse();
					}

					if(in_LevelMap[x, z] == 8)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_F.Add (go_WPClone);
					}

					if(in_LevelMap[x, z] == 9)
					{
						GameObject go_PCClone = Instantiate (go_PC, new Vector3(x, 1f, z), Quaternion.identity);;
						PCScript = go_PCClone.GetComponent<TL_PCStats>();

						GameObject go_CamClone = Instantiate (go_Cam, new Vector3(go_PCClone.transform.position.x + 2f, go_PCClone.transform.position.y + 13f, go_PCClone.transform.position.z), Quaternion.identity);
						go_CamClone.transform.localEulerAngles = new Vector3(76f, 270f, 0);

						go_LevelArea[x, z] = go_PCClone;
					}

					if(in_LevelMap[x, z] == 10)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_G.Add (go_WPClone);
					}

					if(in_LevelMap[x, z] == 11)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_H.Add (go_WPClone);
					}

					if(in_LevelMap[x, z] == 12)
					{
						go_WPClone = Instantiate (go_WP, new Vector3(x, 1f, z), Quaternion.identity);
						WP_List_I.Add (go_WPClone);
						WP_List_I.Reverse ();
					}

					if(in_LevelMap[x, z] == 13)
					{
						GameObject HealthPillClone = Instantiate (go_HealthPill, new Vector3(x, 1f, z), Quaternion.identity);
                        HealthPillClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    }

					if(in_LevelMap[x, z] == 14)
					{
						GameObject PistolClone = Instantiate (go_Pistol, new Vector3(x, 1f, z), Quaternion.identity);
                        PistolClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    }

					if(in_LevelMap[x, z] == 15)
					{
						GameObject SmokeGrenadeClone = Instantiate (go_SmokeGrenade, new Vector3(x, 1f, z), Quaternion.identity);
                        SmokeGrenadeClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    }

					if(in_LevelMap[x, z] == 16)
					{
						GameObject AmmoClone = Instantiate (go_Ammo, new Vector3(x, 1f, z), Quaternion.identity);
                        AmmoClone.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                    }

					if(in_LevelMap[x, z] == 17)
					{
						GameObject go_EndGameClone = Instantiate (go_EndGame, new Vector3(x, 1f, z), Quaternion.identity);
						go_EndGameClone.transform.eulerAngles = new Vector3(90f, 270f, 0);
					}
				}
			}
		}
		//Temporary lists to store certain elements from another list to swap them
		List <GameObject> Swap_List_E = new List<GameObject>();
		List <GameObject> Swap_List_I = new List<GameObject>();
		List <GameObject> Swap_List_C = new List<GameObject>();

		Master_List.Add(WP_List_F);
		for (int f = 0; f < WP_List_F.Count; f++)
		{
			WP_List_F[f].name = f.ToString();
		}

		Master_List.Add(WP_List_G);
		for (int g = 0; g < WP_List_G.Count; g++)
		{
			WP_List_G[g].name = g.ToString();
		}

		Master_List.Add(WP_List_H);
		for (int h = 0; h < WP_List_H.Count; h++)
		{
			WP_List_H[h].name = h.ToString();
		}

		Swap_List_E.Add (WP_List_E[2]);				//Adds index 2 element of the WP List into index 1 element of the temporary list
		Swap_List_E.Add (WP_List_E[1]);				//Adds index 1 element of the WP List into index 2 element of the temporary list
		WP_List_E.RemoveRange(1, 2);				//Removes a range of old elements from the list
		WP_List_E.Insert(1, Swap_List_E[0]);		//Inserts index 0 element of the temporary list into index 1 of the WP List
		WP_List_E.Insert(2, Swap_List_E[1]);		//Inserts index 1 element of the temporary list into index 2 of the WP List

		//Adds the WP List into the master list
		Master_List.Add(WP_List_E);
		for (int e = 0; e < WP_List_E.Count; e++)
		{
			WP_List_E[e].name = e.ToString();
		}

		Master_List.Add(WP_List_D);
		for (int d = 0; d < WP_List_D.Count; d++)
		{
			WP_List_D[d].name = d.ToString();
		}

		Swap_List_I.Add (WP_List_I[2]);			//Adds index 2 element of the WP List into index 1 element of the temporary list
		Swap_List_I.Add (WP_List_I[1]);			//Adds index 1 element of the WP List into index 2 element of the temporary list
		WP_List_I.RemoveRange(1, 2);			//Removes a range of old elements from the list
		WP_List_I.Insert(1, Swap_List_I[0]);	//Inserts index 0 element of the temporary list into index 1 of the WP List
		WP_List_I.Insert(2, Swap_List_I[1]);	//Inserts index 1 element of the temporary list into index 2 of the WP List
		Master_List.Add(WP_List_I);
		for (int i = 0; i < WP_List_I.Count; i++)
		{
			WP_List_I[i].name = i.ToString();
		}

		Master_List.Add(WP_List_A);
		for (int a = 0; a < WP_List_A.Count; a++)
		{
			WP_List_A[a].name = a.ToString();
		}

		Swap_List_C.Add (WP_List_C[2]);				//Adds index 2 element of the WP List into index 1 element of the temporary list
		Swap_List_C.Add (WP_List_C[1]);				//Adds index 1 element of the WP List into index 2 element of the temporary list
		WP_List_C.RemoveRange(1, 2);				//Removes a range of old elements from the list
		WP_List_C.Insert(1, Swap_List_C[0]);		//Inserts index 0 element of the temporary list into index 1 of the WP List
		WP_List_C.Insert(2, Swap_List_C[1]);		//Inserts index 1 element of the temporary list into index 2 of the WP List
		Master_List.Add(WP_List_C);
		for (int c = 0; c < WP_List_C.Count; c++)
		{
            //Name the waypoints depending on the value from the for loop
			WP_List_C[c].name = c.ToString();
		}

		Master_List.Add(WP_List_B);
		for (int b = 0; b < WP_List_B.Count; b++)
		{
            //Name the waypoints depending on the value from the for loop
            WP_List_B[b].name = b.ToString();
		}

		for(int x = 0; x < in_LevelMap.GetLength(0); x++)
		{
			for(int z = 0; z < in_LevelMap.GetLength(1); z++)
			{
				if(go_LevelArea[x, z] != null)
				{
                    //If the gameobject in the 2D array is the NPC
					if(go_LevelArea[x, z].transform.tag == "NPC")
					{
                        //Obtain the script from the NPC
						NPCScript = go_LevelArea[x, z].GetComponent<TL_NPC_FSM>();

                        //Assign the waypoints for the NPC
						NPCScript.WP_Assignment(Master_List[in_Master_List_Index]);

                        //Assign the ID for waypoints
						NPCScript.ID_TrailAssignment(in_Master_List_Index);

                        //Increment the index for the master list
						in_Master_List_Index++;
                        
                        //If it is more than 7, break the loop
						if(in_Master_List_Index > 7)
						{
							break;
						}
					}
				}
			}
		}
        //Build the navmesh
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
	}

    void Update()
    {
        //Display the time
        DisplayTime();

        //Display the scoreboard when the player finishes the level
        DisplayScoreboard();

        //Check if the player has selected a mode or not
        CheckPauseState();

        //Function for saving data
        SaveData();
    }

    int RandomizeNumber(int Min, int Max)
    {
        //Set a random number between a minimum and maximum value
        int RandomInt = Random.Range(Min, Max);

        //Add the random number in an int list
        PreviousValues.Add(RandomInt);

        //While the int list contains the same value as the randomized one
        while (PreviousValues.Contains(RandomInt))
        {
            //Re-randomize the number again until it is different
            RandomInt = Random.Range(Min, Max);
        }
        //Return the random int
        return RandomInt;
    }

    public void StandardAIMode()
    {
        //Switch bool to true to indicate that a mode has been selected
        bl_Selected = true;

        //Set bool to true to show instructions
        PCScript.bl_Instructions = true;

        //Set dynamic AI mode to false
        bl_DynamicAI = false;

        //Deactivate the choice box
        GameObject.Find("PC_Cam(Clone)/Canvas/ChoiceBox").SetActive(false);
    }

    public void DynamicAIMode()
    {
        //Switch bool to true to indicate that a mode has been selected
        bl_Selected = true;

        //Set bool to true to show instructions
        PCScript.bl_Instructions = true;

        //Set dynamic AI mode to true
        bl_DynamicAI = true;

        //Deactivate the choice box
        GameObject.Find("PC_Cam(Clone)/Canvas/ChoiceBox").SetActive(false);
    }

    void DisplayScoreboard()
    {
        //When the player has finished the level
        if (bl_EndLevel)
        {
            //Find the scoreboard in the canvas
            GameObject Scoreboard = GameObject.Find("PC_Cam(Clone)/Canvas/ScoreboardBox");

            //Activate the scoreboard
            Scoreboard.SetActive(true);

            //Obtain the text from the child gameobject
            Text ScoreboardText = Scoreboard.transform.Find("Scoreboard").GetComponent<Text>();

            //Display all of the data
            ScoreboardText.text = 
            "Completion Time: " + DataScript.in_Minutes + "mins & " + DataScript.in_Seconds + "secs" + "\n\n" +
            "PC Damage Received: " + DataScript.in_PCDamageReceived + "     PC Damage Dealt: " + DataScript.in_PCDamageDealt + "\n\n" +
            "Amount of times detected: " + DataScript.in_PCDetected + "     NPC Damage Received: " + DataScript.in_NPCDamageReceived + "\n\n" +
            "NPC Damage Dealt: " + DataScript.in_NPCDamageDealt + "     Items Collected: " + DataScript.in_ItemsCollected + "\n\n" +
            "Items Used: " + DataScript.in_ItemsUsed + "        Guards Killed: " + DataScript.in_GuardsKilled + "\n\n" +
            "Lowest PC Health: " + (int)PCScript.fl_Health + "      Amount of retries: " + DataScript.in_Retries + "\n\n" +
            "Invisibility Used: " + DataScript.in_InvisUsed;
        }
    }

    void DisplayTime()
    {
        //Find the play time display in the canvas
        GameObject PlaytimeDisplay = GameObject.Find("PC_Cam(Clone)/Canvas/PlayTimeBox");

        //Obtain the text from the child gameobject
        Text PlaytimeText = PlaytimeDisplay.GetComponentInChildren<Text>();

        //Display the play time in a format
        PlaytimeText.text = "Play Time\n" + string.Format("{00:00}:{1:00}", DataScript.in_Minutes, DataScript.in_Seconds);
    }

    void CheckPauseState()
    {
        if (!bl_Selected)
        {
            //If the player has not selected a mode then freeze the game
            Time.timeScale = 0f;
        }
        else
        {
            //If the player has selected a mode then unfreeze the game
            Time.timeScale = 1f;

            //When the player has selected a mode and the instructions are off
            if (!PCScript.bl_Instructions)
            {
                //Run the timer
                DataScript.CompletionTime();
            }
        }
    }

    void SaveData()
    {
        //When the level has ended
        if (bl_EndLevel)
        {
            //When the data hasn't been saved yet
            if (!bl_SavedData)
            {
                //Unused code for stream writer
                /*FileStream fs_Directory = new FileStream("U:\\3rd Year\\MS6400 Dissertation\\Stealth Game Prototype\\Data File.txt", FileMode.Append, FileAccess.Write, FileShare.None);
				StreamWriter sw_WriteFile = new StreamWriter(fs_Directory);

				if(bl_DynamicAI)
				{
					sw_WriteFile.WriteLine("AI Mode: Dynamic");
				}
				else
				{
					sw_WriteFile.WriteLine("AI Mode: Standard");
				}
				sw_WriteFile.WriteLine("Completion Time: " + DataScript.in_Minutes + "mins & " + DataScript.in_Seconds + "secs");
				sw_WriteFile.WriteLine("PC Damage Received: " + DataScript.in_PCDamageReceived);
				sw_WriteFile.WriteLine("PC Damage Dealt: " + DataScript.in_PCDamageDealt);
				sw_WriteFile.WriteLine("Amount of times detected: " + DataScript.in_PCDetected);
				sw_WriteFile.WriteLine("NPC Damage Received: " + DataScript.in_NPCDamageReceived);
				sw_WriteFile.WriteLine("NPC Damage Dealt: " + DataScript.in_NPCDamageDealt);
				sw_WriteFile.WriteLine("Items Collected: " + DataScript.in_ItemsCollected);
				sw_WriteFile.WriteLine("Items Used: " + DataScript.in_ItemsUsed);
				sw_WriteFile.WriteLine("Guards Killed: " + DataScript.in_GuardsKilled);
				sw_WriteFile.WriteLine("Guards Eluded: " + DataScript.in_GuardsEluded);
				sw_WriteFile.WriteLine("Lowest PC Health: " + (int) PCScript.fl_Health);
				sw_WriteFile.WriteLine("Amount of retries: " + DataScript.in_Retries);
				sw_WriteFile.WriteLine("Invisibility Used: " + DataScript.in_InvisUsed);
				sw_WriteFile.WriteLine("");
				sw_WriteFile.Close();
				bl_SavedData = true;*/
            }
            //Pause the game
            Time.timeScale = 0f;
        }
    }

}
