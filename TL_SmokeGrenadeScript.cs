using UnityEngine;

public class TL_SmokeGrenadeScript : MonoBehaviour {

	//Variables
	private float fl_Timer = 5f;
    private float Distance;
    private ParticleSystem SmokeParticles;
    

    void Start()
	{
        //Obtain the particle system
        SmokeParticles = GetComponent<ParticleSystem>();        
    }

	void Update()
	{
        //Method for checking conditions for the smoke grenade
        CheckConditions();

        //Timer for how long the smoke lasts for
        SmokeTimer();
    }

    void CheckConditions()
    {
        //Locate the player
        GameObject PC = GameObject.FindGameObjectWithTag("PC");

        //Set the current position without the Y position
        Vector3 CurrentPos = new Vector3(transform.position.x, 1f, transform.position.z);

        //Calculate the distance between the player and the grenade
        float Range = Vector3.Distance(CurrentPos, new Vector3(PC.transform.position.x, 1f, PC.transform.position.z));
        
        //If the grenade is far away from the player
        if (Range >= 4f)
        {
            //Set the parent to null
            transform.parent = null;

            //Destroy the smoke grenade
            Destroy(GameObject.FindGameObjectWithTag("SmokeGrenade"));

            //Play the particle system
            SmokeParticles.Play();
        }
    }

    void SmokeTimer()
    {
        //If the smoke particles are playing
        if (SmokeParticles.isPlaying)
        {
            //Decrease the timer based on delta time
            fl_Timer -= Time.deltaTime;

            //If the timer is less than or equal to 0
            if (fl_Timer <= 0f)
            {
                //Stop the particle system
                SmokeParticles.Stop();

                //Destroy the gameobject
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider Col)
    {
        //If the trigger hits an NPC
        if (Col.tag == "NPC")
        {
            //Set the parent to null
            transform.parent = null;

            //Destroy the smoke grenade
            Destroy(GameObject.FindGameObjectWithTag("SmokeGrenade"));

            //Play the particle system
            SmokeParticles.Play();
        }
    }

}
