using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cocinero : Gato
{

    private enum EstadosFSM { PATRULLAR, COCINAR, LLEVAR_COMIDA, ECHAR_LADRON};
    private EstadosFSM estadoActual;
    private Vector3 posMesaPedidos;
    private Plato platoActual;
    private float timer;
    private Catco referenciaCatco;
    

    // Start is called before the first frame update
    void Start()
    {
        posMesaPedidos = mundo.mesaPedidos.transform.position;
        this.estadoActual = EstadosFSM.PATRULLAR;
    }

    // Update is called once per frame
    void Update()
    {
        FSM();
    }

    public void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM.PATRULLAR:
                //Camina por la cocina
                if (mundo.hayComandas())
                {
                    walkTo(mundo.muroComandas);
                    platoActual = mundo.takeComanda();
                    mundo.setPlato(platoActual.comida, platoActual.mesa, platoActual.plato);
                    this.timer = Random.Range(5f, 20f);
                    estadoActual = EstadosFSM.COCINAR;
                }

                
                break;

            case EstadosFSM.COCINAR:
                //animaciones de cocinar y andar para dar posibilidad de robar
                timer -= Time.deltaTime;
                //Si acabade cocinar
                if(timer <= 0)
                {
                    pick(mundo.getPlato().plato);
                    estadoActual = EstadosFSM.LLEVAR_COMIDA;
                }
                //Si ya han robado el plato lo vuelve a empezar
                if (!mundo.hayPlato())
                {
                    mundo.setPlato(platoActual.comida, platoActual.mesa, platoActual.plato);
                    this.timer = Random.Range(5f, 20f);
                }
                //If con percepción del ladron en el camino
                if(false)
                {
                    angry(referenciaCatco.transform.position);
                    referenciaCatco.Pillado();
                }
                


                break;

            case EstadosFSM.LLEVAR_COMIDA:
                walkTo(posMesaPedidos);
                if (isInPosition(posMesaPedidos))
                {
                    //dejar el plato
                    mundo.pushPlato(platoActual);
                    platoActual = null;
                }
                    
                break;
        }
    }
}
