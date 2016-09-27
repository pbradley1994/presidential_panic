using UnityEngine;
using System.Collections;

public class camera_s : MonoBehaviour {
    public Transform player;
    private Transform my_transform;

	// Use this for initialization
	void Start () {
        my_transform = gameObject.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player) {
			float new_x;
			new_x = player.position [0] + 4.5f;
			new_x = Mathf.Max (0, Mathf.Min (4400f / 16f - 4.5f, new_x));
			my_transform.position = new Vector3 (new_x, my_transform.position [1], my_transform.position [2]);
		}
	}
}
