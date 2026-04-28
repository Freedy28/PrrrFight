using System.Collections.Generic;
using UnityEngine;

public static class BuscadorRutas
{
    private static readonly Vector2Int[] direcciones = {
        new Vector2Int(0, 1), new Vector2Int(1, 0),
        new Vector2Int(0, -1), new Vector2Int(-1, 0)
    };

    public static List<Vector2Int> EncontrarCamino(TableroLogico tablero, Vector2Int inicio, Vector2Int destino)
    {
        if (!tablero.EsCoordenadaValida(destino.x, destino.y) || tablero.ObtenerUnidadEn(destino.x, destino.y) != null)
        {
            return null; 
        }

        Queue<Vector2Int> frontera = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> proveniencia = new Dictionary<Vector2Int, Vector2Int>();

        frontera.Enqueue(inicio);
        proveniencia[inicio] = inicio; 

        bool destinoAlcanzado = false;

        while (frontera.Count > 0)
        {
            Vector2Int actual = frontera.Dequeue();

            if (actual == destino)
            {
                destinoAlcanzado = true;
                break;
            }

            foreach (Vector2Int direccion in direcciones)
            {
                Vector2Int vecino = actual + direccion;

                if (tablero.EsCoordenadaValida(vecino.x, vecino.y) && !proveniencia.ContainsKey(vecino))
                if (tablero.EsCoordenadaValida(vecino.x, vecino.y) && !proveniencia.ContainsKey(vecino))
                {
                    
                    if (tablero.ObtenerUnidadEn(vecino.x, vecino.y) == null || vecino == destino)
                    {
                        frontera.Enqueue(vecino);
                        proveniencia[vecino] = actual; 
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

        ruta.Reverse(); // Invertimos la lista para que vaya del inicio al destino
        return ruta;
    }
}