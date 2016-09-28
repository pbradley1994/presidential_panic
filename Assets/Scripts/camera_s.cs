using UnityEngine;
using System.Collections;

public class camera_s : MonoBehaviour {
    public Transform player;

	void Start () {
        float targetaspect = 3.0f / 2.0f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;

        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
	}
	
	void Update () {
		if (player) {
			float new_x;
			new_x = player.position [0] + 4.5f;
			new_x = Mathf.Max (-1f, Mathf.Min (4400f / 16f - 4.5f, new_x));
			transform.position = new Vector3 (new_x, transform.position [1], transform.position [2]);
		}
	}
}
