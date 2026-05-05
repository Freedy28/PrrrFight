using UnityEngine;

public class TableroLogico : MonoBehaviour
{
    [Header("Dimensiones (Orientación Vertical)")]
    public int columnas = 6; // Eje X (Ancho)
    public int filas = 10;   // Eje Y (Alto)

    // La matriz bidimensional que guardará la referencia de quién ocupa cada casilla
    private UnidadBase[,] cuadricula;

    void Awake()
    {
        // Al instanciar la clase, creamos la matriz vacía
        cuadricula = new UnidadBase[columnas, filas];
        Debug.Log($"Matriz del tablero generada: {columnas}x{filas}");
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
        Debug.LogWarning($"No se pudo registrar la unidad en [{x},{y}]: la casilla no existe o está ocupada.");
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
        UnidadBase unidad = ObtenerUnidadEn(origenX, origenY);

        // Si no hay unidad en el origen, fallamos
        if (unidad == null) return false;

        // Validamos el rango usando nuestro nuevo método matemático
        bool enRango = ValidarRangoMovimiento(origenX, origenY, destinoX, destinoY, unidad.datosDeClase);

        if (enRango && EsCoordenadaValida(destinoX, destinoY) && cuadricula[destinoX, destinoY] == null)
        {
            // Efectuamos el movimiento lógico
            cuadricula[destinoX, destinoY] = unidad;
            cuadricula[origenX, origenY] = null;

            Debug.Log($"{unidad.datosDeClase.nombreClase} se movió a [{destinoX},{destinoY}]");
            return true;
        }

        Debug.LogWarning($"Movimiento inválido para {unidad.datosDeClase.nombreClase}. Fuera de rango o casilla ocupada.");
        return false;
    }
}