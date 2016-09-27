using UnityEngine;
using System.Collections;

public class menu_s : MonoBehaviour {
    private GameObject[] pauseObjects;
    public GameObject background;
    public GameObject creditScreen;
    public GameObject tutorialScreen;
    public GameObject resume1;
    public GameObject resume0;
    public GameObject restart1;
    public GameObject restart0;
    public GameObject tutorial1;
    public GameObject tutorial0;
    public GameObject credits1;
    public GameObject credits0;

    private int state;

    //0: game
    //1: menu & resume selected
    //2: restart selected
    //3: tutorial selected
    //4: credits selected
    //5: tutorial screen
    //6: credits screen

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        state = 0;
        hidePaused();
        hideCredits();
        hideTutorial();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonUp("Cancel")) {
			if(Time.timeScale != 0) {
				Time.timeScale = 0;
				state = 1;
                showPaused();
			} 
			
			else {
				Debug.Log("PauseScreen");
				Time.timeScale = 1;
				state = 0;
                hidePaused();
			}
		}
		//change for actual key
		if(Input.GetKeyUp("s") && state > 0 && state < 4) {
            Debug.Log("Menu Down");
			state++;
			updateMenu();
		}
		
        //change for actual key
        if (Input.GetKeyUp("w") && state > 1)
        {
            Debug.Log("Menu Up");
			state--;
			updateMenu();
		}
		
		if(Input.GetButtonUp("Jump")) {
			if(state < 5 && state > 0) {
				Time.timeScale = 1;
				state = 0;
                hidePaused();
			}
			
			if(state == 5) {
                state = 1;	
				showPaused();
				hideTutorial();		
			}
			
			if(state == 6) {
                state = 1;	
				showPaused();
				hideCredits();		
			}	
		}
		
		if(Input.GetButtonUp("Fire1")) {
			if(state == 1) {
                state = 0;
				Time.timeScale = 1;
				hidePaused();
			}
			
			if(state == 2) {
				Restart();
			}
			
			if(state == 3) {
                state = 5;
                hidePaused();
				showTutorial();
			}
			
			if(state == 4) {
                state = 6;
                hidePaused();
				showCredits();			
			}
		}
	}

    public void updateMenu()
    {
        Debug.Log("Update Menu");
        if (state == 1)
        {
            resume1.SetActive(true);
            resume0.SetActive(false);

            restart1.SetActive(false);
            restart0.SetActive(true);

            tutorial1.SetActive(false);
            tutorial0.SetActive(true);

            credits1.SetActive(false);
            credits0.SetActive(true);
        }

        if (state == 2)
        {
            resume1.SetActive(false);
            resume0.SetActive(true);

            restart1.SetActive(true);
            restart0.SetActive(false);

            tutorial1.SetActive(false);
            tutorial0.SetActive(true);

            credits1.SetActive(false);
            credits0.SetActive(true);
        }

        if (state == 3)
        {
            resume1.SetActive(false);
            resume0.SetActive(true);

            restart1.SetActive(false);
            restart0.SetActive(true);

            tutorial1.SetActive(true);
            tutorial0.SetActive(false);

            credits1.SetActive(false);
            credits0.SetActive(true);
        }

        if (state == 4)
        {
            resume1.SetActive(false);
            resume0.SetActive(true);

            restart1.SetActive(false);
            restart0.SetActive(true);

            tutorial1.SetActive(false);
            tutorial0.SetActive(true);

            credits1.SetActive(true);
            credits0.SetActive(false);
        }

    }

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void showPaused()
    {
        Debug.Log("show Paused");
        background.SetActive(true);
        updateMenu();
    }

    public void hidePaused()
    {
        Debug.Log("hide Paused");
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    public void showCredits()
    {
        Debug.Log("show Credits");
        background.SetActive(true);
        creditScreen.SetActive(true);
    }

    public void hideCredits()
    {
        Debug.Log("hide Credits");
        creditScreen.SetActive(false);
    }

    public void showTutorial()
    {
        Debug.Log("show Tutorial");
        background.SetActive(true);
        tutorialScreen.SetActive(true);
    }

    public void hideTutorial()
    {
        Debug.Log("hide Tutorial");
        tutorialScreen.SetActive(false);
    }
}
