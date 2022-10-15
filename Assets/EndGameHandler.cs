using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        else if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
    }
}
