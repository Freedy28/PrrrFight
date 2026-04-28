using UnityEngine;

// Esto agrega una opción al menú de Unity al hacer clic derecho
[CreateAssetMenu(fileName = "NuevaClaseLuchador", menuName = "Prrrfight/Clase de Luchador")]
public class ClaseLuchadorS0 : ScriptableObject
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