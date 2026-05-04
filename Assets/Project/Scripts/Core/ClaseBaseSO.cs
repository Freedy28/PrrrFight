using UnityEngine;

// ScriptableObject base compartido por todas las clases de unidad (Tanque, Tirador, Luchador, Clérigo)
public class ClaseBaseSO : ScriptableObject
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
