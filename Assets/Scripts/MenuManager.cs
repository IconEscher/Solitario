using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	[SerializeField]
	GameObject newGameCanvas;
	public bool gamePaused;
	Scene activeScene;

	void Start()
	{
		activeScene = SceneManager.GetActiveScene();
		if (activeScene.name == "2D") {
			newGameCanvas = GameObject.Find ("Canvas_NewGame");
			newGameCanvas.SetActive (false);
		}
	}

	public void GoToMenu()
	{
		SceneManager.LoadScene ("Menu");
	}

	public void StartGame()
    {
		SceneManager.LoadScene("2D");
    }

	public void ActivateMenuNewGame()
	{
		newGameCanvas.SetActive (true);
		gamePaused = true;
		SetTimeScale ();
	}

	public void Resume()
	{
		newGameCanvas.SetActive (false);
		gamePaused = false;
		SetTimeScale ();
	}

	public void SetTimeScale()
	{
		if (gamePaused) {
			Time.timeScale = 0f;
		} else
			Time.timeScale = 1f;

	}

    public void Quit()
    {
        Application.Quit();
    }
}
