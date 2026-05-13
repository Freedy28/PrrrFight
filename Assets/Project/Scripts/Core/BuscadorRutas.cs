using System.Collections.Generic;
using UnityEngine;

public static class BuscadorRutas
{
    // Separamos las direcciones para poder evaluarlas según la clase del gato
    private static readonly Vector2Int[] dirOrtogonales = {
        new Vector2Int(0, 1), new Vector2Int(1, 0),
        new Vector2Int(0, -1), new Vector2Int(-1, 0)
    };

    private static readonly Vector2Int[] dirDiagonales = {
        new Vector2Int(1, 1), new Vector2Int(1, -1),
        new Vector2Int(-1, -1), new Vector2Int(-1, 1)
    };

    // Actualizamos la firma para recibir las estadísticas de la unidad
    public static List<Vector2Int> EncontrarCamino(TableroLogico tablero, Vector2Int inicio, Vector2Int destino, ClaseBaseSO estadisticas)
    {
        if (!tablero.EsCoordenadaValida(inicio.x, inicio.y) ||
            !tablero.EsCoordenadaValida(destino.x, destino.y) ||
            tablero.ObtenerUnidadEn(destino.x, destino.y) != null)
        {
            return null;
        }

        // 1. Configurar las direcciones permitidas para esta clase específica
        List<Vector2Int> direccionesPermitidas = new List<Vector2Int>();
        if (estadisticas.movimientoOrtogonal > 0) direccionesPermitidas.AddRange(dirOrtogonales);
        if (estadisticas.movimientoDiagonal > 0) direccionesPermitidas.AddRange(dirDiagonales);

        // 2. Definir la distancia máxima que el BFS tiene permitido explorar
        int maxPasos = Mathf.Max(estadisticas.movimientoOrtogonal, estadisticas.movimientoDiagonal);

        Queue<Vector2Int> frontera = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> proveniencia = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costoCamino = new Dictionary<Vector2Int, int>(); // Nuevo: Control de rango

        frontera.Enqueue(inicio);
        proveniencia[inicio] = inicio;
        costoCamino[inicio] = 0;

        bool destinoAlcanzado = false;

        while (frontera.Count > 0)
        {
            Vector2Int actual = frontera.Dequeue();

            if (actual == destino)
            {
                destinoAlcanzado = true;
                break;
            }

            // Si llegamos al límite de pasos de la unidad en esta ruta, no exploramos más allá
            if (costoCamino[actual] >= maxPasos) continue;

            foreach (Vector2Int direccion in direccionesPermitidas)
            {
                Vector2Int vecino = actual + direccion;

                if (tablero.EsCoordenadaValida(vecino.x, vecino.y) && !proveniencia.ContainsKey(vecino))
                {
                    if (tablero.ObtenerUnidadEn(vecino.x, vecino.y) == null || vecino == destino)
                    {
                        frontera.Enqueue(vecino);
                        proveniencia[vecino] = actual;
                        costoCamino[vecino] = costoCamino[actual] + 1; // Aumentamos el costo de pasos
                    }
                }
            }
        }

        if (!destinoAlcanzado) return null;

        return ReconstruirRuta(proveniencia, inicio, destino);
    }

    private static List<Vector2Int> ReconstruirRuta(Dictionary<Vector2Int, Vector2Int> proveniencia, Vector2Int inicio, Vector2Int destino)
    {
        List<Vector2Int> ruta = new List<Vector2Int>();
        Vector2Int actual = destino;

        while (actual != inicio)
        {
            ruta.Add(actual);
            actual = proveniencia[actual];
        }

        ruta.Reverse();
        return ruta;
    }
}