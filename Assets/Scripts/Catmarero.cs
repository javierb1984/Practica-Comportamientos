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
    private Vector3 muroComandas;

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

    //Inicialización de variables de mundo
    void Start () {
        puestoCamarero = mundo.puestoCamareros;
        muroComandas = mundo.muroComandas;
        juguete = mundo.juguete;
        transform.position = puestoCamarero;
        posMesaPedidos = mundo.mesaPedidos.transform.position;
        estadoActual = EstadosFSM1.ESPERAR;
        estadoDistraerse = EstadosDistraerse.TRABAJAR;
        veJuguete = false;
        vuelveAlTrabajo = false;
        distraido = false;
        mirarEncargado = false;
        timer = waitingTime;
        timerDistraerse = 0;
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
                    timerDistraerse = 30;
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
                if (mundo.hayPlatos())
                    estadoActual = EstadosFSM1.COGER_PEDIDO;

                else if (mundo.clientesSinAtender())
                {
                    timer -= Time.deltaTime;

                    if (timer <= 0)
                    {
                        mesaActual = mundo.clientePorAtender();

                        clienteActual = mundo.getClienteEnMesa(mesaActual);
                        posMesaCliente = mundo.mesas[mesaActual].transform.parent.GetChild(0).position;
                        walkTo(posMesaCliente);

                        estadoActual = EstadosFSM1.IR_MESA;
                    }
                }

            break;

            case EstadosFSM1.ENTRAR_COCINA:
                walkTo(posMesaPedidos);
                if(isInPosition())
                    estadoActual = EstadosFSM1.COGER_PEDIDO;
            break;

            case EstadosFSM1.COGER_PEDIDO:
                Plato platoActual = mundo.takePlato();
                pedidoActual = platoActual.comida;
                posMesaCliente = mundo.mesas[platoActual.mesa].transform.position;
                pick(platoActual.plato);
                estadoActual = EstadosFSM1.LLEVAR_PEDIDO;
            break;

            case EstadosFSM1.LLEVAR_PEDIDO:
                walkTo(posMesaCliente);

                if (isInPosition())
                {
                    set(pedidoActual);
                    pedidoActual = null;
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
                Plato actual = mundo.clienteAtendido();
                pedidoActual = actual.comida;
                estadoActual = EstadosFSM1.LLEVAR_COMANDA;
            break;

            case EstadosFSM1.LLEVAR_COMANDA:
                walkTo(muroComandas);

                if (isInPosition())
                {
                    mundo.pushComanda(mesaActual, pedidoActual);
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
