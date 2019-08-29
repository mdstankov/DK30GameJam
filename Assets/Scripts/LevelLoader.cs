using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

	public void StartGame( )
	{
		//SceneManager.LoadScene( "3D Test Scene" );
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

	public void ResetSaveProgress( )
	{		
		SaveDialogProgress blank_save = new SaveDialogProgress( );
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
		bf.Serialize(file, blank_save);
		file.Close( );
	}


}

