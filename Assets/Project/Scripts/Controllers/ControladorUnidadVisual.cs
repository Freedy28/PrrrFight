using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorUnidadVisual : MonoBehaviour
{
    [Header("Ajustes de Animación")]
    public float velocidadMovimiento = 5f; // Qué tan rápido camina el gato
    public float tamanoCelda = 1f;         // Escala para convertir coordenadas lógicas a 3D

    // Bandera para evitar que reciba nuevas órdenes mientras camina
    public bool SeEstaMoviendo { get; private set; } = false;

    // Este es el método público que llamaremos cuando el jugador toque una casilla
    public void IniciarMovimiento(List<Vector2Int> ruta)
    {
        if (!SeEstaMoviendo && ruta != null && ruta.Count > 0)
        {
            StartCoroutine(RutinaCaminarPorRuta(ruta));
        }
    }

    // La magia visual ocurre aquí
    // La magia visual ocurre aquí
    // La magia visual ocurre aquí
    private IEnumerator RutinaCaminarPorRuta(List<Vector2Int> ruta)
    {
        SeEstaMoviendo = true;

        foreach (Vector2Int paso in ruta)
        {
            // 1. Buscamos la casilla física en la escena por su nombre (ej. "Casilla_1_2")
            GameObject casillaFisica = GameObject.Find($"Casilla_{paso.x}_{paso.y}");
            Vector3 posicionDestino;

            if (casillaFisica != null)
            {
                // Tomamos la posición X y Z exactas de la casilla en el mundo.
                // MANTENEMOS la Y actual del personaje (transform.position.y) para que no se hunda.
                posicionDestino = new Vector3(casillaFisica.transform.position.x, transform.position.y, casillaFisica.transform.position.z);
            }
            else
            {
                // Respaldo matemático por si la casilla no existe
                posicionDestino = new Vector3(paso.x * tamanoCelda, transform.position.y, paso.y * tamanoCelda);
            }

            // 2. Girar al personaje (Arreglado para que no se incline hacia el piso si las alturas varían)
            transform.LookAt(new Vector3(posicionDestino.x, transform.position.y, posicionDestino.z));

            // 3. Moverse suavemente hacia esa coordenada
            while (Vector3.Distance(transform.position, posicionDestino) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    posicionDestino,
                    velocidadMovimiento * Time.deltaTime
                );
                yield return null;
            }

            // 4. Forzamos la posición exacta al llegar
            transform.position = posicionDestino;
        }

        SeEstaMoviendo = false;
        Debug.Log("El personaje ha llegado a su destino.");
    }
}