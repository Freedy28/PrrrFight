using UnityEngine;


// Clase base para todas las unidades del tablero
public class UnidadBase : MonoBehaviour
{
    // Cada unidad tiene datos de clase asignados desde el Inspector
    public ClaseBaseSO datosDeClase;
    [Header("Configuración de equipo")]
    [Tooltip("1 para el Jugador 1, 2 para el Jugador 2")]
    public int idEquipo = 1;
    // Coordenada inicial dentro del tablero lógico (asignar desde el Inspector)
    public Vector2Int coordenadaInicial;

    // Puntos de salud actuales durante la partida
    protected int saludActual;

    protected virtual void Start()
    {
        if (datosDeClase == null)
        {
            Debug.LogError("No se ha asignado ningún ScriptableObject a datosDeClase.", this);
            enabled = false;
            return;
        }

        saludActual = datosDeClase.puntosSaludBase;
        Debug.Log($"Ha entrado al tablero un {datosDeClase.nombreClase} con {saludActual} PS.");
    }

    public virtual void RecibirDano(int cantidad)
    {
        if (datosDeClase == null || cantidad <= 0) return;
        saludActual = Mathf.Max(0, saludActual - cantidad);
        Debug.Log($"{datosDeClase.nombreClase} recibió daño. PS restantes: {saludActual}");
    }
}
