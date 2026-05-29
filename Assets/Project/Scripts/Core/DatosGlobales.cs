using UnityEngine;
using System.Collections.Generic;

public class DatosGlobales : MonoBehaviour
{
    // Esta es la instancia única a la que todos pueden acceder
    public static DatosGlobales Instancia;

    // Aquí guardamos la lista de gatos para que viaje entre escenas
    public List<GestorEquipo.GatoSeleccionado> equipoJugador = new List<GestorEquipo.GatoSeleccionado>();

    private void Awake()
    {
        // Si ya existe una mochila, destruimos esta nueva para no tener duplicados
        if (Instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        // Si somos la única mochila, nos guardamos en la variable Instancia
        Instancia = this;
        // Le decimos a Unity que no destruya este objeto al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }
}