using UnityEngine;
using UnityEngine.UI; // Requisito para controlar Sliders

public class GestorAjustes : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Slider sliderMusica;
    public Slider sliderEfectos;

    private void Start()
    {
        // Al abrir el menú, fuerza a los sliders a moverse a la posición correcta leyendo el AudioManager
        if (AudioManager.instance != null)
        {
            sliderMusica.value = AudioManager.instance.musicSource.volume;
            sliderEfectos.value = AudioManager.instance.sfxSource.volume;
        }
    }

    // Estos métodos serán llamados dinámicamente por los Sliders
    public void CambiarVolumenMusica(float valor)
    {
        AudioManager.instance.SetMusicVolume(valor);
    }

    public void CambiarVolumenEfectos(float valor)
    {
        AudioManager.instance.SetSFXVolume(valor);
    }
}