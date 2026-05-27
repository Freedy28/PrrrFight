using UnityEngine;

// ScriptableObject base compartido por todas las clases de unidad (Tanque, Tirador, Luchador, Clérigo)
//public class ClaseBaseSO : ScriptableObject
//{
//    [Header("Información Principal")]
//    public string nombreClase; // Ej: "Tanque" o "Clérigo"
//    [TextArea]
//    public string descripcionRol; // Ej: "Primera línea de batalla..."
//    public Sprite iconoClase; // Aquí tus diseñadores arrastrarán el arte 2D

//    [Header("Estadísticas Base")]
//    public int puntosSaludBase; // PS (Ej: 20 para el Tanque)

//    [Header("Movilidad")]
//    public int movimientoOrtogonal; // Casillas rectas (Ej: 2)
//    public int movimientoDiagonal;  // Casillas en diagonal (Ej: 1)

//    // 👇 ESTO ES LO NUEVO QUE CONECTARÁ TU INTERFAZ CON SU TABLERO 👇
//    [Header("Variantes de Skins (Para UI y Tablero)")]
//    public Sprite[] spritesSkins = new Sprite[3]; // Aquí pondrás tus 3 caritas del menú
//    public GameObject[] prefabsSkins = new GameObject[3]; // Aquí él pondrá los 3 prefabs
//}

[CreateAssetMenu(fileName = "NuevaClase", menuName = "PrrrFight/Clase de Unidad")]
public class ClaseBaseSO : ScriptableObject
{
    [Header("Información Principal")]
    public string nombreClase;// Ej: "Tanque" o "Clérigo"
    [TextArea] public string descripcionRol;// Ej: "Primera línea de batalla..."

    public Sprite iconoClase; // Aquí tus diseñadores arrastrarán el arte 2D

    [Header("Estadísticas Base")]
    public int puntosSaludBase; // PS (Ej: 20 para el Tanque)

    [Header("Movilidad")]
    public int movimientoOrtogonal; // Casillas rectas (Ej: 2)
    public int movimientoDiagonal;  // Casillas en diagonal (Ej: 1)


    [Header("Iconos Estáticos (Para los slots de abajo)")]
    public Sprite[] spritesSkins = new Sprite[3]; // Aquí pones las fotos fijas

    [Header("Prefabs Animados (Para tu amigo)")]
    public GameObject[] prefabsSkins = new GameObject[3];
}