using UnityEngine;

public class GeneradorTablero : MonoBehaviour
{
    [Header("Ajustes del Tablero")]
    public int ancho = 6;
    public int alto = 10;
    public float tamanoCelda = 1f;

    [Header("Visual")]
    public GameObject prefabCasilla; // Un plano o cubo que represente la casilla
    public Material materialClaro;
    public Material materialOscuro;

    void Start()
    {
        GenerarTableroVisual();
    }

    public void GenerarTableroVisual()
    {
        // Validar que el prefab esté asignado antes de proceder
        if (prefabCasilla == null)
        {
            Debug.LogError("GeneradorTablero: prefabCasilla no está asignado. Abortando generación del tablero.", this);
            enabled = false;
            return;
        }

        // Limpiamos el tablero si ya existía algo (útil para prototipar)
        foreach (Transform hijo in transform)
        {
            Destroy(hijo.gameObject);
        }

        for (int x = 0; x < ancho; x++)
        {
            for (int y = 0; y < alto; y++)
            {
                // Instanciamos la casilla como hijo del tablero y usamos posición local
                // para que el tablero pueda moverse/rotarse sin desalinear la grilla
                GameObject nuevaCasilla = Instantiate(prefabCasilla, this.transform);
                nuevaCasilla.transform.localPosition = new Vector3(x * tamanoCelda, 0, y * tamanoCelda);
                nuevaCasilla.transform.localRotation = Quaternion.identity;
                nuevaCasilla.name = $"Casilla_{x}_{y}";

                // Aplicamos color tipo ajedrez si tenemos los materiales
                MeshRenderer render = nuevaCasilla.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.sharedMaterial = (x + y) % 2 == 0 ? materialClaro : materialOscuro;
                }

                // IMPORTANTE: Asegurarnos de que tenga un Collider para el Raycast
                if (nuevaCasilla.GetComponent<Collider>() == null)
                {
                    nuevaCasilla.AddComponent<BoxCollider>();
                }
            }
        }

        // Opcional: Centrar la cámara o el tablero
        Debug.Log($"Tablero de {ancho}x{alto} generado visualmente.");
    }
}