using UnityEngine;

public class UnidadGuerrero : MonoBehaviour
{
    // Aquí arrastraremos el archivo de datos (Ej: TanqueData)
    public ClaseGuerreroSO datosDeClase;

    // Variables actuales durante la partida
    private int saludActual;

    void Start()
    {
        // Al iniciar, la unidad toma los datos del ScriptableObject
        saludActual = datosDeClase.puntosSaludBase;

        Debug.Log($"Ha entrado al tablero un {datosDeClase.nombreClase} con {saludActual} PS.");
    }

    public void RecibirDano(int cantidad)
    {
        saludActual -= cantidad;
        Debug.Log($"{datosDeClase.nombreClase} recibió daño. PS restantes: {saludActual}");
    }
}