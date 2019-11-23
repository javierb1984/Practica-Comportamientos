using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cocinero : Gato
{

    private enum EstadosFSM { ESPERAR, COCINAR, LLEVAR_COMIDA, ECHAR_LADRON};
    private enum CocinarFSM { COCINAR, PENSAR, VAGAR};
    private EstadosFSM estadoActual;
    private CocinarFSM estadoCocinar;
    private Vector3 posMesaPedidos;
    private Plato platoActual;
    private float timer;
    private float cocinaTimer;
    

    //Para caminar por la cocina
    Vector3 posVagar;
    Vector3 min;
    Vector3 max;


    // Start is called before the first frame update
    void Start()
    {
        posMesaPedidos = mundo.mesaPedidos.transform.position;
        transform.position = mundo.puestoCocinero;
        this.min = mundo.minCocinero;
        this.max = mundo.maxCocinero;
        this.estadoActual = EstadosFSM.ESPERAR;
        this.estadoCocinar = CocinarFSM.PENSAR;
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
            case EstadosFSM.ESPERAR:
                if (mundo.hayComandas())
                {
                    walkTo(mundo.muroComandas);
                    if (isInPosition())
                    {
                        platoActual = mundo.takeComanda();
                        mundo.setPlato(platoActual.comida, platoActual.mesa, platoActual.plato, platoActual.cliente);
                        timer = Random.Range(10f, 20f);
                        estadoActual = EstadosFSM.COCINAR;
                    }
                }
                break;

            case EstadosFSM.COCINAR:

                //Si acabade cocinar
                if (timer <= 0)
                {
                    pick(mundo.getPlato().plato);
                    walkTo(posMesaPedidos);
                    estadoActual = EstadosFSM.LLEVAR_COMIDA;
                    estadoCocinar = CocinarFSM.PENSAR;
                }
                else
                {
                    timer -= Time.deltaTime;
                    FSM_Cocinar();
                }


                //Si ya han robado el plato lo vuelve a empezar
                if (!mundo.hayPlato())
                {
                    mundo.setPlato(platoActual.comida, platoActual.mesa, platoActual.plato, platoActual.cliente);
                    timer = Random.Range(10f, 20f);
                }
                //If con percepción del ladron en el camino
                if (true)
                {
                    angry();
                    //referenciaCatco.Pillado();
                    timer = Random.Range(2f, 3f);
                    estadoActual = EstadosFSM.ECHAR_LADRON;
                    
                }
                break;

            case EstadosFSM.ECHAR_LADRON:
                timer -= Time.deltaTime;

                if(timer <= 0)
                {
                    timer = Random.Range(10f, 20f);
                    estadoActual = EstadosFSM.COCINAR;
                }
                
                break;

            case EstadosFSM.LLEVAR_COMIDA:
                
                if (isInPosition())
                {
                    //dejar el plato
                    mundo.pushPlato(platoActual);
                    platoActual = null;
                    estadoActual = EstadosFSM.ESPERAR;
                }
                    
                break;

        }
    }

    public void FSM_Cocinar()
    {

        switch (estadoCocinar)
        {
            case CocinarFSM.COCINAR:
                if (isInPosition())
                    cook();
                break;

            case CocinarFSM.PENSAR:
                if(timer <= 7)
                {
                    walkTo(mundo.posCocina);
                    estadoCocinar = CocinarFSM.COCINAR;
                }
                else
                {
                    posVagar = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

                    estadoCocinar = CocinarFSM.VAGAR;
                }
                
                break;

            case CocinarFSM.VAGAR:
                walkTo(posVagar);
                if (isInPosition())
                    estadoCocinar = CocinarFSM.PENSAR;
                break;
        }
    }
}
