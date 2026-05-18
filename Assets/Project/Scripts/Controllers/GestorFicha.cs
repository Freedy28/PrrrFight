using UnityEngine;
using TMPro; // Librería obligatoria para poder modificar textos HD

public class GestorFicha : MonoBehaviour
{
    [Header("Conexión a la Interfaz")]
    public TextMeshProUGUI textoDescripcion;

    // Este es el método público que llamaremos desde los botones/toggles
    public void ActualizarFicha(ClaseBaseSO datosDelPersonaje)
    {
        textoDescripcion.text = datosDelPersonaje.descripcionRol;
    }
}