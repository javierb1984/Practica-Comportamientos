using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plato
{
    public int mesa;
    public string comida;
    public GameObject plato;
    public Cliente cliente;

    public Plato(int mesa, string comida, GameObject plato, Cliente cliente)
    {
        this.mesa = mesa;
        this.comida = comida;
        this.plato = plato;
        this.cliente = cliente;
    }
}
