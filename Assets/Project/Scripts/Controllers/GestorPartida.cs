using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

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
    public ControladorUnidadVisual controladorVisual;
    public Camera camaraPrincipal;

    [Header("Configuración Visual")]
    public float tamanoCelda = 1f; // Debe ser el mismo que tienes en tu ControladorUnidadVisual

    [Header("Input / Raycast")]
    public LayerMask mascaraTablero = 0;

    [Header("Estado Actual (Solo lectura)")]
    public EstadoJuego estadoActual;
    public FaseTurno faseActual;

    // Memoria temporal para la pieza que estamos moviendo
    private UnidadBase unidadSeleccionada;
    private Vector2Int coordenadaOrigen;

    // Jugador actual independiente del estado de animación (1 o 2)
    private int jugadorActual = 1;

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    private static void LogDev(string mensaje)
    {
        UnityEngine.Debug.Log(mensaje);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    private static void LogWarningDev(string mensaje)
    {
        UnityEngine.Debug.LogWarning(mensaje);
    }

    void Start()
    {
        ValidarReferenciasEscena();

        // Si no asignas una cámara en el inspector, busca la Main Camera automáticamente
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;

        if (mascaraTablero.value == 0)
        {
            mascaraTablero = 1 << tablero.gameObject.layer;
        }

        if (controladorVisual != null)
            controladorVisual.MovimientoFinalizado += OnMovimientoVisualFinalizado;

        // Registramos las unidades ya presentes en la escena antes de permitir input
        RegistrarUnidadesIniciales();

        estadoActual = EstadoJuego.TurnoJugador1;
        faseActual = FaseTurno.EsperandoSeleccion;
        LogDev("Inicia la partida. Turno del Jugador 1.");
    }

    private void OnDestroy()
    {
        if (controladorVisual != null)
            controladorVisual.MovimientoFinalizado -= OnMovimientoVisualFinalizado;
    }

    // Busca todos los UnidadBase en la escena y los registra en el tablero lógico
    private void RegistrarUnidadesIniciales()
    {
        UnidadBase[] unidades = FindObjectsByType<UnidadBase>(FindObjectsSortMode.None);
        HashSet<Vector2Int> coordenadasUsadas = new HashSet<Vector2Int>();

        foreach (UnidadBase unidad in unidades)
        {
            Vector2Int coordenadaDetectada = ObtenerCoordenadaDesdeTransform(unidad.transform.position);
            unidad.coordenadaInicial = coordenadaDetectada;

            if (!tablero.EsCoordenadaValida(unidad.coordenadaInicial.x, unidad.coordenadaInicial.y))
            {
                LogWarningDev($"'{unidad.name}' quedó fuera del tablero con coordenada [{unidad.coordenadaInicial.x},{unidad.coordenadaInicial.y}].");
                continue;
            }

            if (!coordenadasUsadas.Add(unidad.coordenadaInicial))
            {
                LogWarningDev($"Coordenada duplicada detectada en [{unidad.coordenadaInicial.x},{unidad.coordenadaInicial.y}] para '{unidad.name}'.");
                continue;
            }

            bool registrada = tablero.RegistrarUnidad(unidad, unidad.coordenadaInicial.x, unidad.coordenadaInicial.y);
            if (registrada)
                LogDev($"Unidad '{unidad.datosDeClase?.nombreClase}' registrada en [{unidad.coordenadaInicial.x},{unidad.coordenadaInicial.y}].");
            else
                LogWarningDev($"No se pudo registrar '{unidad.datosDeClase?.nombreClase}' en [{unidad.coordenadaInicial.x},{unidad.coordenadaInicial.y}]: coordenada inválida u ocupada.");
        }
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
        if (!TryObtenerPosicionInput(out Vector3 posicionInput))
            return;

        Vector2Int coordenadaTocada = ConvertirToqueACoordenada(posicionInput);

        // Verificamos que el toque no devolviera la coordenada inválida (-1, -1)
        if (coordenadaTocada.x != -1)
        {
            ProcesarToque(coordenadaTocada);
        }
    }

    private bool TryObtenerPosicionInput(out Vector3 posicionInput)
    {
        posicionInput = Vector3.zero;

        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);
            if (toque.phase != TouchPhase.Began) return false;

            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(toque.fingerId))
                return false;

            posicionInput = toque.position;
            return true;
        }

        if (!Input.GetMouseButtonDown(0)) return false;

        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return false;

        posicionInput = Input.mousePosition;
        return true;
    }

    private Vector2Int ConvertirToqueACoordenada(Vector3 posicionPantalla)
    {
        // 1. Creamos un rayo invisible que sale de la cámara y atraviesa el punto de la pantalla que tocaste
        Ray rayo = camaraPrincipal.ScreenPointToRay(posicionPantalla);
        RaycastHit impacto;

        // 2. Disparamos el rayo hacia la escena 3D
        if (Physics.Raycast(rayo, out impacto, Mathf.Infinity, mascaraTablero))
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

                LogDev($"<color=green>Seleccionaste:</color> {unidadTocada.datosDeClase.nombreClase} en [{coordenada.x}, {coordenada.y}]");
            }
            else
            {
                LogDev("Tocaste una casilla vacía. Selecciona a un gato primero.");
            }
        }
        else if (faseActual == FaseTurno.UnidadSeleccionada)
        {
            // Si tocas el mismo gato que ya tenías seleccionado, se cancela la selección
            if (coordenada == coordenadaOrigen)
            {
                unidadSeleccionada = null;
                faseActual = FaseTurno.EsperandoSeleccion;
                LogDev("Unidad deseleccionada.");
                return;
            }

            // Llamamos a la lógica que corregimos anteriormente en TableroLogico
            bool movimientoExitoso = tablero.IntentarMoverUnidad(coordenadaOrigen.x, coordenadaOrigen.y, coordenada.x, coordenada.y);

            if (movimientoExitoso)
            {
                // Bloqueamos el juego mientras el gato camina
                estadoActual = EstadoJuego.AnimandoAccion;
                List<Vector2Int> rutaVisual = ConstruirRutaVisualSimple(coordenadaOrigen, coordenada);
                unidadSeleccionada.coordenadaInicial = coordenada;

                if (controladorVisual != null && rutaVisual != null && rutaVisual.Count > 0)
                {
                    controladorVisual.IniciarMovimiento(rutaVisual);
                }
                else
                {
                    OnMovimientoVisualFinalizado();
                }

                // Limpiamos variables
                unidadSeleccionada = null;
            }
            else
            {
                LogWarningDev("Destino inválido o fuera de rango. Selecciona otra casilla o toca al gato para deseleccionarlo.");
            }
        }
    }

    private void OnMovimientoVisualFinalizado()
    {
        if (estadoActual != EstadoJuego.AnimandoAccion)
            return;

        CambiarTurno();
    }

    private List<Vector2Int> ConstruirRutaVisualSimple(Vector2Int origen, Vector2Int destino)
    {
        int deltaX = destino.x - origen.x;
        int deltaY = destino.y - origen.y;
        int pasos = Mathf.Max(Mathf.Abs(deltaX), Mathf.Abs(deltaY));
        if (pasos <= 0) return null;

        bool esOrtogonal = deltaX == 0 || deltaY == 0;
        int pasoX = TableroLogico.CalcularPaso(deltaX, esOrtogonal);
        int pasoY = TableroLogico.CalcularPaso(deltaY, esOrtogonal);

        List<Vector2Int> ruta = new List<Vector2Int>(pasos);
        Vector2Int actual = origen;
        for (int i = 0; i < pasos; i++)
        {
            actual = new Vector2Int(actual.x + pasoX, actual.y + pasoY);
            ruta.Add(actual);
        }

        return ruta;
    }

    private Vector2Int ObtenerCoordenadaDesdeTransform(Vector3 posicionMundo)
    {
        Vector3 puntoLocal = tablero.transform.InverseTransformPoint(posicionMundo);
        int xLogica = Mathf.RoundToInt(puntoLocal.x / tamanoCelda);
        int yLogica = Mathf.RoundToInt(puntoLocal.z / tamanoCelda);
        return new Vector2Int(xLogica, yLogica);
    }

    private void ValidarReferenciasEscena()
    {
        if (tablero == null)
        {
            UnityEngine.Debug.LogError("GestorPartida requiere referencia a TableroLogico.", this);
            enabled = false;
            return;
        }
    }

    // Método para alternar el flujo del juego
    public void CambiarTurno()
    {
        // Alternamos basándonos en jugadorActual, no en estadoActual, para que el estado
        // AnimandoAccion no interfiera con la lógica de cambio de turno.
        jugadorActual = (jugadorActual == 1) ? 2 : 1;
        estadoActual = (jugadorActual == 1) ? EstadoJuego.TurnoJugador1 : EstadoJuego.TurnoJugador2;
        faseActual = FaseTurno.EsperandoSeleccion;
        LogDev($"<color=cyan>--- CAMBIO DE TURNO: {estadoActual} ---</color>");
    }
}
