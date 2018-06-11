using UnityEngine;
using UnityEngine.UI;

public class TL_PCStats : MonoBehaviour {

	//Variables
	public float fl_MaxHealth;
	public float fl_Health;
	public int in_WeaponAtk;
	public GUIStyle GUI_HealthBar;
    public float fl_InvisCooldown;
    public bool bl_Invis;
    public bool bl_Instructions;
    private bool bl_InvisCooldown;
    public GUIStyle GUI_InvisBar;
	public Material mt_PC;
	public float fl_RespawnCooldown = 3f;
    private float fl_HealthBarPercentage;
    private float fl_InvisMaxEffect;
    private float fl_InvisEffect;
    private float fl_InvisInterval;
    private float fl_InvisBarPercentage;    
    private TL_LevelMediator LM_Script;
    private TL_RecordData DataScript;
	private TL_NPC_FSM NPCScript;
	private GameObject[] go_NPCs;
    private Button StandardAIButton;
    private Button DynamicAIButton;
    private Text GameOverText;



	void Start()
	{
        //Set default values
		fl_MaxHealth = 10f;
		fl_Health = fl_MaxHealth;
		fl_InvisMaxEffect = 5f;
		fl_InvisEffect = fl_InvisMaxEffect;
		fl_InvisCooldown = 0f;
		bl_Instructions = false;
		bl_Invis = false;
		bl_InvisCooldown = false;

        //Find the level map and obtain the script
		LM_Script = GameObject.Find("Level_Map").GetComponent<TL_LevelMediator>();

        //Find the data recorder and obtain the script
		DataScript = GameObject.Find("Data_Recorder").GetComponent<TL_RecordData>();

        //Obtain the material from the renderer
		mt_PC = GetComponent<Renderer>().material;

        //Find the standard AI button in the canvas
        StandardAIButton = GameObject.Find("PC_Cam(Clone)/Canvas/ChoiceBox/StandardAIButton").GetComponent<Button>();

        //Set the function of the button from the level manager
        StandardAIButton.onClick.AddListener(delegate { LM_Script.StandardAIMode(); } );

        //Find the dynamic AI button in the canvas
        DynamicAIButton = GameObject.Find("PC_Cam(Clone)/Canvas/ChoiceBox/DynamicAIButton").GetComponent<Button>();

        //Set the function of the button from the level manager
        DynamicAIButton.onClick.AddListener(delegate { LM_Script.DynamicAIMode(); });
    }

	void Update()
    {
        //For updating the health bar
        UpdateHealthBar();

        //For updating the invisibility meter
        UpdateInvisibleMeter();

        //Checking if the player is dead or not
        CheckPlayerDeath();

        //Display the instructions
        InstructionsPage();

        //Making the PC go invisible
		Invisiblility();
	}

    void UpdateHealthBar()
    {
        //Find the health bar in the canvas
        GameObject HealthBar = GameObject.Find("PC_Cam(Clone)/Canvas/HealthBox/HealthBar");

        //Obtain the rect transform from the health bar
        RectTransform HealthBarWidth = HealthBar.GetComponent<RectTransform>();

        //Calculate the health bar percentage
        fl_HealthBarPercentage = fl_Health / fl_MaxHealth * 120f;

        //Scale the X size of the sizedelta with the percentage of the health
        HealthBarWidth.sizeDelta = new Vector2(fl_HealthBarPercentage, 25f);
    }

    void UpdateInvisibleMeter()
    {
        //Find the invisible meter in the canvas
        GameObject InvisibleMeter = GameObject.Find("PC_Cam(Clone)/Canvas/InvisibilityBox/InvisibleMeter");

        //Obtain the rect transform from the invisible meter
        RectTransform InvisibleMeterWidth = InvisibleMeter.GetComponent<RectTransform>();

        //Calculate the invisible meter
        fl_InvisBarPercentage = fl_InvisEffect / fl_InvisMaxEffect * 123f;

        //Scale the X size of the sizedelta with the percentage of the invisible meter
        InvisibleMeterWidth.sizeDelta = new Vector2(fl_InvisBarPercentage, 25f);

        //Find the gameobject and obtain the text component
        Text InvisibleCooldown = GameObject.Find("PC_Cam(Clone)/Canvas/InvisibilityBox/CooldownText").GetComponent<Text>();

        //Display the current cooldown of the invisibility
        InvisibleCooldown.text = "Cooldown: " + fl_InvisCooldown.ToString("F0");
    }

    void CheckPlayerDeath()
    {
        //If the PC has less than or equal to 0 health
        if (fl_Health <= 0)
        {
            //Find the game over box in the canvas
            GameObject GameOverBox = GameObject.Find("PC_Cam(Clone)/Canvas/GameOverBox");

            //Activate the game over box
            GameOverBox.SetActive(true);

            //Find the text in the canvas
            GameOverText = GameOverBox.GetComponentInChildren<Text>();
            
            //Set PC health to 0
            fl_Health = 0f;

            //Find all of the NPC's
            go_NPCs = GameObject.FindGameObjectsWithTag("NPC");

            //Disable all of the NPC's scripts
            foreach (GameObject go in go_NPCs)
            {
                NPCScript = go.GetComponent<TL_NPC_FSM>();
                NPCScript.enabled = false;
            }

            //Disable the movement script on the PC
            GetComponent<TL_PCMove>().enabled = false;

            //If the respawn cooldown is less than or equal to 0
            if (fl_RespawnCooldown <= 0f)
            {
                //Set the respawn cooldown to 0
                fl_RespawnCooldown = 0f;

                //Maintain the data and restart the scene
                DataScript.MaintainData();
            }
            else
            {
                //Subtract the respawn cooldown with the deltatime
                fl_RespawnCooldown -= Time.deltaTime;
            }

            //Display the text and the respawn countdown
            GameOverText.text = "You died \nRespawning in: " + Mathf.Floor(fl_RespawnCooldown).ToString("F0");
        }
    }

	private void InstructionsPage()
	{
        //When the level hasn't ended yet, the player has selected a mode and is still alive
		if(!LM_Script.bl_EndLevel && LM_Script.bl_Selected && fl_Health > 0)
		{
            //Find the instructions page within the canvas
            GameObject InstructionsPage = GameObject.Find("PC_Cam(Clone)/Canvas/Instructions");

            //When the P key is pressed
			if(Input.GetKeyDown(KeyCode.P))
			{
                //Toggle the boolean
				bl_Instructions = !bl_Instructions;
            }

            //Set the instructions page active or inactive depending on the boolean
            InstructionsPage.SetActive(bl_Instructions);

            if (bl_Instructions)
			{
                //If the instructions page is on then freeze the game
                Time.timeScale = 0f;
                
            }
			else
			{
                //If the instructions page is off then unfreeze the game
				Time.timeScale = 1f;
			}
		}
	}

	private void Invisiblility()
	{
        //If the instructions are off
        if (!bl_Instructions)
        {
            //If the player press Q and the cooldown is 0
            if (Input.GetKeyDown(KeyCode.Q) && fl_InvisCooldown == 0f)
            {
                //Set bool to true
                bl_Invis = true;

                //Record usage of invisibility used
                DataScript.in_InvisUsed++;

                //Change the alpha channel of the material
                GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a / 2f);

                //Set the cooldown to 12
                fl_InvisCooldown = 12f;
            }

            //If the player is invisible
            if (bl_Invis)
            {
                //If the invisible interval is less than time since start up
                if (fl_InvisInterval < Time.realtimeSinceStartup)
                {
                    //Subtract the value of the invisble effect
                    fl_InvisEffect -= 1f;

                    //Add the interval with real time since start up
                    fl_InvisInterval = 1f + Time.realtimeSinceStartup;
                }

                //If the invisible effect reaches less than 0
                if (fl_InvisEffect < 0)
                {
                    //Change the alpha channel of the material back to original
                    GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a * 2f);

                    //Set the value back to 0
                    fl_InvisEffect = 0;

                    //Set the cooldown of the invisibility to true
                    bl_InvisCooldown = true;

                    //Set invisibility to false
                    bl_Invis = false;
                }
            }
            else
            {
                //If the invisibility interval is less than real time since start up
                if (fl_InvisInterval < Time.realtimeSinceStartup)
                {
                    //Increment the value to slowly fill up the invisibility bar
                    fl_InvisEffect += 1f;

                    //Add the interval with real time since start up
                    fl_InvisInterval = 1f + Time.realtimeSinceStartup;
                }

                //If the current invisible effect is more than or equal to its' maximum
                if (fl_InvisEffect >= fl_InvisMaxEffect)
                {
                    //Set the current effect to its' max effect
                    fl_InvisEffect = fl_InvisMaxEffect;
                }
            }

            //If the cooldown is on
            if (bl_InvisCooldown)
            {
                //Reduce the cooldown value with delta time
                fl_InvisCooldown -= Time.deltaTime;

                //If the cooldown is less than 0
                if (fl_InvisCooldown < 0f)
                {
                    //Set the cooldown to 0
                    fl_InvisCooldown = 0f;

                    //Set the cooldown to false
                    bl_InvisCooldown = false;
                }
            }
        }
	}

	public void ReceiveDamage(float fl_damage)
	{
        //Subtract the current health with damage
		fl_Health -= fl_damage;

        //Record the damage received from the NPC
		DataScript.in_PCDamageReceived += (int)fl_damage;
	}

}