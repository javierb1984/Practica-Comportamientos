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

    void Start()
    {
        puestoMaitre = mundo.puestoMaitre;
        transform.position = puestoMaitre;
        clienteSentado = false;
        estadoActual = EstadosFSM.ESPERAR;
        timer = waitingTime;
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

                if (!mundo.colaIsEmpty() && !mundo.mesasIsFull())
                {
                    //Espera 2 segundos antes de llevar al cliente a la mesa
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        mesaActual = mundo.nextMesa();
                        clienteActual = mundo.popClienteCola();
                        clienteActual.concedeMesa(mesaActual);
                        estadoActual = EstadosFSM.LLEVAR_CLIENTE;
                    }
                }
            break;

            case EstadosFSM.LLEVAR_CLIENTE:
                Vector3 position = mundo.mesas[mesaActual].transform.GetChild(0).position;
                walkTo(position);

                if (isInPosition(position))
                {
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

                    //if(timer <= 0)
                        estadoActual = EstadosFSM.VOLVER;
                }
            break;

            case EstadosFSM.VOLVER:
                clienteSentado = false;
                walkTo(puestoMaitre);

                if (isInPosition(puestoMaitre))
                {
                    timer = waitingTime;
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
