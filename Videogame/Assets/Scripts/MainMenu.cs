using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Acá manejamos el código de la UI, manejamos el cambiar de escena
    public void PlayGame()
    {
        //Función a ejecutar cuando el juego se inicia (Ocupamos importar SceneManagement)
        //Podemos dar el nombre de la escena
        //Podemos dar el index de la escena
        //Acá cargaremos el siguiente nivel de acuerdo a la cola

        //SceneManager.LoadScene("Asteroids");
        //SceneManager.LoadScene(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //Claro pará ésto tenemos que agregar las escenas a la cola
        //File -> BuildProfile -> OpenSceneList -> AddOpenScene 
    }

    public void QuitGame()
    {
        Debug.Log("Cerramos el juego");
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        //De acuerdo al juego, acá MainMenu tiene 0, y Game tiene 1
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ReloadGame()
    {
        //Llamamos al método para reiniciar el juego
        //GameManager.Instance.ResetGame();

        //Recargar la escena actual
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
