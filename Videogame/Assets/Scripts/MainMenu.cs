using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    //Ac� manejamos el c�digo de la UI, manejamos el cambiar de escena
    private void Start()
    {
        //Reproducimos la m�sica apenas iniciar
        AudioManager.Instance.Play("BackgroundMusic");
    }

    public void PlayGame()
    {
        //Funci�n a ejecutar cuando el juego se inicia (Ocupamos importar SceneManagement)
        //Podemos dar el nombre de la escena
        //Podemos dar el index de la escena
        //Ac� cargaremos el siguiente nivel de acuerdo a la cola

        //SceneManager.LoadScene("Asteroids");
        //SceneManager.LoadScene(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //Claro par� �sto tenemos que agregar las escenas a la cola
        //File -> BuildProfile -> OpenSceneList -> AddOpenScene 
    }

    public void QuitGame()
    {
        Debug.Log("Cerramos el juego");
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        //De acuerdo al juego, ac� MainMenu tiene 0, y Game tiene 1
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ReloadGame()
    {
        //Llamamos al m�todo para reiniciar el juego
        //GameManager.Instance.ResetGame();

        //Recargar la escena actual
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Scrolling Credits
    public float scrollSpeed = 40f;
    public RectTransform rectTransform;

    private bool isScrool { get; set; } = false;

    private void Update()
    {
        if (isScrool)
        {
            rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }
    }

    public void ScrollCredits(bool value)
    {
        isScrool = value;
    }
}
