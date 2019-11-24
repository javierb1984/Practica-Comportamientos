using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cocinero : Gato
{

    private enum EstadosFSM { ESPERAR, IR_PUESTO, COCINAR, LLEVAR_COMIDA, ECHAR_LADRON};
    private enum CocinarFSM { COCINAR, PENSAR, VAGAR};
    private EstadosFSM estadoActual;
    private CocinarFSM estadoCocinar;
    private Vector3 posMesaPedidos;
    private GameObject puestoCocinar;
    private Vector3 posCocinero;
    private Plato platoActual;
    private GameObject plato;
    private float timer;
    private float cocinaTimer;
    private Catco catco;
    

    //Para caminar por la cocina
    Vector3 posVagar;
    Vector3 min;
    Vector3 max;


    // Start is called before the first frame update
    void Start()
    {
        posMesaPedidos = mundo.mesaPedidos.transform.GetChild(1).position;
        puestoCocinar = mundo.posCocina;
        posCocinero = mundo.posCocinero.transform.position;
        transform.position = posCocinero;
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
                if (isInPosition())
                {
                    wait();
                    if (mundo.hayComandas())
                    {
                        platoActual = mundo.takeComanda();
                        walkTo(puestoCocinar.transform.position);
                        estadoActual = EstadosFSM.IR_PUESTO;
                        estadoCocinar = CocinarFSM.PENSAR;
                    }
                }
                break;

            case EstadosFSM.IR_PUESTO:
                rotateTowards(puestoCocinar.transform.parent.position);
                if (isInPosition() && isLookingTowards(puestoCocinar.transform.parent.position))
                {
                    timer = Random.Range(5f, 10f);
                    mundo.setPlato(platoActual.comida, platoActual.mesa, mundo.plato, platoActual.cliente);
                    estadoActual = EstadosFSM.COCINAR;
                }
                break;

            case EstadosFSM.COCINAR:
                //Si acaba de cocinar
                if (timer <= 0)
                {
                    Debug.Log("Termina de cocinar");
                    Plato comida = mundo.getPlato();
                    pick(comida.plato);
                    walkTo(posMesaPedidos);
                    estadoActual = EstadosFSM.LLEVAR_COMIDA;
                }
                //Si han robado el plato lo vuelve a empezar
                else if (!mundo.hayPlato())
                {
                    walkTo(puestoCocinar.transform.position);
                    estadoActual = EstadosFSM.IR_PUESTO;
                }
                else
                {
                    timer -= Time.deltaTime;
                    FSM_Cocinar();
                }
                break;

            case EstadosFSM.ECHAR_LADRON:
                rotateTowards(catco.transform.position);
                if(isLookingTowards(catco.transform.position))
                {
                    angry();
                    catco.Pillado();
                    estadoActual = EstadosFSM.COCINAR;
                }                
                break;

            case EstadosFSM.LLEVAR_COMIDA:
                if (isInPosition())
                {
                    if (mundo.pushPlato(platoActual))
                    {
                        platoActual = null;
                        walkTo(posCocinero);
                        estadoActual = EstadosFSM.ESPERAR;
                    }
                    else
                        wait();
                }
                    
                break;

        }
    }

    public void FSM_Cocinar()
    {

        switch (estadoCocinar)
        {
            case CocinarFSM.COCINAR:
                rotateTowards(mundo.posCocina.transform.parent.position);
                if (isInPosition() && isLookingTowards(mundo.posCocina.transform.parent.position))
                    cook();
                break;

            case CocinarFSM.PENSAR:
                if(timer <= 5)
                {
                    walkTo(mundo.posCocina.transform.position);
                    estadoCocinar = CocinarFSM.COCINAR;
                }
                else
                {
                    posVagar = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
                    walkTo(posVagar);
                    estadoCocinar = CocinarFSM.VAGAR;
                }
                
                break;

            case CocinarFSM.VAGAR:
                if (isInPosition())
                    estadoCocinar = CocinarFSM.PENSAR;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Ladron"))
        {
            Debug.Log("He pillado al ladron");
            catco = other.transform.GetComponentInParent<Catco>();

            estadoActual = EstadosFSM.ECHAR_LADRON;
            
        }
    }
}
