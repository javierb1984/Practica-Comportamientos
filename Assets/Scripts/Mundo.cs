using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mundo : MonoBehaviour
{
    //Public parameters
    public List<GameObject> mesas;
    public GameObject mesaPedidos;
    public GameObject mesaMaitre;
    public GameObject juguete;
    public GameObject posCocina;
    public GameObject posCocinero;
    public GameObject[] posicionesPedidos;
    public GameObject posCatmereroCocina;
    public Vector3 puestoCamareros;
    public Vector3 puestoMaitre;

    public Vector3 principioCola;
    public Vector3 puestoCaco;
    public Vector3 puertaTrasera;

    public Vector3 posDestroy;

    //Camino del encargado
    public GameObject pathEncargado;

    //Platos
    public GameObject plato;

    //Distancia entre los clientes en la cola
    public float distanciaCola;

    //Rango de spawn de Clientes
    public Vector3 minSpawnCliente;
    public Vector3 maxSpawnCliente;

    //Movimiento de cocinero
    public Vector3 minCocinero;
    public Vector3 maxCocinero;

    //Private parameters
    private int MAX_COMANDAS = 9;
    private int MAX_PLATOS = 6;
    private int MAX_COLA = 3;

    //Posibles comidas
    private string [] comidas = {"A","B","C","D","E"};

    //Cliente-plato-objeto
    private Queue<Plato> comandas;
    private Queue<Plato> platos;

    //Clientes por sentarse
    private Queue<Cliente> clientesCola;

    //Clientes en sus mesas
    private Cliente [] clientesEnMesas;


    //Números de mesa de los clientes por atender
    private Queue<int> clientesPorAtender;

    private bool [] mesasOcupadas;

    //Plato que está cocinando el cocinero
    private Plato platoCocinero;

    void Awake()
    {
        comandas = new Queue<Plato>();
        platos = new Queue<Plato>();
        mesasOcupadas = new bool[mesas.Count];
        clientesEnMesas = new Cliente[mesas.Count];
        clientesPorAtender = new Queue<int>();

        clientesCola = new Queue<Cliente>();
        platoCocinero = null;

        for(int i = 0; i < mesasOcupadas.Length; i++)
        {
            mesasOcupadas[i] = false;
        }

        MAX_COMANDAS = posicionesPedidos.Length;
    }

    //Métodos para acceder a comandas
    /// <summary>
    /// Devuelve "true" si hay comandas, "false" en caso contrario.
    /// </summary>
    public bool hayComandas()
    {
        return comandas.Count != 0;
    }

    /// <summary>
    /// Añade una comanda al final de la cola de comandas si no está llena.
    /// </summary>
    public void pushComanda(int mesa, string comida, Cliente cliente)
    {
        comandas.Enqueue(new Plato(mesa, comida, null, cliente));
    }

    /// <summary>
    /// Devuelve la comanda de la cabeza de la cola de comandas y la borra.
    /// Uso exclusivo del cocinero.
    /// </summary>
    public Plato takeComanda()
    {
        return comandas.Dequeue();
    }

    //Métodos para acceder a platos
    /// <summary>
    /// Devuelve "true" si hay platos sin llevar, "false" en caso contrario.
    /// </summary>
    public bool hayPlatos()
    {
        return !platos.Count.Equals(0);
    }

    /// <summary>
    /// Inserta un plato al final de la cola.
    /// </summary>
    public bool pushPlato(Plato plato)
    {
        bool pushed = false;
        if (platos.Count < MAX_COMANDAS)
        {
            plato.plato = Instantiate(this.plato, posicionesPedidos[platos.Count].transform.position, this.plato.transform.rotation);
            platos.Enqueue(plato);
            pushed = true;
        }
        Debug.Log(pushed);
        return pushed;
    }

    /// <summary>
    /// Devuelve el primer plato de la cola de platos.
    /// </summary>
    public Plato takePlato()
    {
        Plato aux = platos.Dequeue();
        Destroy(aux.plato);
        return aux;
    }

    //Métodos para acceder a clientes
    /// <summary>
    /// Devuelve "true" si hay clientes sin atender, "false" en caso contrario.
    /// </summary>
    public bool clientesSinAtender()
    {
        return clientesPorAtender.Count != 0;
    }

    /// <summary>
    /// Devuelve el cliente por atender en la cabeza de la cola sin sacarlo de ella.
    /// </summary>
    public int clientePorAtender()
    {
        return clientesPorAtender.Peek();
    }

    /// <summary>
    /// Devuelve un objeto plato con la mesa y la comida de un cliente.
    /// Uso exclusivo del Catmarero para sus comandas.
    /// </summary>
    /*public Plato clienteAtendido()
    {
        int mesa = popCliente();

        //Nos proporciona una comida aleatoria
        return new Plato(mesa, comidas[Random.Range(0, comidas.Length)], null);
    }*/

    public string ComidaAleatoria()
    {
        return comidas[Random.Range(0, comidas.Length)];
    }

    /// <summary>
    /// Saca al primer cliente de la cola y devuelve su número de mesa.
    /// </summary>
    public int popCliente()
    {
        return clientesPorAtender.Dequeue();
    }

    /// <summary>
    ///Inserta un cliente al final de la cola de clientes por atender.
    ///Uso exclusivo de cliente.
    /// </summary>
    public void pushCliente(int mesa, Cliente cliente)
    {
        clientesPorAtender.Enqueue(mesa);
        clientesEnMesas[mesa] = cliente;
        mesasOcupadas[mesa] = true;
    }

    public Cliente getClienteEnMesa(int mesa)
    {
        return clientesEnMesas[mesa];
    }

    /// <summary>
    ///Pone a "false" la posición correspondiente a la mesa de un cliente que se ha marchado.
    /// </summary>
    public void clienteLeaves(int mesa)
    {
        mesasOcupadas[mesa] = false;
    }

    /// <summary>
    ///Devuelve "true" si todas las mesas están ocupadas, "false" en caso contrario
    /// </summary>
    public bool mesasIsFull()
    {
        bool full = true;
        foreach(bool mesa in mesasOcupadas)
        {
            if (!mesa)
            {
                full = false;
                break;
            }
        }
        return full;
    }

    /// <summary>
    ///Devuelve la primera mesa vacía
    /// </summary>
    public int nextMesa()
    {
        int mesa = -1;
        for(int i = 0; i < mesasOcupadas.Length; i++)
        {
            if(mesasOcupadas[i] == false)
            {
                mesa = i;
                break;
            }
        }

        return mesa;
    }

    //Métodos para gestionar la cola de clientes
    /// <summary>
    ///Inserta un cliente al final de la cola si no está llena.
    ///Devuelve la siguiente posición libre en la cola
    /// </summary>
    public void pushClienteCola(Cliente cliente)
    {
        clientesCola.Enqueue(cliente);
    }

    /// <summary>
    ///Solo para uso del cliente.
    ///Devuelve la siguiente posición libre en la cola.
    /// </summary>
    public Vector3 getNextCola()
    {
        //El cliente tendrá que moverse al ((principio de la cola) + n * (número de clientes))
        Vector3 aux = principioCola;
        aux.z += distanciaCola * clientesCola.Count;
        return aux;
    }

    /// <summary>
    ///Quita el cliente de la cabecera de la cola si no está vacía y lo devuelve.
    /// </summary>
    public Cliente popClienteCola()
    {
        if (colaIsEmpty())
            return null;
        Cliente aux = clientesCola.Dequeue();
        avanzaCola();
        return aux;
    }

    /// <summary>
    ///Para hacer que la cola avance después que que salga un cliente.
    /// </summary>
    public void avanzaCola()
    {
        //Todos los clientes tendrán que moverse n hacia delante
        foreach (Cliente cliente in clientesCola.ToArray())
        {
            Vector3 v = cliente.transform.position;
            v.z -= distanciaCola;
            cliente.avanzaCola(v);
        }
    }

    /// <summary>
    ///Devuelve "true" si la cola está vacía, "false" en caso contrario.
    /// </summary>
    public bool colaIsEmpty()
    {
        return (clientesCola.Count.Equals(0));
    }

    /// <summary>
    ///Devuelve "true" si la cola está llena, "false" en caso contrario.
    /// </summary>
    public bool colaIsFull()
    {
        return (clientesCola.Count.Equals(MAX_COLA));
    }

    public void setPlato(string nombre, int mesa, GameObject plato, Cliente cliente)
    {
        GameObject platoInstancia = Instantiate(plato);
        platoCocinero = new Plato(mesa, nombre, platoInstancia, cliente);
    }

    public bool hayPlato()
    {
        return platoCocinero != null;
    }

    public Plato getPlato()
    {
        Plato aux = platoCocinero;
        platoCocinero = null;
        return aux;
    }

    public Plato pollPlato()
    {
        return platoCocinero;
    } 

}
