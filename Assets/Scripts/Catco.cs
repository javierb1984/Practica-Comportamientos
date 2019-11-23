using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catco : Gato
{
    private enum EstadosFSM { ESPERAR, ENTRAR, IR_PLATO, COGER_PLATO, VOLVER, COMER }
    private EstadosFSM estadoActual;
    private Vector3 puestoCaco;
    private Vector3 puertaTrasera;
    private Vector3 posPlato;
    private bool pillado;
    Plato comida; 

    void Start()
    {
        transform.position = puestoCaco;
        comida = null;
        puertaTrasera = mundo.puertaTrasera;
        puestoCaco = mundo.puestoCaco;
    }

    // Update is called once per frame

    void Update()
    {
        FSM();
    }

    void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM.ESPERAR:
                if (mundo.hayPlato())
                    estadoActual = EstadosFSM.ENTRAR;
                break;

            case EstadosFSM.ENTRAR:
                walkTo(puertaTrasera);

                if (isInPosition())
                {
                    //If con percepción del cocinero en el camino
                    Plato plato = mundo.pollPlato();
                    posPlato = plato.plato.transform.position;
                    estadoActual = EstadosFSM.IR_PLATO;
                }
                break;

            case EstadosFSM.IR_PLATO:
                if (pillado || !mundo.hayPlato())
                {
                    estadoActual = EstadosFSM.VOLVER;
                }
                else
                {
                    Plato plato = mundo.pollPlato();
                    walkTo(posPlato);

                    if(isInPosition())
                        estadoActual = EstadosFSM.COGER_PLATO;
                }
                break;

            case EstadosFSM.COGER_PLATO:
                comida = mundo.getPlato();
                pick(comida.plato);
                estadoActual = EstadosFSM.VOLVER;
                break;

            case EstadosFSM.VOLVER:
                runTo(puestoCaco);
                if (isInPosition())
                {
                    if (comida != null)
                        estadoActual = EstadosFSM.COMER;
                    else
                        estadoActual = EstadosFSM.ESPERAR;
                    pillado = false;
                }
                break;

            case EstadosFSM.COMER:
                //Faltan cosas
                comida = null;
                estadoActual = EstadosFSM.ESPERAR;
                break;
        }
    }

    public void Pillado()
    {
        pillado = true;
    }
}
