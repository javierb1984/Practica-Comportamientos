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

    void Start()
    {
        puestoMaitre = mundo.puestoMaitre;
        transform.position = puestoMaitre;
        estadoActual = EstadosFSM.ESPERAR;
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
                    mesaActual = mundo.nextMesa();
                    clienteActual = mundo.popClienteCola();
                    clienteActual.concedeMesa(mesaActual);
                    estadoActual = EstadosFSM.LLEVAR_CLIENTE;
                }
            break;

            case EstadosFSM.LLEVAR_CLIENTE:
                Vector3 position = mundo.mesas[mesaActual].transform.GetChild(0).position;
                walkTo(position);

                if(isInPosition(position))
                    estadoActual = EstadosFSM.SENTAR;
            break;

            case EstadosFSM.SENTAR:
                clienteActual.sentar();
                estadoActual = EstadosFSM.VOLVER;
            break;

            case EstadosFSM.VOLVER:
                walkTo(puestoMaitre);

                if(isInPosition(puestoMaitre))
                    estadoActual = EstadosFSM.ESPERAR;
            break;
        }
    }
}
