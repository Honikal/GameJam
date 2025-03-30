using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{
    //Para manejar el audio del juego tanto música como sfx
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;

    //Creamos un array de resoluciones que el usuario puede usar depende del dispositivo que usa
    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;

        //Limpiamos las opciones existentes y agregamos las nuevas dependiendo al dispositivo
        resolutionDropdown.ClearOptions();

        //Manejamos la resolución actual
        int currentResolutionIndex = 0;
        //Creamos una lista de resoluciones, hay que hacer casting para las resoluciones deseadas
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            //Ahora, nosotros cambiamos la resolución actual, si ya está en la lista, entonces lo colocamos, e indicamos que está seleccionado
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        //Cambiamos para que muestre el que la computadora esté usando actualmente
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetMusicVolume(float volume)
    {
        //Manejamos el slider de la música, de modo que al mover la música, se va a aumentar
        float dB = volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
        Debug.Log($"Volumen de Música: {dB}");
        audioMixer.SetFloat("MUSIC_volume", dB);
    }

    public void SetSFXVolume(float volume)
    {
        //Manejamos el slider de la música, de modo que al mover la música, se va a aumentar
        //Como manejamos el audio en ambos AudioManager por float y acá en decibeles, convertimos
        float dB = volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
        Debug.Log($"Volumen de SFX: {dB}");
        audioMixer.SetFloat("SFX_volume", dB);
    }

    //Con base a éste, acá manejaremos el AudioMixer, vamos a Window, y AudioMixer
    //No, ya probé y no, hay que ir a Create, Audio, AudioMixer


    public void SetFullScreen(bool isFullscreen)
    {
        //Aplicamos efecto de pantalla para pantalla completa o no
        Screen.fullScreen = isFullscreen;
    }

    public void setResolution(int resolutionIndex)
    {
        //Aplicamos entonces la resolución asignada, que recibe ancho, alto y si está en fullscreen
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
