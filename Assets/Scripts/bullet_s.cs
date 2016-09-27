using UnityEngine;
using System.Collections;

public class bullet_s : MonoBehaviour {
    public int team;
    public float speed;

    private bool lethal = true;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 0.75f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Bullet: I collided with something!");
        if (lethal && col.gameObject.tag == "Player" && team == 1)
        {
            Debug.Log("Bullet: A player");
            player_s player_script;
            player_script = col.gameObject.GetComponent<player_s>();
            player_script.handle_hit();
            lethal = false;
            Destroy(gameObject, 0.05f);
        }
        else if (lethal && col.gameObject.tag == "Enemy" && team == 0)
        {
            Debug.Log("Bullet: An enemy!");
            enemy_s enemy_script;
            enemy_script = col.gameObject.GetComponent<enemy_s>();
            enemy_script.handle_hit();
            lethal = false;
            Destroy(gameObject, 0.05f);
        }
        else if (col.gameObject.tag == "Ground")
        {
            Debug.Log("Bullet: The World");
            Destroy(gameObject, 0.0f);
        }
    }

    public void setTeam(int s_team) { team = s_team; }
    public void setFacing(float dir) { speed = dir * speed; if (dir < 0) Flip(); }
    public void fire() { gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(speed, 0f, 0f); }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
