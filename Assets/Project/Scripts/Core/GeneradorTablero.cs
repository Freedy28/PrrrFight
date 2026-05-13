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
        // Limpiamos el tablero si ya existía algo (útil para prototipar)
        foreach (Transform hijo in transform)
        {
            Destroy(hijo.gameObject);
        }

        for (int x = 0; x < ancho; x++)
        {
            for (int y = 0; y < alto; y++)
            {
                // Calculamos la posición física (X, 0, Z)
                Vector3 posicion = new Vector3(x * tamanoCelda, 0, y * tamanoCelda);

                // Instanciamos la casilla
                GameObject nuevaCasilla = Instantiate(prefabCasilla, posicion, Quaternion.identity, this.transform);
                nuevaCasilla.name = $"Casilla_{x}_{y}";

                // Aplicamos color tipo ajedrez si tenemos los materiales
                MeshRenderer render = nuevaCasilla.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.material = (x + y) % 2 == 0 ? materialClaro : materialOscuro;
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