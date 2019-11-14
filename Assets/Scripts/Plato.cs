using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plato : MonoBehaviour
{
    public int mesa;
    public string comida;
    public GameObject plato;

    public Plato(int mesa, string comida, GameObject plato)
    {
        this.mesa = mesa;
        this.comida = comida;
        this.plato = plato;
    }
}
