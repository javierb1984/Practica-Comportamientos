using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catmarero : Gato {

    private enum EstadosFSM1 { ESPERAR, ENTRAR_COCINA, COGER_PEDIDO, LLEVAR_PEDIDO, IR_MESA, MISMA_MESA, CONSULTAR_CLIENTE, TOMAR_NOTA, LLEVAR_COMANDA, VOLVER }
    private EstadosFSM1 estadoActual;
    private enum EstadosDistraerse {TRABAJAR, IR_JUGUETE, DISTRAERSE, AVERGONZADO}
    private EstadosDistraerse estadoDistraerse;
    private string pedidoActual = null;

    //Número de mesa del cliente actual
    private int mesaActual;

    //Referencias a posiciones
    private Vector3 puestoCamarero;
    private Vector3 posMesaPedidos;
    private Vector3 posMesaCliente;
    private Vector3 posCatmareroCocina;

    private Cliente clienteActual;

    //Esperas necesarias para el catmarero
    private float timer;
    private float waitingTime = 1.5f;
    private float timerDistraerse;

    //True si el juguete entra en su collider
    private bool veJuguete;
    private bool mirarEncargado;
    private bool distraido;
    public bool vuelveAlTrabajo;
    private GameObject juguete;
    private GameObject encargado;
    private GameObject platoLlevando;
    private Transform bandeja;

    //Inicialización de variables de mundo
    void Start () {
        puestoCamarero = mundo.puestoCamareros;
        posCatmareroCocina = mundo.posCatmereroCocina.transform.position;
        juguete = mundo.juguete;
        transform.position = puestoCamarero;
        posMesaPedidos = mundo.mesaPedidos.transform.GetChild(0).position;
        estadoActual = EstadosFSM1.ESPERAR;
        estadoDistraerse = EstadosDistraerse.TRABAJAR;
        veJuguete = false;
        vuelveAlTrabajo = false;
        distraido = false;
        mirarEncargado = false;
        timer = waitingTime;
        timerDistraerse = 30;
        bandeja = transform.Find("bandeja");
        idle();
	}


	void Update () {
        DistraerseFSM();
	}

    void DistraerseFSM()
    {
        switch (estadoDistraerse)
        {
            case EstadosDistraerse.TRABAJAR:
                FSM();
                timerDistraerse -= Time.deltaTime;
                if (veJuguete && timerDistraerse <= 0)
                {
                    timerDistraerse = 60;
                    veJuguete = false;

                    walkTo(juguete.transform.GetChild(0).position);
                    estadoDistraerse = EstadosDistraerse.IR_JUGUETE;
                }
                break;
            case EstadosDistraerse.IR_JUGUETE:
                rotateTowards(juguete.transform.position);
                if (isInPosition() && isLookingTowards(juguete.transform.position))
                {
                    play();
                    distraido = true;
                    estadoDistraerse = EstadosDistraerse.DISTRAERSE;
                }
                break;
            case EstadosDistraerse.DISTRAERSE:
                if (mirarEncargado)
                {
                    idle();
                    rotateTowards(encargado.transform.position);
                    if (isLookingTowards(encargado.transform.position))
                    {
                        shamed();
                        estadoDistraerse = EstadosDistraerse.AVERGONZADO;
                    }
                }

                break;
            case EstadosDistraerse.AVERGONZADO:
                if (vuelveAlTrabajo)
                {
                    distraido = false;
                    vuelveAlTrabajo = false;
                    mirarEncargado = false;
                    estadoDistraerse = EstadosDistraerse.TRABAJAR;
                }
                break;

        }
    }

    void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM1.ESPERAR:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    if (mundo.hayPlatos())
                    {
                        walkTo(posMesaPedidos);
                        estadoActual = EstadosFSM1.ENTRAR_COCINA;
                    }


                    else if (mundo.clientesSinAtender())
                    {

                        mesaActual = mundo.clientePorAtender();

                        clienteActual = mundo.getClienteEnMesa(mesaActual);
                        posMesaCliente = mundo.mesas[mesaActual].transform.parent.GetChild(0).position;
                        Debug.Log("ESPERAR: " + mesaActual + "," + clienteActual);
                        walkTo(posMesaCliente);

                        estadoActual = EstadosFSM1.IR_MESA;

                    }
                }

            break;

            case EstadosFSM1.ENTRAR_COCINA:                
                if(isInPosition())
                    estadoActual = EstadosFSM1.COGER_PEDIDO;
            break;

            case EstadosFSM1.COGER_PEDIDO:
                Plato platoActual = mundo.takePlato();
                pedidoActual = platoActual.comida;
                mesaActual = platoActual.mesa;
                clienteActual = platoActual.cliente;
                posMesaCliente = mundo.mesas[mesaActual].transform.parent.GetChild(0).position;
                pick(platoActual.plato);

                //Ponemos el plato en la bandeja del catmarero
                platoLlevando = Instantiate(mundo.plato, bandeja.position + transform.up * 0.05f, mundo.plato.transform.rotation);
                platoLlevando.transform.SetParent(bandeja);

                walkTo(posMesaCliente);
                estadoActual = EstadosFSM1.LLEVAR_PEDIDO;
            break;

            case EstadosFSM1.LLEVAR_PEDIDO:
                if (isInPosition())
                {
                    Destroy(platoLlevando);
                    GameObject pedido = Instantiate(mundo.plato, mundo.mesas[mesaActual].transform.GetChild(0).position, mundo.plato.transform.rotation);
                    clienteActual.servir(pedido);
                    pedidoActual = null;
                    walkTo(puestoCamarero);
                    estadoActual = EstadosFSM1.VOLVER;
                }
            break;

            case EstadosFSM1.IR_MESA:
                if (isInPosition())
                {
                    timer = waitingTime;
                    idle();
                    estadoActual = EstadosFSM1.CONSULTAR_CLIENTE;
                }
            break;

            case EstadosFSM1.CONSULTAR_CLIENTE:
                timer -= Time.deltaTime;
                rotateTowards(clienteActual.transform.position);
                if (timer <= 0 && isLookingTowards(clienteActual.transform.position))
                {
                    if (clienteActual.estaDecidido())
                    {
                        //Si se ha decidido sacamos al cliente de la cola
                        mundo.popCliente();
                        clienteActual.atender();
                        estadoActual = EstadosFSM1.TOMAR_NOTA;
                    }
                    else
                    {
                        //Si no se ha decidido pasamos al cliente al final de la cola
                        int mesa = mundo.popCliente();
                        mundo.pushCliente(mesa, mundo.getClienteEnMesa(mesa));

                        walkTo(puestoCamarero);
                        estadoActual = EstadosFSM1.VOLVER;
                    }
                }

                break;

            case EstadosFSM1.TOMAR_NOTA:
                pedidoActual = mundo.ComidaAleatoria();
                walkTo(posCatmareroCocina);
                estadoActual = EstadosFSM1.LLEVAR_COMANDA;
            break;

            case EstadosFSM1.LLEVAR_COMANDA:              

                if (isInPosition())
                {
                    Debug.Log("LLEVARCOMANDA: "+mesaActual + "," + pedidoActual + "," + clienteActual);
                    mundo.pushComanda(mesaActual, pedidoActual, clienteActual);
                    estadoActual = EstadosFSM1.VOLVER;
                }

            break;

            case EstadosFSM1.VOLVER:
                if (isInPosition())
                {
                    timer = waitingTime;
                    idle();
                    estadoActual = EstadosFSM1.ESPERAR;
                }
            break;
        }
    }

    public void volverAlTrabajo()
    {
        vuelveAlTrabajo = true;
    }

    public bool isDistraido()
    {
        return distraido;
    }

    public void lookAtEncargado(GameObject encargado)
    {
        this.encargado = encargado;
        mirarEncargado = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == ("Juguete"))
        {
            veJuguete = true;
        }
    }
}
