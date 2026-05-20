using UnityEngine;

public class GeneradorTablero : MonoBehaviour
{
    [Header("Ajustes del Tablero")]
    public int ancho = 6;
    public int alto = 10;
    public float tamanoCelda = 1f;

    [Header("Visual")]
    public GameObject prefabCasilla;
    public Material materialClaro;
    public Material materialOscuro;

    void Start()
    {
        // En el juego real, esto se llamará y construirá el tablero.
        GenerarTableroVisual();
    }

    [ContextMenu("Generar Tablero Visualmente")]
    public void GenerarTableroVisual()
    {
        if (prefabCasilla == null)
        {
            Debug.LogError("GeneradorTablero: prefabCasilla no está asignado. Abortando generación del tablero.", this);
            return;
        }

        // Limpiamos el tablero (usamos DestroyImmediate para el modo Edición)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Si tu prefabCasilla es un plano por defecto de Unity, normalmente mide 10x10.
        // Si es un "Quad" o un cubo de 1x1, este cálculo lo centrará perfecto.
        for (int x = 0; x < ancho; x++)
        {
            for (int y = 0; y < alto; y++)
            {
                GameObject nuevaCasilla = Instantiate(prefabCasilla, this.transform);

                // AQUÍ ESTÁ EL CAMBIO MÁGICO:
                // Colocamos la casilla exactamente en las coordenadas X e Y enteras.
                // Si tu prefab dibuja desde la esquina en vez del centro, 
                // aquí es donde lo compensamos.
                nuevaCasilla.transform.localPosition = new Vector3(x * tamanoCelda, 0, y * tamanoCelda);

                // Ahora copia la rotación exacta que tiene tu Prefab (los 90 grados en X)
                nuevaCasilla.transform.rotation = prefabCasilla.transform.rotation; nuevaCasilla.name = $"Casilla_{x}_{y}";

                MeshRenderer render = nuevaCasilla.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.sharedMaterial = (x + y) % 2 == 0 ? materialClaro : materialOscuro;
                }

                if (nuevaCasilla.GetComponent<Collider>() == null)
                {
                    nuevaCasilla.AddComponent<BoxCollider>();
                }
            }
        }
        Debug.Log($"Tablero de {ancho}x{alto} generado visualmente.");
    }
}