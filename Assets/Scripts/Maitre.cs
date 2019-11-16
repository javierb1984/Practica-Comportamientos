using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maitre : Gato
{
    private enum EstadosFSM { ESPERAR, LLEVAR_CLIENTE, SENTAR, VOLVER }
    private EstadosFSM estadoActual;

    //Habrá que cambiar el GameObject por Cliente
    private GameObject clienteActual;

    //Número de mesa del cliente actual
    private int mesaActual;

    //Referencias a posiciones
    private Vector3 puestoMaitre;

    void Start()
    {
        puestoMaitre = mundo.puestoMaitre;
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
                    estadoActual = EstadosFSM.LLEVAR_CLIENTE;
            break;

            case EstadosFSM.LLEVAR_CLIENTE:
                clienteActual = mundo.popClienteCola();
                mesaActual = mundo.nextMesa();
                //Llamada a función del cliente para que deje de esperar y le siga
                walkTo(mundo.mesas[mesaActual].transform.position);
                estadoActual = EstadosFSM.SENTAR;
            break;

            case EstadosFSM.SENTAR:
                //Llamada al cliente para que se siente
                estadoActual = EstadosFSM.VOLVER;
            break;

            case EstadosFSM.VOLVER:
                walkTo(puestoMaitre);
                estadoActual = EstadosFSM.ESPERAR;
            break;
        }
    }
}
