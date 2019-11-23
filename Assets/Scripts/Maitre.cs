using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maitre : Gato
{
    private enum EstadosFSM { ESPERAR, LLEVAR_CLIENTE, SENTAR, VOLVER }
    private EstadosFSM estadoActual;

    //Habrá que cambiar el GameObject por Cliente
    private Cliente clienteActual;

    //Número de mesa del cliente actual
    private int mesaActual;

    //Referencias a posiciones
    private Vector3 puestoMaitre;

    //El cliente se ha sentado
    private bool clienteSentado;

    //Esperas necesarias para el maitre
    private float timer;
    private float waitingTime = 1f;

    private Vector3 lookAt;

    void Start()
    {
        puestoMaitre = mundo.puestoMaitre;
        transform.position = puestoMaitre;
        clienteSentado = false;
        estadoActual = EstadosFSM.ESPERAR;
        timer = waitingTime;
        lookAt = mundo.mesaMaitre.transform.position;
        wait();
    }

    void Update()
    {
        FSM();
    }

    void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM.ESPERAR:
                rotateTowards(lookAt);
                if (!mundo.colaIsEmpty() && !mundo.mesasIsFull())
                {
                    //Espera 1 segundos antes de llevar al cliente a la mesa

                    timer -= Time.deltaTime;
                    if (timer <= 0 && isLookingTowards(lookAt))
                    {
                        mesaActual = mundo.nextMesa();
                        clienteActual = mundo.popClienteCola();
                        clienteActual.concedeMesa(mesaActual);

                        //Camina hasta la siguiente mesa
                        Vector3 position = mundo.mesas[mesaActual].transform.parent.GetChild(1).position;
                        walkTo(position);

                        estadoActual = EstadosFSM.LLEVAR_CLIENTE;
                    }
                }
            break;

            case EstadosFSM.LLEVAR_CLIENTE:
                if (isInPosition())
                {
                    wait();
                    timer = waitingTime;
                    estadoActual = EstadosFSM.SENTAR;
                }
            break;

            case EstadosFSM.SENTAR:
                clienteActual.sentar();
                if (clienteSentado)
                {
                    //Espera un poco antes de volver
                    timer -= Time.deltaTime;

                    if (timer <= 0)
                    {
                        clienteSentado = false;
                        walkTo(puestoMaitre);
                        estadoActual = EstadosFSM.VOLVER;
                    }
                }
            break;

            case EstadosFSM.VOLVER:

                if (isInPosition())
                {
                    timer = waitingTime;
                    idle();
                    estadoActual = EstadosFSM.ESPERAR;
                }
            break;
        }
    }

    public void Sentado()
    {
        clienteSentado = true;
    }
}
