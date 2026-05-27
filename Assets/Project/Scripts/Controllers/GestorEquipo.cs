using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GestorEquipo : MonoBehaviour
{
    // Una pequeña estructura para guardar la combinación exacta de clase y skin
    [System.Serializable]
    public struct GatoSeleccionado
    {
        public ClaseBaseSO claseDatos;
        public int indiceSkin;
    }

    [Header("Conexión con la UI")]
    public Image[] imagenesSlotsEquipo; // Las 3 imágenes de los cuadritos grises
    public Sprite spriteCuadroGris;     // Imagen por defecto del cuadro gris
    public Button btnContinuar;

    [Header("Popup de Confirmación")]
    public GameObject panelConfirmacion;

    // Ahora guardamos la Ficha de Datos completa.
    // Se podrá acceder a "gatosElegidos[0].prefabTablero" mas facil.
    private int slotQueSeQuiereBorrar = -1;
    public List<GatoSeleccionado> gatosElegidos = new List<GatoSeleccionado>();
    void Start()
    {
        panelConfirmacion.SetActive(false);
        ActualizarPantalla();
    }

    // Esto lo llamarán los botones de skins
    public void AgregarGatito(ClaseBaseSO clase, int skin)
    {
        //Sprite skinSeleccionada = nuevoGato.sprite;

        // Validamos que no se meta dos veces exactamente al mismo gato (misma clase y misma skin)
        bool yaExiste = gatosElegidos.Exists(g => g.claseDatos == clase && g.indiceSkin == skin);

        // Revisa que haya espacio (< 3) y que no se haya metido ya a ese mismo gatito
        if (gatosElegidos.Count < 3 && !yaExiste)
        {
            GatoSeleccionado nuevoGato = new GatoSeleccionado { claseDatos = clase, indiceSkin = skin };
            gatosElegidos.Add(nuevoGato);
            ActualizarPantalla();
        }
    }

    // Esto lo llamarán los 3 botones de abajo
    public void TocarSlot(int numeroDeSlot)
    {
        // Solo abre el aviso si tocaste un slot que sí tiene un gato adentro
        if (numeroDeSlot < gatosElegidos.Count)
        {
            slotQueSeQuiereBorrar = numeroDeSlot;
            panelConfirmacion.SetActive(true); // Prende la ventana
        }
    }

    //botón "Sí" del panel
    public void ConfirmarBorrado()
    {
        if (slotQueSeQuiereBorrar != -1)
        {
            gatosElegidos.RemoveAt(slotQueSeQuiereBorrar);
            slotQueSeQuiereBorrar = -1;
            panelConfirmacion.SetActive(false); // Apaga la ventana
            ActualizarPantalla();
        }
    }

    //botón "NO" del panel
    public void CancelarBorrado()
    {
        slotQueSeQuiereBorrar = -1;
        panelConfirmacion.SetActive(false);
    }

    //Dibuja los gatos y prende el botón
    private void ActualizarPantalla()
    {
        for (int i = 0; i < imagenesSlotsEquipo.Length; i++)
        {
            if (i < gatosElegidos.Count)
            {
                // Obtenemos el sprite correcto desde el ScriptableObject usando el índice guardado
                ClaseBaseSO claseGato = gatosElegidos[i].claseDatos;
                int skinGato = gatosElegidos[i].indiceSkin;

                imagenesSlotsEquipo[i].sprite = claseGato.spritesSkins[skinGato]; // Pone al gato
                imagenesSlotsEquipo[i].color = Color.white;
            }
            else
            {
                imagenesSlotsEquipo[i].sprite = spriteCuadroGris; // Regresa al gris
                imagenesSlotsEquipo[i].color = new Color(0.2f, 0.2f, 0.2f); // Tono oscurito
            }
        }

        // Si están los 3 gatos se habilita el botón. Si no se bloquea.
        btnContinuar.interactable = (gatosElegidos.Count == 3);
    }
}
