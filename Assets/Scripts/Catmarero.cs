using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catmarero : Gato {

    private enum EstadosFSM1 { ESPERAR, COGER_PEDIDO, LLEVAR_PEDIDO, IR_MESA, MISMA_MESA, CONSULTAR_CLIENTE, TOMAR_NOTA, LLEVAR_COMANDA, ESCRIBIR_COMANDA, VOLVER }
    private EstadosFSM1 estadoActual;
    private string pedidoActual = null;

    //Número de mesa del cliente actual
    private int mesaActual;

    //Referencias a posiciones
    private Vector3 puestoCamarero;
    private Vector3 posMesaPedidos;
    private Vector3 posMesaCliente;
    private Vector3 muroComandas;

    private Cliente clienteActual;

    //Inicialización de variables de mundo
    void Start () {
        puestoCamarero = mundo.puestoCamareros;
        muroComandas = mundo.muroComandas;
        transform.position = puestoCamarero;
        posMesaPedidos = mundo.mesaPedidos.transform.position;
        estadoActual = EstadosFSM1.ESPERAR;
	}


	void Update () {
        FSM();	
	}

    void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM1.ESPERAR:
                if (mundo.hayPlatos())
                    estadoActual = EstadosFSM1.COGER_PEDIDO;

                else if (mundo.clientesSinAtender())
                    estadoActual = EstadosFSM1.IR_MESA;

            break;

            //INNECESARIO
            /*case EstadosFSM1.ENTRAR_COCINA:
                walkTo(EntradaCocina);
                estadoActual = EstadosFSM1.COGER_PEDIDO;
            break;*/

            case EstadosFSM1.COGER_PEDIDO:
                walkTo(posMesaPedidos);
                Plato platoActual = mundo.takePlato();
                pedidoActual = platoActual.comida;
                posMesaCliente = mundo.mesas[platoActual.mesa].transform.position;
                pick(platoActual.plato);
                estadoActual = EstadosFSM1.LLEVAR_PEDIDO;
            break;

            case EstadosFSM1.LLEVAR_PEDIDO:
                walkTo(posMesaCliente);
                set(pedidoActual);
                pedidoActual = null;
                estadoActual = EstadosFSM1.VOLVER;
            break;

            case EstadosFSM1.IR_MESA:
                mesaActual = mundo.clientePorAtender();

                clienteActual = mundo.getClienteEnMesa(mesaActual);
                posMesaCliente = mundo.mesas[mesaActual].transform.GetChild(0).position;
                walkTo(posMesaCliente);

                if (isInPosition(posMesaCliente))
                {

                    estadoActual = EstadosFSM1.CONSULTAR_CLIENTE;
                }
            break;

            case EstadosFSM1.MISMA_MESA:
                
                break;

            case EstadosFSM1.CONSULTAR_CLIENTE:

                if (clienteActual.estaDecidido())
                {
                    estadoActual = EstadosFSM1.TOMAR_NOTA;
                }
                else
                {
                    //Si no se ha decidido pasamos al cliente al final de la cola
                    int mesa = mundo.popCliente();
                    mundo.pushCliente(mesa, mundo.getClienteEnMesa(mesa));
                    estadoActual = EstadosFSM1.VOLVER;
                }

                break;

            case EstadosFSM1.TOMAR_NOTA:
                Plato actual = mundo.clienteAtendido();
                pedidoActual = actual.comida;
                estadoActual = EstadosFSM1.LLEVAR_COMANDA;
            break;

            case EstadosFSM1.LLEVAR_COMANDA:
                walkTo(muroComandas);

                if (isInPosition(muroComandas))
                {
                    estadoActual = EstadosFSM1.ESCRIBIR_COMANDA;
                }

            break;

            case EstadosFSM1.ESCRIBIR_COMANDA:
                mundo.pushComanda(mesaActual, pedidoActual);
                estadoActual = EstadosFSM1.VOLVER;
                break;

            case EstadosFSM1.VOLVER:
                walkTo(puestoCamarero);

                if(isInPosition(puestoCamarero))
                    estadoActual = EstadosFSM1.ESPERAR;
            break;
        }
    }
}
