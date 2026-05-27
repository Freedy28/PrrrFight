using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestorFicha : MonoBehaviour
{
    [Header("Conexión a la Interfaz")]
    public TextMeshProUGUI textoDescripcion;
    public GameObject[] panelesDeSkins; // Arrastra los 4 paneles aquí

    [Header("Conexión al Equipo")]
    public GestorEquipo gestorEquipo;

    [Header("Arrastra tus 4 Archivos Data de la carpeta Clases")]
    public ClaseBaseSO datosLuchador;
    public ClaseBaseSO datosTanque;
    public ClaseBaseSO datosTirador;
    public ClaseBaseSO datosClerigo;

    // Aquí guardamos la clase que está viendo el jugador en este momento
    private ClaseBaseSO claseActiva;

    void Start()
    {
        // Por defecto, iniciamos mostrando la clase Luchador (Guerrero) al abrir el menú
        MostrarClase(0);
    }

    // Los botones/toggles de la izquierda llamarán a esto pasando un número: 0, 1, 2 o 3
    public void MostrarClase(int indiceClase)
    {
        for (int i = 0; i < panelesDeSkins.Length; i++) panelesDeSkins[i].SetActive(false);
        panelesDeSkins[indiceClase].SetActive(true);

        if (indiceClase == 0) claseActiva = datosLuchador;
        else if (indiceClase == 1) claseActiva = datosTanque;
        else if (indiceClase == 2) claseActiva = datosTirador;
        else if (indiceClase == 3) claseActiva = datosClerigo;

        // Cambiamos el texto del pergamino con la descripción de la clase
        textoDescripcion.text = claseActiva.descripcionRol;

        // Cambiamos los 3 botones dinámicos con los sprites que guardaste en el ScriptableObject
        //for (int i = 0; i < 3; i++)
        //{
        //    if (claseActiva.spritesSkins[i] != null)
        //    {
        //        botonesSkins[i].sprite = claseActiva.spritesSkins[i];
        //    }
        //}
    }

    // Tus 3 botones genéricos (btn_Skin1, 2, 3) llamarán a esto pasando su propio número (0, 1 o 2)
    public void TocarBotonSkin(int numeroBoton)
    {
        // Le mandamos al equipo de abajo la Clase Activa y qué número de variante (skin) eligió
        gestorEquipo.AgregarGatito(claseActiva, numeroBoton);
    }
}