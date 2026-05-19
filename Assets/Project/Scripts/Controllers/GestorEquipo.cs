using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GestorEquipo : MonoBehaviour
{
    [Header("Conexión con la UI")]
    public Image[] imagenesSlotsEquipo; // Las 3 imágenes de los cuadritos grises
    public Sprite spriteCuadroGris;     // Imagen por defecto del cuadro gris
    public Button btnContinuar;

    [Header("Popup de Confirmación")]
    public GameObject panelConfirmacion;

    private int slotQueSeQuiereBorrar = -1;
    private List<Sprite> gatosElegidos = new List<Sprite>(); // Lista que guarda a los gatos

    void Start()
    {
        panelConfirmacion.SetActive(false);
        ActualizarPantalla();
    }

    // Esto lo llamarán los botones de skins
    public void AgregarGatito(Image imagenDelGatoQuePique)
    {
        Sprite skinSeleccionada = imagenDelGatoQuePique.sprite;

        // Revisa que haya espacio (< 3) y que no se haya metido ya a ese mismo gatito
        if (gatosElegidos.Count < 3 && !gatosElegidos.Contains(skinSeleccionada))
        {
            gatosElegidos.Add(skinSeleccionada);
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

    //botón "SÍ" del panel
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
                imagenesSlotsEquipo[i].sprite = gatosElegidos[i]; // Pone al gato
                imagenesSlotsEquipo[i].color = Color.white;
            }
            else
            {
                imagenesSlotsEquipo[i].sprite = spriteCuadroGris; // Regresa al gris
                imagenesSlotsEquipo[i].color = new Color(0.2f, 0.2f, 0.2f); // Tono oscurito
            }
        }

        // Si estan los 3 gatos se habilita el botón. Si no se bloquea.
        btnContinuar.interactable = (gatosElegidos.Count == 3);
    }
}