using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Levels
{
    Lobby = 0,  
    Game = 1,
}

public class LevelLoader : MonoBehaviour
{

    // Start is called before the first frame update
    void Start( )
    {

    }

    // Update is called once per frame
    void Update( )
    {
        
    }

	public void LoadGameLevel( )
	{
		SceneManager.LoadScene("GameLevel");
	}
	
	public void LoadLevel( Levels level )
	{
		switch( level )
		{
			case Levels.Lobby:
				SceneManager.LoadScene("Lobby");
				break;

			case Levels.Game:
				SceneManager.LoadScene("GameLevel");
				break;

			default:
				break;
		}
	}

	public void QuitGame()
    {
        Application.Quit();
    }
}

