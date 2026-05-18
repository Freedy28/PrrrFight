using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ControladorUnidadVisual : MonoBehaviour
{
    [Header("Ajustes de Animación")]
    public float velocidadMovimiento = 5f; // Qué tan rápido camina el gato
    public float tamanoCelda = 1f;         // Escala para convertir coordenadas lógicas a 3D

    // Bandera para evitar que reciba nuevas órdenes mientras camina
    public bool SeEstaMoviendo { get; private set; } = false;
    public event Action MovimientoFinalizado;

    // Este es el método público que llamaremos cuando el jugador toque una casilla
    public void IniciarMovimiento(List<Vector2Int> ruta)
    {
        if (!SeEstaMoviendo && ruta != null && ruta.Count > 0)
        {
            StartCoroutine(RutinaCaminarPorRuta(ruta));
        }
    }

    // La magia visual ocurre aquí
    private IEnumerator RutinaCaminarPorRuta(List<Vector2Int> ruta)
    {
        SeEstaMoviendo = true;
        const float umbralLlegadaSqr = 0.05f * 0.05f;

        // Recorremos cada casilla de la lista que nos dio el Pathfinding
        foreach (Vector2Int paso in ruta)
        {
            // 1. Traducir la coordenada matemática [X, Y] a una posición física en Unity (X, 0, Z)
            // Asumimos que Y es 0 porque se mueven sobre una superficie plana
            Vector3 posicionDestino = new Vector3(paso.x * tamanoCelda, 0, paso.y * tamanoCelda);

            // 2. Girar al personaje para que mire hacia donde camina (Opcional pero recomendado)
            transform.LookAt(posicionDestino);

            // 3. Moverse suavemente hacia esa coordenada
            while ((transform.position - posicionDestino).sqrMagnitude > umbralLlegadaSqr)
            {
                // MoveTowards calcula el pasito exacto que debe dar en este frame
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    posicionDestino,
                    velocidadMovimiento * Time.deltaTime
                );

                // yield return null le dice a Unity: "Pausa aquí, dibuja el frame en pantalla, y continuamos en el siguiente frame"
                yield return null;
            }

            // 4. Forzamos la posición para evitar errores de punto flotante al llegar
            transform.position = posicionDestino;
        }

        SeEstaMoviendo = false;
        RegistrarLogMovimientoFinalizado();
        MovimientoFinalizado?.Invoke();

        // ¡Aquí es donde le avisaremos al juego que el turno del movimiento terminó!
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    private static void RegistrarLogMovimientoFinalizado()
    {
        UnityEngine.Debug.Log("El personaje ha llegado a su destino.");
    }
}
