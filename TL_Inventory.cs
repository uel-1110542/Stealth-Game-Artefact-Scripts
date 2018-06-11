using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TL_Inventory : MonoBehaviour {

	//Variables	
	public GameObject go_ActiveWeapon;
	public Sprite sp_ActiveWeaponIcon;
	public string st_ActiveWeaponTooltip;
	private float fl_ScaleHeight;
	private float fl_ScaleWidth;	
	private Vector3 v3_ScreenToWorldSpace;

	//Variables for Items
	private string st_DisplayTooltip;
	private TL_ActiveWeaponFire FiringScript;
	private GameObject go_AddedItem;
	private TL_PCStats PCScript;
	private TL_RecordData DataScript;

    //Variables for inventory    
    public int in_AmmoCount = 0;
    public Sprite sp_InventoryBox;
    public GameObject[] go_Inventory = new GameObject[8];
    public string[] st_ItemTooltip = new string[8];
    public string StoredTooltip;
    public Button ActiveWeaponButton;
    public GameObject AmmoCount;
    private Button[] InventoryButtons = new Button[8];
    private GameObject InventoryTooltip;
    private int PrevItemIndex;



    void Start()
	{
        //Obtain the script from the PC
		PCScript = GetComponent<TL_PCStats>();

        //Find the data recorder and obtain the script
		DataScript = GameObject.Find("Data_Recorder").GetComponent<TL_RecordData>();

        //Find the ammo counter gameobject
        AmmoCount = GameObject.Find("PC_Cam(Clone)/Canvas/AmmoCount");

        //Find the inventory tooltip
        InventoryTooltip = GameObject.Find("PC_Cam(Clone)/Canvas/InventoryBox/InventoryTooltip");

        //Initialize the buttons in an array
        InitializeInventory();
    }

    void InitializeInventory()
    {
        //Loop through the inventory buttons
        for (int i = 0; i < 7; i++)
        {
            //Find all of the buttons in the canvas and set it to the arrays
            InventoryButtons[i] = GameObject.Find("PC_Cam(Clone)/Canvas/InventoryBox/Button" + i.ToString()).GetComponent<Button>();
        }
        //Find the equip box and obtain the button
        ActiveWeaponButton = GameObject.Find("PC_Cam(Clone)/Canvas/ActiveWeaponBox/EquipBox").GetComponent<Button>();
    }

    void Update()
    {
        //Keeps track of the ammo from the pistol
        AmmoCounter();
    }

    void AmmoCounter()
    {
        //If the active weapon is the pistol equipped
        if (go_ActiveWeapon != null && go_ActiveWeapon.name == "pf_Pistol")
        {
            //Set the gameobject showing the ammo counter to be active
            AmmoCount.SetActive(true);

            //Obtain the text component from the child gameobject
            Text AmmoText = AmmoCount.GetComponentInChildren<Text>();

            //Display the ammo count
            AmmoText.text = "Ammo: " + in_AmmoCount;
        }
        else
        {
            //Set the gameobject showing the ammo counter to be inactive
            AmmoCount.SetActive(false);
        }
    }

    void RevealTooltip(int Index, string Tooltip)
    {
        //If the inventory slot is not empty
        if (go_Inventory[Index] != null)
        {
            //Reveal the tooltip
            InventoryTooltip.SetActive(true);

            //Obtain the text component from the child gameobject
            Text ItemTooltip = InventoryTooltip.GetComponentInChildren<Text>();

            //Set the item tooltip
            ItemTooltip.text = Tooltip;
        }
    }

    void UpdateTooltip(int Index)
    {
        //If the inventory slot is empty
        if (go_Inventory[Index] == null)
        {
            //Hide the tooltip
            InventoryTooltip.SetActive(false);
        }
    }

    void HideTooltip()
    {
        //Hide the tooltip
        InventoryTooltip.SetActive(false);
    }

    void UpdateInventory(GameObject item, int index, string tooltip, Sprite itemicon)
    {        
        //If the item slot based on the element is empty
        if (go_Inventory[index] == null)
        {
            //Add the gameobject into the array based on the element
            go_Inventory[index] = item;

            //Set the item tooltip
            st_ItemTooltip[index] = tooltip;

            //Set the event trigger variable to obtain the event trigger component
            EventTrigger TriggerButton = InventoryButtons[index].GetComponent<EventTrigger>();

            //Create new entries for the entry and exit button
            EventTrigger.Entry MouseHover = new EventTrigger.Entry();
            EventTrigger.Entry MouseClick = new EventTrigger.Entry();
            EventTrigger.Entry MouseExit = new EventTrigger.Entry();

            //Set event ID for the pointer enter event trigger type
            MouseHover.eventID = EventTriggerType.PointerEnter;

            //Add the listener for revealing the tooltip
            MouseHover.callback.AddListener(delegate { RevealTooltip(index, st_ItemTooltip[index]); });

            //Add the trigger for the mouse hover
            TriggerButton.triggers.Add(MouseHover);

            //Set event ID for the pointer click event trigger type
            MouseClick.eventID = EventTriggerType.PointerClick;

            //Add the listener for updating the tooltip
            MouseClick.callback.AddListener(delegate { UpdateTooltip(index); });

            //Add the trigger for the mouse click
            TriggerButton.triggers.Add(MouseClick);

            //Set event ID for the pointer exit event trigger type
            MouseExit.eventID = EventTriggerType.PointerExit;

            //Add the listener for hiding the tooltip
            MouseExit.callback.AddListener(delegate { HideTooltip(); });

            //Add the trigger for the mouse exit
            TriggerButton.triggers.Add(MouseExit);

            //Update the sprite with the item icon
            InventoryButtons[index].image.sprite = itemicon;

            //Add the listener
            InventoryButtons[index].onClick.AddListener(delegate { UseItem(index); });
        }
        else
        {
            //If the item slot is not empty then make it empty
            go_Inventory[index] = null;

            //Set the item tooltip to blank
            st_ItemTooltip[index] = "";

            //Updates the sprite
            InventoryButtons[index].image.sprite = sp_InventoryBox;

            //Remove the listener
            InventoryButtons[index].onClick.RemoveListener(delegate { UseItem(index); } );
        }
    }

	public void AddItem(GameObject go_Item, string st_Tooltip, Sprite sp_Icon)
	{
        //Loop through the inventory array with a for loop
		for(int i = 0; i < go_Inventory.Length; i++)
		{
            //Checks if that item slot is empty and if it is
			if(go_Inventory[i] == null)
			{
                //Update the inventory
                UpdateInventory(go_Item, i, st_Tooltip, sp_Icon);

                //Store the previous item index
                PrevItemIndex = i;

                //Add the gameobject into the array
                go_Inventory[i] = go_Item;
                break;
			}
			else if(go_Inventory[0] != null && go_Inventory[1] != null && go_Inventory[2] != null && go_Inventory[3] != null && go_Inventory[4] != null && go_Inventory[5] != null && go_Inventory[6] != null && go_Inventory[7] != null)
			{
                //If all of the inventory slots are taken then break out of the loop
				break;
			}
		}
	}

	public void UseItem(int itemindex)
	{
        //If the item slot based on the index is not null
		if(go_Inventory[itemindex] != null)
		{
            //Slecting a case statement for the inventory names depending on the inventory slot
            switch (go_Inventory[itemindex].name)
			{
                case "pf_HealthPill":
                    //If the player's health is not equal to its' maximum health
				    if(PCScript.fl_Health != PCScript.fl_MaxHealth)
				    {
                        //Add 3 health
					    PCScript.fl_Health += 3f;

                        //If the player's health overflows its' maximum health
					    if(PCScript.fl_Health > PCScript.fl_MaxHealth)
					    {
                            //Make the current health equal to its' maximum health
						    PCScript.fl_Health = PCScript.fl_MaxHealth;
					    }
                        //Increment the value of items used
					    DataScript.in_ItemsUsed++;

                        //Update the inventory with an empty slot
                        UpdateInventory(null, itemindex, null, null);
				    }
				    break;
				
			    case "pf_Ammo":
                    //If the active weapon is still equipped
				    if(go_ActiveWeapon != null)
				    {
                        //If the active weapon is a pistol
					    if(go_ActiveWeapon.name == "pf_Pistol")
					    {
                            //Increase the ammo count
						    in_AmmoCount += 8;

                            //Increment the value of items used
                            DataScript.in_ItemsUsed++;
					    }
                        //Update the inventory with an empty slot
                        UpdateInventory(null, itemindex, null, null);
				    }
				    break;

			    case "pf_SmokeGrenade":
                    //Equip the weapon
                    EquipWeapon(itemindex);
				    break;

			    case "pf_Pistol":
                    //Equip the weapon
                    EquipWeapon(itemindex);
				    break;
			}
		}
	}

	public void EquipWeapon(int itemindex)
	{
        //If there isn't any active weapon
		if(go_ActiveWeapon == null)
		{
            //If the gameobject in the inventory is equipable
			if(go_Inventory[itemindex].tag == "Equipable")
			{
                //Set the active weapon from the gameobject inventory based on the item index
                go_ActiveWeapon = go_Inventory[itemindex];

                //Enable the active weapon fire script
                gameObject.GetComponent<TL_ActiveWeaponFire>().enabled = true;

                //Store the item tooltip for the active weapon
                StoredTooltip = st_ItemTooltip[itemindex];

                //Set the active weapon icon from the sprite of the inventory button
                sp_ActiveWeaponIcon = InventoryButtons[itemindex].image.sprite;

                //Set the sprite of the active weapon button from the sprite of the inventory button
                ActiveWeaponButton.image.sprite = InventoryButtons[itemindex].image.sprite;
                
                //Add the listener to the active weapon button
                ActiveWeaponButton.onClick.AddListener(delegate { UnequipActiveWeapon(); });

                //Update the inventory with an empty slot
                UpdateInventory(null, itemindex, null, null);
            }
		}
	}

	void UnequipActiveWeapon()
	{
        //If there isn't any active weapon equipped
		if(go_ActiveWeapon == null)
		{
            //Disable the weapon firing script
			gameObject.GetComponent<TL_ActiveWeaponFire>().enabled = false;

            //Set the active weapon icon to null
            sp_ActiveWeaponIcon = null;
        }
		else
		{
            //Set the active weapon tooltip as the stored tooltip
            st_ActiveWeaponTooltip = StoredTooltip;

            //If there is an active weapon equipped, add the item back into the inventory
            AddItem(go_ActiveWeapon, st_ActiveWeaponTooltip, sp_ActiveWeaponIcon);

            //Set the active weapon to null
            go_ActiveWeapon = null;

            //Set the active weapon icon to null
            sp_ActiveWeaponIcon = null;

            //Revert the button to a blank box
            ActiveWeaponButton.image.sprite = sp_InventoryBox;

            //Remove the listener
            ActiveWeaponButton.onClick.RemoveListener(delegate { UnequipActiveWeapon(); });            
        }

	}

	void OnTriggerEnter(Collider col_obj)
	{
		switch(col_obj.tag)
		{
            case "HealthPickup":
                //Obtain the script from the collided gameobject
                TL_PickupItem PillScript = col_obj.gameObject.GetComponent<TL_PickupItem>();

                //Add the item with its' tooltip and icon
                AddItem(PillScript.go_Item, PillScript.st_Tooltip, PillScript.sp_Icon);

                //Increase the value of items collected
                DataScript.in_ItemsCollected++;

                //Destroy the collided gameobject
                Destroy(col_obj.gameObject);
                break;

		    case "PistolPickup":
                //Obtain the script from the collided gameobject
                TL_PickupItem PistolScript = col_obj.gameObject.GetComponent<TL_PickupItem>();

                //Add the item with its' tooltip and icon
                AddItem(PistolScript.go_Item, PistolScript.st_Tooltip, PistolScript.sp_Icon);

                //Equip the weapon if the player is not equipping anything
                EquipWeapon(PrevItemIndex);

                //Add ammo
                in_AmmoCount += 8;

                //Increase the value of items collected
                DataScript.in_ItemsCollected++;

                //Destroy the collided gameobject
                Destroy(col_obj.gameObject);
                break;

		    case "SmokeGrenadePickup":
                //Obtain the script from the collided gameobject
                TL_PickupItem SmokeGrenadeScript = col_obj.gameObject.GetComponent<TL_PickupItem>();

                //Add the item with its' tooltip and icon
                AddItem(SmokeGrenadeScript.go_Item, SmokeGrenadeScript.st_Tooltip, SmokeGrenadeScript.sp_Icon);

                //Equip the weapon if the player is not equipping anything
                EquipWeapon(PrevItemIndex);

                //Increase the value of items collected
                DataScript.in_ItemsCollected++;

                //Destroy the collided gameobject
                Destroy(col_obj.gameObject);
                break;

		    case "AmmoPickup":
                //Obtain the script from the collided gameobject
                TL_PickupItem AmmoScript = col_obj.gameObject.GetComponent<TL_PickupItem>();

                //Add the item with its' tooltip and icon
                AddItem(AmmoScript.go_Item, AmmoScript.st_Tooltip, AmmoScript.sp_Icon);

                //Increase the value of items collected
                DataScript.in_ItemsCollected++;

                //Destroy the collided gameobject
                Destroy (col_obj.gameObject);
                break;
		}

	}

}
