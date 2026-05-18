using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TableroLogico : MonoBehaviour
{
    [Header("Dimensiones (Orientación Vertical)")]
    public int columnas = 6; // Eje X (Ancho)
    public int filas = 10;   // Eje Y (Alto)

    // La matriz bidimensional que guardará la referencia de quién ocupa cada casilla
    private UnidadBase[,] cuadricula;

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

    void Awake()
    {
        // Al instanciar la clase, creamos la matriz vacía
        cuadricula = new UnidadBase[columnas, filas];
        LogDev($"Matriz del tablero generada: {columnas}x{filas}");
    }

    // Valida si una coordenada matemática (x,y) existe dentro de los límites
    public bool EsCoordenadaValida(int x, int y)
    {
        return x >= 0 && x < columnas && y >= 0 && y < filas;
    }

    // Registra una unidad felina en una coordenada específica
    public bool RegistrarUnidad(UnidadBase unidad, int x, int y)
    {
        // Verificamos que la casilla exista y esté vacía
        if (EsCoordenadaValida(x, y) && cuadricula[x, y] == null)
        {
            cuadricula[x, y] = unidad;
            // Aquí en el futuro le diremos a la Vista que mueva el modelo 3D
            return true;
        }
        LogWarningDev($"No se pudo registrar la unidad en [{x},{y}]: la casilla no existe o está ocupada.");
        return false;
    }

    // Devuelve qué unidad está en cierta casilla (útil para el sistema de combate)
    public UnidadBase ObtenerUnidadEn(int x, int y)
    {
        if (EsCoordenadaValida(x, y))
        {
            return cuadricula[x, y];
        }
        return null;
    }

    // Libera una casilla (ej. cuando una unidad se mueve o es derrotada)
    public void VaciarCasilla(int x, int y)
    {
        if (EsCoordenadaValida(x, y))
        {
            cuadricula[x, y] = null;
        }
    }
    // Evalúa si la geometría del movimiento está permitida según las estadísticas de la clase
    public bool ValidarRangoMovimiento(int origenX, int origenY, int destinoX, int destinoY, ClaseBaseSO estadisticas)
    {
        // 1. Calculamos cuántas casillas hay de diferencia en cada eje
        int distanciaX = Mathf.Abs(destinoX - origenX);
        int distanciaY = Mathf.Abs(destinoY - origenY);

        // 2. Comprobamos si es un movimiento puramente ORTOGONAL (en forma de cruz: arriba, abajo, izquierda, derecha)
        // Esto ocurre cuando una de las dos distancias es 0.
        if (distanciaX == 0 || distanciaY == 0)
        {
            int distanciaTotal = distanciaX + distanciaY; // La distancia real es la suma
            if (distanciaTotal <= estadisticas.movimientoOrtogonal)
            {
                return true; // Movimiento ortogonal válido
            }
        }
        // 3. Comprobamos si es un movimiento puramente DIAGONAL (en forma de X)
        // Esto ocurre cuando avanzas la misma cantidad de casillas en X y en Y.
        else if (distanciaX == distanciaY)
        {
            if (distanciaX <= estadisticas.movimientoDiagonal)
            {
                return true; // Movimiento diagonal válido
            }
        }
        // 4. (Opcional) Si quieres permitir movimientos "híbridos" como el de un Caballo en ajedrez, 
        // se programaría aquí. Por ahora, nos apegamos a la regla estricta de tu documento.

        return false; // Si no cumplió ninguna regla, el movimiento es inválido
    }
    // Intenta mover una unidad de una casilla a otra
    public bool IntentarMoverUnidad(int origenX, int origenY, int destinoX, int destinoY)
    {
        // 0. Validar que ambas coordenadas están dentro del tablero antes de cualquier cálculo
        if (!EsCoordenadaValida(origenX, origenY) || !EsCoordenadaValida(destinoX, destinoY))
        {
            LogWarningDev($"Coordenadas fuera del tablero: origen [{origenX},{origenY}] o destino [{destinoX},{destinoY}].");
            return false;
        }

        UnidadBase unidad = cuadricula[origenX, origenY];

        if (unidad == null) return false;

        // 1. Validamos matemáticamente primero (es rápido y descarta clics erróneos de inmediato)
        bool enRangoMatematico = ValidarRangoMovimiento(origenX, origenY, destinoX, destinoY, unidad.datosDeClase);

        if (!enRangoMatematico)
        {
            LogWarningDev($"Movimiento inválido para {unidad.datosDeClase.nombreClase}. Fuera de sus reglas de diseño.");
            return false;
        }

        // 2b. Verificar que el destino no esté ocupado antes de llamar al pathfinding
        if (cuadricula[destinoX, destinoY] != null)
        {
            LogWarningDev($"La casilla destino [{destinoX},{destinoY}] ya está ocupada.");
            return false;
        }

        // 3. Determinar el tipo de movimiento para que el pathfinding use las direcciones correctas
        int distanciaX = Mathf.Abs(destinoX - origenX);
        int distanciaY = Mathf.Abs(destinoY - origenY);
        TipoMovimiento tipoMovimiento = (distanciaX == 0 || distanciaY == 0)
            ? TipoMovimiento.Ortogonal
            : TipoMovimiento.Diagonal;

        // 4. Ruta optimizada para reglas actuales: línea recta ortogonal o diagonal.
        Vector2Int inicio = new Vector2Int(origenX, origenY);
        Vector2Int destino = new Vector2Int(destinoX, destinoY);
        List<Vector2Int> ruta = ConstruirRutaLinealSiLibre(inicio, destino, tipoMovimiento);
        if (ruta == null)
        {
            // Fallback para reglas futuras más complejas.
            ruta = BuscadorRutas.EncontrarCamino(this, inicio, destino, unidad.datosDeClase, tipoMovimiento);
        }

        // Si la ruta no es nula y tiene pasos, el camino está totalmente despejado
        if (ruta != null && ruta.Count > 0)
        {
            // Efectuamos el movimiento en la memoria del tablero
            cuadricula[destinoX, destinoY] = unidad;
            cuadricula[origenX, origenY] = null;

            LogDev($"{unidad.datosDeClase.nombreClase} se movió lógicamente a [{destinoX},{destinoY}]");

            // TODO: En el siguiente paso arquitectónico, aquí pasaremos la variable 'ruta' 
            // a ControladorUnidadVisual.IniciarMovimiento() para que inicie la corrutina de animación.
            return true;
        }

        LogWarningDev($"La ruta hacia [{destinoX},{destinoY}] está bloqueada por obstáculos intermedios para {unidad.datosDeClase.nombreClase}.");
        return false;
    }

    private List<Vector2Int> ConstruirRutaLinealSiLibre(Vector2Int inicio, Vector2Int destino, TipoMovimiento tipoMovimiento)
    {
        int deltaX = destino.x - inicio.x;
        int deltaY = destino.y - inicio.y;

        int pasos = Mathf.Max(Mathf.Abs(deltaX), Mathf.Abs(deltaY));
        if (pasos <= 0) return null;

        int pasoX = CalcularPaso(deltaX, tipoMovimiento == TipoMovimiento.Ortogonal);
        int pasoY = CalcularPaso(deltaY, tipoMovimiento == TipoMovimiento.Ortogonal);

        var ruta = new List<Vector2Int>(pasos);
        Vector2Int actual = inicio;

        for (int i = 0; i < pasos; i++)
        {
            actual = new Vector2Int(actual.x + pasoX, actual.y + pasoY);

            if (!EsCoordenadaValida(actual.x, actual.y))
                return null;

            if (actual != destino && cuadricula[actual.x, actual.y] != null)
                return null;

            ruta.Add(actual);
        }

        return ruta;
    }

    public static int CalcularPaso(int delta, bool permitirCero)
    {
        if (permitirCero && delta == 0) return 0;
        return delta > 0 ? 1 : -1;
    }
} 
