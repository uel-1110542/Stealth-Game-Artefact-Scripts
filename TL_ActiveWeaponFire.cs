using UnityEngine;
using UnityEngine.AI;

public class TL_ActiveWeaponFire : MonoBehaviour {

	//Variables
	public GameObject go_Bullet;
	public GameObject go_SmokeGrenade;
	private GameObject go_Projectile;
	private GameObject go_ProjectileClone;
	private float fl_RateOfFire = 0.5f;
	private float fl_FiringCooldown;
	private TL_Inventory InventoryScript;
	private TL_RecordData DataScript;
	private TL_PCStats PCScript;



	void Start()
	{
        //Obtain the scripts from the gameobject
		InventoryScript = GetComponent<TL_Inventory>();
		PCScript = GetComponent<TL_PCStats>();

        //Find the data recorder gameobject and obtain the script
        DataScript = GameObject.Find("Data_Recorder").GetComponent<TL_RecordData>();
	}

	void Update()
	{
        //If the player is not invisible, the player can use the active weapon
		if(!PCScript.bl_Invis)
		{
			UseActiveWeapon();
		}
	}

	void UseActiveWeapon()
	{
        //If the active weapon exists
		if(InventoryScript.go_ActiveWeapon != null)
		{
            //Change the projectile depending on the active weapon
			switch(InventoryScript.go_ActiveWeapon.name)
			{
			case "pf_Pistol":
				go_Projectile = go_Bullet;
				break;
				
			case "pf_SmokeGrenade":
				go_Projectile = go_SmokeGrenade;
				break;
			}

            //If the projectile exists in the scene
			if(go_Projectile != null)
			{
                //If the firing cooldown is less than the time since start up
				if(fl_FiringCooldown < Time.realtimeSinceStartup)
				{
                    //If the ammo count is not 0 or if the projectile is a smoke grenade
					if(InventoryScript.in_AmmoCount > 0 || go_Projectile == go_SmokeGrenade)
					{
                        //If the player presses one of the directional keys
						if(Input.GetKey(KeyCode.LeftArrow))
						{
                            //Instantiate the projectile with an offset dependant to the position
							go_ProjectileClone = Instantiate (go_Projectile, new Vector3(transform.position.x - 0.01f, transform.position.y, transform.position.z - 1f), Quaternion.identity);

                            //Add force to the projectile depending on the direction
                            go_ProjectileClone.GetComponent<Rigidbody>().AddForce(Vector3.back * 500);

                            //Add the cooldown
                            fl_FiringCooldown = fl_RateOfFire + Time.realtimeSinceStartup;
						}
						else if(Input.GetKey(KeyCode.RightArrow))
						{
                            //Instantiate the projectile with an offset dependant to the position
                            go_ProjectileClone = Instantiate (go_Projectile, new Vector3(transform.position.x - 0.01f, transform.position.y, transform.position.z + 1f), Quaternion.identity);

                            //Add force to the projectile depending on the direction
                            go_ProjectileClone.GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);

                            //Add the cooldown
                            fl_FiringCooldown = fl_RateOfFire + Time.realtimeSinceStartup;
						}
						else if(Input.GetKey(KeyCode.UpArrow))
						{
                            //Instantiate the projectile with an offset dependant to the position
                            go_ProjectileClone = Instantiate (go_Projectile, new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z), Quaternion.identity);

                            //Add force to the projectile depending on the direction
                            go_ProjectileClone.GetComponent<Rigidbody>().AddForce(Vector3.right * -500);

                            //Add the cooldown
                            fl_FiringCooldown = fl_RateOfFire + Time.realtimeSinceStartup;
						}
						else if(Input.GetKey(KeyCode.DownArrow))
						{
                            //Instantiate the projectile with an offset dependant to the position
                            go_ProjectileClone = Instantiate (go_Projectile, new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z), Quaternion.identity);

                            //Add force to the projectile depending on the direction
                            go_ProjectileClone.GetComponent<Rigidbody>().AddForce(Vector3.left * -500);

                            //Add the cooldown
                            fl_FiringCooldown = fl_RateOfFire + Time.realtimeSinceStartup;
						}
					}

                    //If the player presses one of the directional keys
                    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
					{
                        //If the projectile is a smoke grenade
						if(go_Projectile == go_SmokeGrenade)
						{
                            //Increment the items used variable
                            DataScript.in_ItemsUsed++;

                            //Reset the active weapon slot to empty
                            InventoryScript.go_ActiveWeapon = null;
                            InventoryScript.sp_ActiveWeaponIcon = null;
                            InventoryScript.st_ActiveWeaponTooltip = "";
							InventoryScript.ActiveWeaponButton.image.sprite = InventoryScript.sp_InventoryBox;
                        }
						else if(go_Projectile == go_Bullet)
						{
                            //If the projectile is a bullet, decrease the ammo count
							InventoryScript.in_AmmoCount--;

                            //If the ammo count is less than or equal to 0
							if(InventoryScript.in_AmmoCount <= 0)
							{
                                //Set the ammo count to 0
								InventoryScript.in_AmmoCount = 0;
							}
						}
					}
				}
			}
		}

	}

}
