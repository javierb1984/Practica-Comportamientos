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

    //Private parameters
    private int MAX_COMANDAS = 9;
    private int MAX_PLATOS = 6;

    //Posibles comidas
    private string [] comidas = {"A","B","C","D","E"};

    //Cliente-plato-objeto
    private Queue<Plato> comandas;
    private Queue<Plato> platos;

    //Números de mesa de los clientes por atender
    private Queue<int> clientesPorAtender;

    //Cola con los clientes esperando al maitre
    private Queue<Cliente> ColaMaitre;

    private List<bool> mesasOcupadas;
 

    void Awake()
    {
        comandas = new Queue<Plato>(MAX_COMANDAS);
        platos = new Queue<Plato>(MAX_PLATOS);
        mesasOcupadas = new List<bool>(mesas.Count);
        clientesPorAtender = new Queue<int>(mesas.Count);
        ColaMaitre = new Queue<Cliente>();
    }

    //Métodos para acceder a comandas
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
        int i = Random.Range(0, comidas.Length);
        return new Plato(mesa, comidas[i], null);
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

    public void clienteLeaves(int mesa)
    {
        mesasOcupadas[mesa] = false;
    }

    /// <summary>
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
