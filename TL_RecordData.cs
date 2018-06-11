using UnityEngine;
using UnityEngine.SceneManagement;

public class TL_RecordData : MonoBehaviour {

	//Variables for the scoreboard
	public int in_Minutes = 0;
	public int in_Seconds = 0;
	public int in_SecondInterval = 1;
	public int in_SecondCooldown = 1;
	public int in_PCDamageReceived = 0;
	public int in_PCDamageDealt = 0;
	public int in_NPCDamageReceived = 0;
	public int in_NPCDamageDealt = 0;
	public int in_ItemsCollected = 0;
	public int in_ItemsUsed = 0;
	public int in_PCDetected = 0;
	public int in_GuardsKilled = 0;
	public int in_GuardsEluded = 0;
	public int in_Retries = 0;
	public int in_InvisUsed = 0;
    public float fl_Timer;

    //Private variables
	private TL_LevelMediator LM_Script;
	private TL_NPC_FSM NPCScript;
	private GameObject[] go_NPCs;



	void Start()
	{
        //Find the level map and obtain the level mediator
		LM_Script = GameObject.Find ("Level_Map").GetComponent<TL_LevelMediator>();
	}

	public void CompletionTime()
	{
        //Add the timer with the deltatime to store it
        fl_Timer += Time.deltaTime;

        //Calculate the seconds with the timer divided by 60 with the modulus to return a reminder
        //and returns the largest integer
        in_Seconds = (int) Mathf.Floor(fl_Timer % 60f);

        //Calculate the minutes with the timer divided by 60 and returns the largest integer
        in_Minutes = (int) Mathf.Floor(fl_Timer / 60f);
	}

	public void MaintainData()
	{
        //Increment the retries by 1
		in_Retries++;

        //Reset the minutes and the seconds
		in_Minutes = 0;
		in_Seconds = 0;

        //Turn off the bool to restart selection again
		LM_Script.bl_Selected = false;

        //Don't destroy this gameobject on load to save data
		DontDestroyOnLoad (transform.gameObject);

        //Find all of the NPCs and enable the scripts
		go_NPCs = GameObject.FindGameObjectsWithTag("NPC");
		foreach(GameObject go in go_NPCs)
		{
			NPCScript = go.GetComponent<TL_NPC_FSM>();
			NPCScript.enabled = true;
		}
        //Reload the scene again
		SceneManager.LoadScene("Level_Area");
	}

}
