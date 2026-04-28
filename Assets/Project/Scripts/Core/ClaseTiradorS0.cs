using UnityEngine;

// Esto agrega una opción al menú de Unity al hacer clic derecho
[CreateAssetMenu(fileName = "NuevaClaseTirador", menuName = "Prrrfight/Clase de Tirador")]
public class ClaseTiradorS0 : ScriptableObject
{
    [Header("Información Principal")]
    public string nombreClase; // Ej: "Tanque" o "Clérigo"
    [TextArea]
    public string descripcionRol; // Ej: "Primera línea de batalla..."
    public Sprite iconoClase; // Aquí tus diseñadores arrastrarán el arte 2D

    [Header("Estadísticas Base")]
    public int puntosSaludBase; // PS (Ej: 20 para el Tanque)

    [Header("Movilidad")]
    public int movimientoOrtogonal; // Casillas rectas (Ej: 2)
    public int movimientoDiagonal;  // Casillas en diagonal (Ej: 1)
}