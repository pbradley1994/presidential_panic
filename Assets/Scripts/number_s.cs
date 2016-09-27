using UnityEngine;
using System.Collections;

public class number_s : MonoBehaviour {
    public player_s player_script;
    private Sprite[] num_sprites;

	// Use this for initialization
	void Awake () {
        num_sprites = Resources.LoadAll<Sprite>("Number");
	}
	
	// Update is called once per frame
	void Update () {
        int clamped_lives = Mathf.Min(9, Mathf.Max(0, player_script.extra_lives));
        GetComponent<SpriteRenderer>().sprite = num_sprites[clamped_lives];
	}
}
