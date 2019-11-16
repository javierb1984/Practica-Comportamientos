using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mundo : MonoBehaviour
{
    //Public parameters
    public List<GameObject> mesas;
    public GameObject mesaPedidos;
    public Vector3 puestoCamareros;
    public Vector3 puestoMaitre;

    public Vector3 principioCola;
    public Vector3 puestoCaco;
    public Vector3 puertaTrasera;


    //Private parameters
    private int MAX_COMANDAS = 9;
    private int MAX_PLATOS = 6;
    private int MAX_COLA = 6;

    //Posibles comidas
    private string [] comidas = {"A","B","C","D","E"};

    //Cliente-plato-objeto
    private Queue<Plato> comandas;
    private Queue<Plato> platos;

    //Clientes por sentarse
    private Queue<GameObject> clientesCola;

    //Números de mesa de los clientes por atender
    private Queue<int> clientesPorAtender;

    //Cola con los clientes esperando al maitre
    private Queue<Cliente> ColaMaitre;

    private List<bool> mesasOcupadas;

    //Plato que está cocinando el cocinero
    private Plato platoCocinero;
 

    void Awake()
    {
        comandas = new Queue<Plato>(MAX_COMANDAS);
        platos = new Queue<Plato>(MAX_PLATOS);
        mesasOcupadas = new List<bool>(mesas.Count);
        clientesPorAtender = new Queue<int>(mesas.Count);

        clientesCola = new Queue<GameObject>(MAX_COLA);
        platoCocinero = null;

        //Por si no se inicializa a false la lista
        for(int i = 0; i < mesasOcupadas.Capacity; i++)
        {
            mesasOcupadas[i] = false;
        }

        ColaMaitre = new Queue<Cliente>();

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
    public void pushComanda(int mesa, string comida)
    {
        if (comandas.Count <= MAX_COMANDAS)
        {
            comandas.Enqueue(new Plato(mesa, comida, null));
        }
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
        return platos.Count != 0;
    }

    /// <summary>
    /// Inserta un plato al final de la cola.
    /// </summary>
    public void pushPlato(Plato plato)
    {
        if (platos.Count <= MAX_COMANDAS)
            platos.Enqueue(plato);
    }

    /// <summary>
    /// Devuelve el primer plato de la cola de platos.
    /// </summary>
    public Plato takePlato()
    {
        return platos.Dequeue();
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
    public Plato clienteAtendido()
    {
        int mesa = popCliente();

        //Nos proporciona una comida aleatoria
        return new Plato(mesa, comidas[Random.Range(0, comidas.Length)], null);
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
    public void pushCliente(int mesa)
    {
        clientesPorAtender.Enqueue(mesa);
        mesasOcupadas[mesa] = true;
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
        return !mesasOcupadas.Contains(false);
    }

    /// <summary>
    ///Devuelve la primera mesa vacía
    /// </summary>
    public int nextMesa()
    {
        int mesa = -1;
        for(int i = 0; i < mesasOcupadas.Count; i++)
        {
            if(mesasOcupadas[i] == true)
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
    /// </summary>
    public void pushClienteCola(GameObject cliente)
    {
        if (!colaIsFull())
        {
            clientesCola.Enqueue(cliente);
            //El cliente tendrá que moverse al ((principio de la cola) + n * (número de clientes))
        }
    }

    /// <summary>
    ///Quita el cliente de la cabecera de la cola si no está vacía y lo devuelve.
    /// </summary>
    public GameObject popClienteCola()
    {
        if (colaIsEmpty())
            return null;

        //Todos los clientes tendrán que moverse n hacia delante
        return clientesCola.Dequeue();
    }

    /// <summary>
    ///Devuelve "true" si la cola está vacía, "false" en caso contrario.
    /// </summary>
    public bool colaIsEmpty()
    {
        return (clientesCola.Count == 0);
    }

    /// <summary>
    ///Devuelve "true" si la cola está llena, "false" en caso contrario.
    /// </summary>
    public bool colaIsFull()
    {
        return (clientesCola.Count == MAX_COLA);
    }

    public void setPlato(string nombre, int mesa, GameObject plato)
    {
        platoCocinero = new Plato(mesa, nombre, plato);
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

    /// 
    /// 
    /// </summary>
    public void pushColaMaitre(Cliente gato) {
        ColaMaitre.Enqueue(gato);
    }

    public bool estaEnColaMaitre(Cliente gato)
    {
        return ColaMaitre.Contains(gato);
    }
 

}
