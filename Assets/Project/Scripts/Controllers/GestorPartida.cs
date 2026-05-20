using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // <-- Agregado para el Nuevo Input System
using static UnityEditor.PlayerSettings;

public enum EstadoJuego { TurnoJugador1, TurnoJugador2, AnimandoAccion, FinDePartida }
public enum FaseTurno { EsperandoSeleccion, UnidadSeleccionada }

public class GestorPartida : MonoBehaviour
{
    [Header("Referencias")]
    public TableroLogico tablero;
    public Camera camaraPrincipal;

    [Header("Configuración Visual")]
    public float tamanoCelda = 1f;

    [Header("Resaltado Visual")]
    public GameObject prefabResaltado; // El cuadrito azul brillante
    private List<GameObject> casillasResaltadasInstanciadas = new List<GameObject>(); // Para borrarlas después

    [Header("Estado Actual (Solo lectura)")]
    public EstadoJuego estadoActual;
    public FaseTurno faseActual;

    private UnidadBase unidadSeleccionada;
    private Vector2Int coordenadaOrigen;
    private int jugadorActual = 1;

    void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        RegistrarUnidadesIniciales();
        estadoActual = EstadoJuego.TurnoJugador1;
        faseActual = FaseTurno.EsperandoSeleccion;
    }

    private void RegistrarUnidadesIniciales()
    {
        UnidadBase[] unidades = FindObjectsByType<UnidadBase>(FindObjectsSortMode.None);
        foreach (UnidadBase unidad in unidades)
        {
            bool registrada = tablero.RegistrarUnidad(unidad, unidad.coordenadaInicial.x, unidad.coordenadaInicial.y);
            if (registrada)
                Debug.Log($"Unidad '{unidad.datosDeClase?.nombreClase}' registrada exitosamente en [{unidad.coordenadaInicial.x},{unidad.coordenadaInicial.y}].");
        }
    }

    void Update()
    {
        if (estadoActual == EstadoJuego.AnimandoAccion || estadoActual == EstadoJuego.FinDePartida) return;
        DetectarInput();
    }

    private void DetectarInput()
    {
        Vector3 posicionInput = Vector3.zero;
        bool inputDetectado = false;

        // --- LÓGICA DEL NUEVO INPUT SYSTEM ---

        // 1. Detección Táctil (Móviles)
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var toque = Touchscreen.current.touches[0];
            if (toque.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(toque.touchId.ReadValue())) return;
                posicionInput = toque.position.ReadValue();
                inputDetectado = true;
            }
        }
        // 2. Detección de Ratón (PC/Editor)
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            posicionInput = Mouse.current.position.ReadValue();
            inputDetectado = true;
        }

        // -------------------------------------

        if (inputDetectado)
        {
            Vector2Int coordenadaTocada = ConvertirToqueACoordenada(posicionInput);
            if (coordenadaTocada.x != -1) ProcesarToque(coordenadaTocada);
        }
    }

    private Vector2Int ConvertirToqueACoordenada(Vector3 posicionPantalla)
    {
        Ray rayo = camaraPrincipal.ScreenPointToRay(posicionPantalla);
        if (Physics.Raycast(rayo, out RaycastHit impacto))
        {
            Debug.Log($"El ratón golpeó físicamente a: {impacto.collider.gameObject.name}");

            // 1. ¿Le dimos directo al Collider del gato?
            UnidadBase gatoTocado = impacto.collider.GetComponentInParent<UnidadBase>();
            if (gatoTocado != null)
            {
                return gatoTocado.coordenadaInicial;
            }

            // 2. ¿Le dimos al suelo? (A prueba de fallos)
            // Si golpeamos un objeto que en su nombre dice "Casilla", tomamos SU posición exacta.
            if (impacto.collider.gameObject.name.StartsWith("Casilla"))
            {
                // Como las casillas están en las coordenadas exactas, solo leemos su Transform
                int xExacta = Mathf.RoundToInt(impacto.collider.transform.localPosition.x / tamanoCelda);
                int yExacta = Mathf.RoundToInt(impacto.collider.transform.localPosition.z / tamanoCelda);

                return new Vector2Int(xExacta, yExacta);
            }
        }
        return new Vector2Int(-1, -1);
    }

    private void ProcesarToque(Vector2Int coordenada)
    {
        if (faseActual == FaseTurno.EsperandoSeleccion)
        {
            UnidadBase unidadTocada = tablero.ObtenerUnidadEn(coordenada.x, coordenada.y);

            if (unidadTocada != null)
            {
                if (unidadTocada.idEquipo == jugadorActual)
                {
                    unidadSeleccionada = unidadTocada;
                    coordenadaOrigen = coordenada;
                    faseActual = FaseTurno.UnidadSeleccionada;
                    Debug.Log($"<color=green>Seleccionaste a:</color> {unidadTocada.datosDeClase.nombreClase} en [{coordenada.x}, {coordenada.y}]");

                    // --- NUEVO: Mostramos el resaltado ---
                    MostrarResaltadoDeMovimiento(unidadTocada, coordenada.x, coordenada.y);
                }
                else
                {
                    Debug.LogWarning("Ese gato pertenece al enemigo.");
                }
            }
        }
        else if (faseActual == FaseTurno.UnidadSeleccionada)
        {
            // Si tocas el mismo gato (o tocas otra cosa para cancelar)
            if (coordenada == coordenadaOrigen)
            {
                unidadSeleccionada = null;
                faseActual = FaseTurno.EsperandoSeleccion;
                LimpiarResaltado(); // --- NUEVO: Limpiamos pantalla ---
                Debug.Log("Gato deseleccionado.");
                return;
            }

            List<Vector2Int> ruta = tablero.IntentarMoverUnidad(coordenadaOrigen.x, coordenadaOrigen.y, coordenada.x, coordenada.y);
            if (ruta != null && ruta.Count > 0)
            {
                estadoActual = EstadoJuego.AnimandoAccion;
                LimpiarResaltado(); // --- NUEVO: Limpiamos pantalla porque ya se va a mover ---

                ControladorUnidadVisual visual = unidadSeleccionada.GetComponent<ControladorUnidadVisual>();
                unidadSeleccionada.coordenadaInicial = coordenada;

                if (visual != null)
                {
                    visual.IniciarMovimiento(ruta);
                    StartCoroutine(EsperarMovimientoYCambiarTurno(visual));
                }
                else CambiarTurno();
                unidadSeleccionada = null;
            }
            else Debug.LogWarning("Movimiento inválido o fuera de rango.");
        }
    }

    // --- MÉTODOS NUEVOS PARA DIBUJAR/BORRAR ---
    private void MostrarResaltadoDeMovimiento(UnidadBase unidad, int origenX, int origenY)
    {
        if (prefabResaltado == null) return;

        List<Vector2Int> validas = tablero.ObtenerMovimientosValidos(unidad, origenX, origenY);

        foreach (Vector2Int pos in validas)
        {
            // Instanciamos el cuadrito ligeramente por encima del suelo (Y: 0.05) para que no se superponga (Z-Fighting)
            Vector3 posicionMundo = new Vector3(pos.x * tamanoCelda, 0.05f, pos.y * tamanoCelda);
            GameObject resaltado = Instantiate(prefabResaltado, posicionMundo, prefabResaltado.transform.rotation);
            casillasResaltadasInstanciadas.Add(resaltado);
        }
    }

    private void LimpiarResaltado()
    {
        foreach (GameObject resaltado in casillasResaltadasInstanciadas)
        {
            Destroy(resaltado);
        }
        casillasResaltadasInstanciadas.Clear();
    }

    private IEnumerator EsperarMovimientoYCambiarTurno(ControladorUnidadVisual visual)
    {
        yield return null;
        while (visual.SeEstaMoviendo) yield return null;
        CambiarTurno();
    }

    public void CambiarTurno()
    {
        jugadorActual = (jugadorActual == 1) ? 2 : 1;
        estadoActual = (jugadorActual == 1) ? EstadoJuego.TurnoJugador1 : EstadoJuego.TurnoJugador2;
        faseActual = FaseTurno.EsperandoSeleccion;
        Debug.Log($"<color=cyan>--- CAMBIO DE TURNO: Jugador {jugadorActual} ---</color>");
    }
}