using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catmarero : Gato {

    private enum EstadosFSM1 { ESPERAR, COGER_PEDIDO, LLEVAR_PEDIDO, IR_MESA, TOMAR_NOTA, LLEVAR_COMANDA, VOLVER }
    private EstadosFSM1 estadoActual;
    private string pedidoActual = null;

    //Número de mesa del cliente actual
    private int mesaActual;

    //Referencias a posiciones
    private Vector3 puestoCamarero;
    private Vector3 posMesaPedidos;
    private Vector3 clienteActual;
    private Vector3 muroComandas;

    //Inicialización de variables de mundo
    void Start () {
        puestoCamarero = mundo.puestoCamareros;
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
                clienteActual = mundo.mesas[platoActual.mesa].transform.position;
                pick(platoActual.plato);
                estadoActual = EstadosFSM1.LLEVAR_PEDIDO;
            break;

            case EstadosFSM1.LLEVAR_PEDIDO:
                walkTo(clienteActual);
                set(pedidoActual);
                pedidoActual = null;
                estadoActual = EstadosFSM1.VOLVER;
            break;

            case EstadosFSM1.IR_MESA:
                mesaActual = mundo.clientePorAtender();
                clienteActual = mundo.mesas[mesaActual].transform.position;
                walkTo(clienteActual);

                //Añadir método del cliente
                bool decidido = true;

                if (decidido)
                {
                    estadoActual = EstadosFSM1.TOMAR_NOTA;
                }
                else
                {
                    //Si no se ha decidido pasamos al cliente al final de la cola
                    mundo.pushCliente(mundo.popCliente());
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
                //set(pedidoActual);
                mundo.pushComanda(mesaActual, pedidoActual);
                estadoActual = EstadosFSM1.VOLVER;
            break;

            case EstadosFSM1.VOLVER:
                walkTo(puestoCamarero);
                estadoActual = EstadosFSM1.ESPERAR;
            break;
        }
    }
}
