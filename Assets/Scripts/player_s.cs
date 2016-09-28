using UnityEngine;
using System.Collections;

public class player_s : MonoBehaviour {
    private bool facingRight = true;
    private bool jump = false;
    private bool take_input = true;

    public float my_speed = 5f;
    public float jumpForce = 100f;

    private bool grounded = false;
    public Animator leg_anim;
    public SpriteRenderer leg_sprite;
    public Animator top_anim;
    private Rigidbody2D rb2d;

    public GameObject pistol_bullet;

    public int extra_lives = 2;
    private bool shotgun = false;

    private float last_shotgun_time = 0;
    public float total_shotgun_ammo;
    private float shotgun_ammo;

    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    public GameObject rescue_message;

	public AudioClip hit;
	public AudioClip pickup;
	public AudioClip jump_grunt;
	public AudioClip scream;
	public AudioClip shot;
	public AudioClip laugh;

	private float fell;


    // Use this for initialization
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
        rescue_message.SetActive(false);
		fell = 0;
    }
	
	// Update is called once per frame
    void Update()
    {
		//Out of Map
		if(transform.position[1] < -5 && fell == 0){
			fell = 1;
			GetComponent<AudioSource>().PlayOneShot (scream);
			Invoke("FadeToGameOver", 1.5f);
			}
        //Jump Handling
        if (take_input && Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
            grounded = false;
            leg_anim.SetTrigger("jump");
			GetComponent<AudioSource>().PlayOneShot (jump_grunt);
        }
        //Shoot Handlings
        if (take_input && Input.GetButton("Fire1")) {
            if (shotgun) {
                //Animation
                top_anim.SetTrigger("shotgun_shoot");
                if (Time.time - last_shotgun_time > 0.25f)
                {
                    //Instantiate
                    Build_Bullet(0.1f);
                    last_shotgun_time = Time.time;
                    CheckShotgunAmmo();
                }
            }
            else {
                if(!top_anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.shoot_pistol")) {
                    //Animation
                    top_anim.SetTrigger("pistol_shoot");
                    //Instantiate
                    Build_Bullet(0.5f);
                }
            }
        }
        if (!Input.GetButton("Fire1") && shotgun)
        {
            Debug.Log("Done Firing Shotgun");
            //Animation
            top_anim.SetTrigger("done_shotgun_shoot");
        }
    }

    void CheckShotgunAmmo() {
        shotgun_ammo -= 1;
        if (shotgun_ammo <= 0) {
            top_anim.SetTrigger("clip_empty");
            shotgun = false;
        }
    }

    void Build_Bullet(float height)
    {
		if (extra_lives > -1) {
			//Instantiate
			float bullet_dir;
			if (facingRight) {
				bullet_dir = 1f;
			} else {
				bullet_dir = -1f;
			}
			GameObject instance = (GameObject)Instantiate (pistol_bullet, transform.position + new Vector3 (bullet_dir * 1.75f, height, 0.0f), Quaternion.identity);
			GetComponent<AudioSource> ().PlayOneShot (shot);
			// Set Team
			bullet_s bullet_script;
			bullet_script = instance.GetComponent<bullet_s> ();
			bullet_script.setTeam (0);
			bullet_script.setFacing (bullet_dir);
			bullet_script.fire ();
		}
    }

    void FixedUpdate()
    {
        if (take_input)
        {
            float h = Input.GetAxis("Horizontal");
            //Actually move
            float push;
            if (h > 0)
                push = 1;
            else if (h < 0)
                push = -1;
            else
                push = 0;
            leg_anim.SetFloat("speed", Mathf.Abs(push));
            rb2d.position += my_speed * Vector2.right * push * Time.deltaTime;

            if (h > 0 && !facingRight)
                Flip();
            else if (h < 0 && facingRight)
                Flip();
        }
        if (jump)
        {
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
        //leg_anim.SetFloat("vspeed", GetComponent<Rigidbody2D>().velocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Ground")
            grounded = true;
            if (leg_anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.jump_legs") || leg_anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.fall_legs"))
            {
                Debug.Log("End Jump");
                leg_anim.Play("Base Layer.idle_legs");
            }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Win")
        {
            //Display rescue message
            rescue_message.SetActive(true);
            //Destroy all enemies!
            GameObject[] enemies;
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy_s enemy_script;
                enemy_script = enemy.GetComponent<enemy_s>();
                enemy_script.handle_hit();
            }
            // President is Free
            //gameObject.GetComponent<Animator>().SetTrigger("free");
            // Fade to black
            Invoke("WinGame", 3f);
        }
        else if (coll.gameObject.tag == "Crate")
        {
            Destroy(coll.gameObject);
            GiveShotgun();
			GetComponent<AudioSource>().PlayOneShot (pickup);
        }
        else if (coll.gameObject.tag == "HealthBox" && extra_lives < 9)
        {
            Destroy(coll.gameObject);
            extra_lives += 1;
			GetComponent<AudioSource>().PlayOneShot (pickup);
        }
    }

    public void handle_hit() {
		extra_lives -= 1;
        //Flash sprites
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        StartCoroutine(FlashSprites(sprites, 5, 0.05f));

		if (extra_lives > -1) {
			GetComponent<AudioSource> ().PlayOneShot (hit);
		}
        if(extra_lives == -1) {
			GetComponent<AudioSource>().PlayOneShot (scream);
            //Destroy(gameObject, 0.0f);
            // Death Animation
            take_input = false;
            leg_sprite.enabled = false;
            leg_anim.enabled = false;
            top_anim.SetTrigger("death");
            // Fade to Black after Animation, Fade to Game Over Screen
            Invoke("FadeToGameOver", 1.5f);
        }
    }

    void GiveShotgun() {
        shotgun = true;
        shotgun_ammo = total_shotgun_ammo;
        top_anim.SetTrigger("shotgun_get");
    }


    void FadeToGameOver() {
        gameOverScreen.SetActive(true);
		GetComponent<AudioSource>().PlayOneShot (laugh);

        Destroy(gameObject, 2.0f); // Remove game object
    }

    void WinGame() {
        victoryScreen.SetActive(true);
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
