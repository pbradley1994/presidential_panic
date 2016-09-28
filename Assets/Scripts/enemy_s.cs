using UnityEngine;
using System.Collections;

public class enemy_s : MonoBehaviour {
    private bool facingRight = false;
    // What behaviour should the enemy follow
    private string State;
    // At what range should the enemy notice the player
    public float detection_range;
    // At what range should the enemy start shooting the player
    public float shooting_range;
    // Percent chance of dropping extra life
    public float hp_drop_chance;
    // My speed
    public float my_speed;

    //private bool shoot_done = false;
    private float last_shoot_time = 0f;

    // The player's position
    public Transform p_transform;
    // The bullet
    public GameObject pistol_bullet;
    // My rigidbody
    private Rigidbody2D rb2d;
    // My animator
    private Animator anim;

    //Extra Lives prefab
    public GameObject extra_life_box;

	public AudioClip death0;
    public AudioClip death1;
    public AudioClip death2;
    public AudioClip death3;
    public AudioClip death4;
	public AudioClip shot;

	// Use this for initialization
	void Start () {
        State = "Idle";
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}

    float calculate_distance(Vector3 pos1, Vector3 pos2) {
        return (Mathf.Abs(pos1[0] - pos2[0]) + Mathf.Abs(pos1[1] - pos2[1]));
    }
	
	// Update is called once per frame
	void Update () {
		if (p_transform) {
			//Debug.Log("Enemy_State: " + State);
			if (State == "Idle") {
				// Do nothing
				if (Time.time - last_shoot_time > 2f && calculate_distance (p_transform.position, transform.position) <= detection_range) {
					anim.SetTrigger ("move");
					State = "Find";
				}
			} else if (State == "Find") {
				// Move towards player
				moveToPlayer ();
				if (p_transform) {
					if (calculate_distance (p_transform.position, transform.position) <= shooting_range) {
						Shoot ();
					}
				} else {
					State = "Idle";
					anim.SetTrigger ("Idle");
				}
			} else if (State == "Shoot") {
				// If I've finished my shoot animation
				if (!anim.GetCurrentAnimatorStateInfo (0).IsName ("Base Layer.enemy_shoot")) {
					//Debug.Log("Shooting Done!");
					//shoot_done = false;
					anim.SetTrigger ("move");
					float distance = calculate_distance (p_transform.position, transform.position);
					if (distance <= 3 * shooting_range / 4) {
						State = "Run";
						anim.SetTrigger ("move");
					} else {
						State = "Idle";
						anim.SetTrigger ("idle");
					}
				}
			} else if (State == "Run") {
				//Move away from player
				moveAwayPlayer ();
				if (calculate_distance (p_transform.position, transform.position) > shooting_range) {
					anim.SetTrigger ("move");
					State = "Find";
				}
			}  
		}
	}

    //void DoneShooting() { shoot_done = true; }

    void moveToPlayer() {
        int dir;
        // If player is to your left
        if (p_transform.position[0] < transform.position[0]) {
            dir = -1;
        }
        else {
            dir = 1;
        }
        move(dir);
    }

    void moveAwayPlayer() {
        int dir;
        // If player is to your left
        if (p_transform.position[0] < transform.position[0]) {
            dir = 1;
        }
        else {
            dir = -1;
        }
        move(dir);
    }

    void move(int dir)
    {
        if (dir > 0 && !facingRight)
            Flip();
        else if (dir < 0 && facingRight)
            Flip();
        rb2d.position += my_speed * Vector2.right * dir * Time.deltaTime;
    }

    public void handle_hit()
    {
        //Flash sprites
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        StartCoroutine(FlashSprites(sprites, 5, 0.05f));
        //Audio
        AudioClip chosen_audio_clip;
        int random_audio = Random.Range(0, 5);
        if(random_audio == 0) {chosen_audio_clip = death0;}
        else if(random_audio == 1) {chosen_audio_clip = death1;}
        else if(random_audio == 2) {chosen_audio_clip = death2;}
        else if(random_audio == 3) {chosen_audio_clip = death3;}
        else {chosen_audio_clip = death4;}
		GetComponent<AudioSource>().PlayOneShot (chosen_audio_clip);
        //Animation
        State = "Death";
        anim.SetTrigger("death");
        if (Random.Range(0f, 100f) < hp_drop_chance) {
            Instantiate(extra_life_box, transform.position, Quaternion.identity);
        }
        //Destroy(rb2d); // No longer collide
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Destroy(gameObject, 1f);
    }

    void Shoot()
    {
        //Animation and State
        //Debug.Log("Anim Trigger Shoot");
        anim.SetTrigger("shoot");
        State = "Shoot";
        Invoke("InstantiateBullet", .5f);
        last_shoot_time = Time.time;
        //Invoke("DoneShooting", 1f); // Pretend shooting is over after 1 second
    }

    void InstantiateBullet()
    {
        if (State == "Shoot")
        {
			GetComponent<AudioSource>().PlayOneShot (shot);
            //Instantiate
            float bullet_dir;
            if (facingRight) { bullet_dir = 1f; }
            else { bullet_dir = -1f; }
            GameObject instance = (GameObject)Instantiate(pistol_bullet, transform.position + new Vector3(bullet_dir * 1.75f, 0.5f, 0.0f), Quaternion.identity);

            // Set Team
            bullet_s bullet_script;
            bullet_script = instance.GetComponent<bullet_s>();
            bullet_script.setTeam(1); // Enemy Team
            bullet_script.setFacing(bullet_dir);
            bullet_script.fire();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    IEnumerator FlashSprites(SpriteRenderer[] sprites, int numTimes, float delay)
    {
        // number of times to loop
        for (int loop = 0; loop < numTimes; loop++)
        {
            // cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                // for changing the alpha
                sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 0.5f);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);

            // cycle through all sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                // for changing the alpha
                sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 1);
            }

            // delay specified amount
            yield return new WaitForSeconds(delay);
        }
    }
}
