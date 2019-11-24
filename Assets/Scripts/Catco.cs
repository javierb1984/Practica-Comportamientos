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
    private Plato comida;
    private GameObject platoComiendo;

    //Timers de comer
    private float timer;

    void Start()
    {
        //Posicion de spawn
        puestoCaco = mundo.puestoCaco;
        transform.position = puestoCaco;

        posPlato = mundo.posCocina;
        comida = null;
        puertaTrasera = mundo.puertaTrasera;
        idle();
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
                {
                    walkTo(puertaTrasera);
                    estadoActual = EstadosFSM.ENTRAR;
                }
                break;

            case EstadosFSM.ENTRAR:
                if (isInPosition())
                {
                    //If con percepción del cocinero en el camino
                    Plato plato = mundo.pollPlato();
                    walkTo(posPlato);
                    estadoActual = EstadosFSM.IR_PLATO;
                }
                break;

            case EstadosFSM.IR_PLATO:
                if (pillado || !mundo.hayPlato())
                {
                    estadoActual = EstadosFSM.VOLVER;
                }
                else if(isInPosition())
                {
                    Plato plato = mundo.pollPlato();
                    estadoActual = EstadosFSM.COGER_PLATO;
                }
                break;

            case EstadosFSM.COGER_PLATO:
                comida = mundo.getPlato();                
                pick(comida.plato);
                runTo(puestoCaco);
                estadoActual = EstadosFSM.VOLVER;
                break;

            case EstadosFSM.VOLVER:
                if (isInPosition())
                {
                    if (comida != null)
                    {
                        timer = 10;
                        platoComiendo = Instantiate(mundo.plato, transform.position + transform.forward * 0.5f, mundo.plato.transform.rotation);
                        eat();
                        estadoActual = EstadosFSM.COMER;
                    }
                    else
                        estadoActual = EstadosFSM.ESPERAR;
                    pillado = false;
                }
                break;

            case EstadosFSM.COMER:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Destroy(platoComiendo);
                    comida = null;
                    idle();
                    estadoActual = EstadosFSM.ESPERAR;
                }
                break;
        }
    }

    public void Pillado()
    {
        pillado = true;
    }
}
