using UnityEngine;

// Estados principales del flujo del juego
public enum EstadoJuego
{
    TurnoJugador1,
    TurnoJugador2,
    AnimandoAccion, // Bloquea los toques en pantalla mientras una pieza se mueve
    FinDePartida
}

// Fases internas de lo que hace el jugador en su turno
public enum FaseTurno
{
    EsperandoSeleccion, // Debe tocar un gato
    UnidadSeleccionada  // Ya tocó un gato, debe elegir a dónde moverlo
}

public class GestorPartida : MonoBehaviour
{
    [Header("Referencias")]
    public TableroLogico tablero;
    // public ControladorUnidadVisual controladorVisual; // Lo descomentaremos en la siguiente fase
    public Camera camaraPrincipal;

    [Header("Configuración Visual")]
    public float tamanoCelda = 1f; // Debe ser el mismo que tienes en tu ControladorUnidadVisual

    [Header("Estado Actual (Solo lectura)")]
    public EstadoJuego estadoActual;
    public FaseTurno faseActual;

    // Memoria temporal para la pieza que estamos moviendo
    private UnidadBase unidadSeleccionada;
    private Vector2Int coordenadaOrigen;

    void Start()
    {
        // Si no asignas una cámara en el inspector, busca la Main Camera automáticamente
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;

        estadoActual = EstadoJuego.TurnoJugador1;
        faseActual = FaseTurno.EsperandoSeleccion;
        Debug.Log("Inicia la partida. Turno del Jugador 1.");
    }

    void Update()
    {
        // Si hay una animación reproduciéndose o el juego terminó, ignoramos la pantalla
        if (estadoActual == EstadoJuego.AnimandoAccion || estadoActual == EstadoJuego.FinDePartida)
            return;

        DetectarInput();
    }

    private void DetectarInput()
    {
        // GetMouseButtonDown(0) es mágico en Unity: detecta el clic izquierdo en PC
        // y también detecta el primer toque (tap) en la pantalla de un dispositivo Android
        if (Input.GetMouseButtonDown(0))
        {
            // Evitamos que los clics sobre elementos de UI disparen un Raycast de escena
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2Int coordenadaTocada = ConvertirToqueACoordenada(Input.mousePosition);

            // Verificamos que el toque no devolviera la coordenada inválida (-1, -1)
            if (coordenadaTocada.x != -1)
            {
                ProcesarToque(coordenadaTocada);
            }
        }
    }

    private Vector2Int ConvertirToqueACoordenada(Vector3 posicionPantalla)
    {
        // 1. Creamos un rayo invisible que sale de la cámara y atraviesa el punto de la pantalla que tocaste
        Ray rayo = camaraPrincipal.ScreenPointToRay(posicionPantalla);
        RaycastHit impacto;

        // 2. Disparamos el rayo hacia la escena 3D
        if (Physics.Raycast(rayo, out impacto))
        {
            // 3. Convertimos el punto de impacto al espacio local del tablero para soportar
            //    cualquier posición, rotación o escala del GameObject del tablero
            Vector3 puntoLocal = tablero.transform.InverseTransformPoint(impacto.point);

            // 4. Traducción Inversa: Transformamos el espacio local a la cuadrícula 2D de TableroLogico
            int xLogica = Mathf.RoundToInt(puntoLocal.x / tamanoCelda);
            int yLogica = Mathf.RoundToInt(puntoLocal.z / tamanoCelda);

            // 5. Verificamos con TableroLogico que no hayamos tocado fuera de los límites
            if (tablero.EsCoordenadaValida(xLogica, yLogica))
            {
                return new Vector2Int(xLogica, yLogica);
            }
        }

        // Si tocaste al vacío o fuera del tablero, devolvemos este valor de error
        return new Vector2Int(-1, -1);
    }

    private void ProcesarToque(Vector2Int coordenada)
    {
        if (faseActual == FaseTurno.EsperandoSeleccion)
        {
            UnidadBase unidadTocada = tablero.ObtenerUnidadEn(coordenada.x, coordenada.y);

            if (unidadTocada != null)
            {
                // TODO: Más adelante validaremos si el gato le pertenece al Jugador 1 o 2
                unidadSeleccionada = unidadTocada;
                coordenadaOrigen = coordenada;
                faseActual = FaseTurno.UnidadSeleccionada;

                Debug.Log($"<color=green>Seleccionaste:</color> {unidadTocada.datosDeClase.nombreClase} en [{coordenada.x}, {coordenada.y}]");
            }
            else
            {
                Debug.Log("Tocaste una casilla vacía. Selecciona a un gato primero.");
            }
        }
        else if (faseActual == FaseTurno.UnidadSeleccionada)
        {
            // Si tocas el mismo gato que ya tenías seleccionado, se cancela la selección
            if (coordenada == coordenadaOrigen)
            {
                unidadSeleccionada = null;
                faseActual = FaseTurno.EsperandoSeleccion;
                Debug.Log("Unidad deseleccionada.");
                return;
            }

            // Llamamos a la lógica que corregimos anteriormente en TableroLogico
            bool movimientoExitoso = tablero.IntentarMoverUnidad(coordenadaOrigen.x, coordenadaOrigen.y, coordenada.x, coordenada.y);

            if (movimientoExitoso)
            {
                // Bloqueamos el juego mientras el gato camina
                estadoActual = EstadoJuego.AnimandoAccion;

                // TODO: Cuando conectemos la parte visual llamaremos a controladorVisual.IniciarMovimiento();

                // Limpiamos variables
                unidadSeleccionada = null;

                // Por ahora, simularemos que la animación de caminar fue instantánea y pasamos de turno
                CambiarTurno();
            }
            else
            {
                Debug.LogWarning("Destino inválido o fuera de rango. Selecciona otra casilla o toca al gato para deseleccionarlo.");
            }
        }
    }

    // Método para alternar el flujo del juego
    public void CambiarTurno()
    {
        estadoActual = (estadoActual == EstadoJuego.TurnoJugador1) ? EstadoJuego.TurnoJugador2 : EstadoJuego.TurnoJugador1;
        faseActual = FaseTurno.EsperandoSeleccion;
        Debug.Log($"<color=cyan>--- CAMBIO DE TURNO: {estadoActual} ---</color>");
    }
}