using UnityEngine;
using System.Collections.Generic;

public class GeneradorEquipo : MonoBehaviour
{
    [Header("Configuración de Posiciones")]
    // Aquí se pueden poner las posiciones iniciales donde quiere que aparezcan los gatos
    public Transform[] posicionesIniciales;

    void Start()
    {
        GenerarGatos();
    }

    void GenerarGatos()
    {
        // Revisamos si la mochila llegó con datos
        if (DatosGlobales.Instancia != null && DatosGlobales.Instancia.equipoJugador.Count > 0)
        {
            List<GestorEquipo.GatoSeleccionado> equipo = DatosGlobales.Instancia.equipoJugador;

            // Recorremos los 3 gatos elegidos
            for (int i = 0; i < equipo.Count; i++)
            {
                ClaseBaseSO claseDatos = equipo[i].claseDatos;
                int skinIndex = equipo[i].indiceSkin;

                GameObject prefabParaInstanciar = claseDatos.prefabsSkins[skinIndex];

                // Instanciamos el Prefab y lo guardamos en una variable
                if (prefabParaInstanciar != null && i < posicionesIniciales.Length)
                {
                    // Apagamos el prefab temporalmente ANTES de clonarlo
                    prefabParaInstanciar.SetActive(false);
                    
                    // Lo clonamos
                    GameObject nuevoGato = Instantiate(prefabParaInstanciar, posicionesIniciales[i].position, posicionesIniciales[i].rotation);

                    // Volvemos a prender el prefab original en los archivos
                    prefabParaInstanciar.SetActive(true);

                    // Buscamos el script UnidadBase en el gato recién clonado
                    UnidadBase cerebro = nuevoGato.GetComponent<UnidadBase>();

                    // Si lo encontramos, le borramos su memoria de "Luchador" y le ponemos los datos del menú
                    if (cerebro != null)
                    {
                        cerebro.datosDeClase = claseDatos;
                    }
                    // Ahora sí, lo prendemos! Y ejecutará su Start() con el cerebro correcto.
                    nuevoGato.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("Falta el prefab en el ScriptableObject o faltan posiciones iniciales.");
                }
            }
        }
        else
        {
            Debug.Log("No se encontraron datos del equipo. Probablemente entraste directo a la escena del Tablero.");
        }
    }
}