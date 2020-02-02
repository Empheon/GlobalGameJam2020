using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("Scenes/Main", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
