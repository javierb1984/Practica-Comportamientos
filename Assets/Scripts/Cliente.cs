﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cliente : Gato
{
    private enum EstadosFSM { VAGAR, EN_COLA, ESPERAR_MAITRE, SEGUIR_MAITRE, DECIDIR_MENU, ESPERAR_PEDIDO, COMER};
    private enum VagarFSM { VAGAR, PENSAR};
    private EstadosFSM estadoActual;
    private VagarFSM estadoVagar;
    private float timer;
    private bool avisoMaitre;
    private bool avisoSentarse;
    private bool decidido;
    private bool atendido;
    private bool servido;
    private int mesaActual;
    private Maitre maitre;

    //Rango de spawn
    Vector3 min;
    Vector3 max;

    //Posición a la que vaga
    Vector3 posVagar;

    //Referencia a posicion en la cola
    private Vector3 posColaMaitre;

    // Start is called before the first frame update
    void Start()
    {
        //Posición de spawn
        min = mundo.minSpawnCliente;
        max = mundo.maxSpawnCliente;
        transform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

        //Segundos
        this.timer = Random.Range(5f, 20f);
        this.estadoActual = EstadosFSM.VAGAR;
        this.avisoMaitre = false;
        this.avisoSentarse = false;
        this.decidido = false;
        this.servido = false;
        this.atendido = false;
        maitre = FindObjectOfType<Maitre>();
    }

    // Update is called once per frame
    void Update()
    {
        FSM();
    }

    void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM.VAGAR:
                
                if (timer <= 0)
                {                    
                    if (!mundo.colaIsFull()) {
                        estadoActual = EstadosFSM.EN_COLA;
                    }
                    else
                    {
                        timer = Random.Range(5, 20);
                    }

                }
                else
                {
                    timer -= Time.deltaTime;
                    FSM_Vagar();
                }

                break;

            case EstadosFSM.EN_COLA:
                posColaMaitre = mundo.getNextCola();
                walkTo(posColaMaitre);

                //Si la cola se ha llenado mientras llegaba volvemos a vagar
                if (mundo.colaIsFull()) {
                    estadoActual = EstadosFSM.VAGAR;
                }
                //Si ha llegado a su posición le metemos en la cola
                else if (isInPosition(posColaMaitre))
                {
                    mundo.pushClienteCola(this);
                    estadoActual = EstadosFSM.ESPERAR_MAITRE;
                }
                break;

            case EstadosFSM.ESPERAR_MAITRE:

                if (avisoMaitre)
                {
                    estadoActual = EstadosFSM.SEGUIR_MAITRE;
                }
                break;

            case EstadosFSM.SEGUIR_MAITRE:
                walkTo(maitre.transform.position);
                if (avisoSentarse)
                {
                    timer = Random.Range(5, 20);//reutilizar timer o usar uno nuevo
                    Vector3 position = mundo.mesas[mesaActual].transform.position;
                    walkTo(position);                    

                    if (isInPosition(position))
                    {
                        maitre.Sentado();
                        mundo.pushCliente(mesaActual, this);
                        estadoActual = EstadosFSM.DECIDIR_MENU;
                    }
                }
                break;

            case EstadosFSM.DECIDIR_MENU:
                avisoMaitre = false;
                if (timer > 0)
                    timer -= Time.deltaTime;
                else if(timer <= 0){
                    decidido = true;
                    timer -= Time.deltaTime;
                }
                ///Se puede probar esto o modificar el estado desde el camarero
                ///revisar/hablarlo en clase
                if (atendido)
                {
                    timer = Random.Range(5, 20);//reutilizar timer o usar uno nuevo 
                    estadoActual = EstadosFSM.ESPERAR_PEDIDO;
                }

                break;
            case EstadosFSM.ESPERAR_PEDIDO:
                timer--;
                if(timer == 0)
                {
                    //this.angry()
                    //this.walkTo() fuera del restaurante
                    //
                    timer = 5000;//reutilizar timer o usar uno nuevo 
                    estadoActual = EstadosFSM.VAGAR;
                }
                if (servido)
                {
                    this.eat();
                    timer = Random.Range(5000, 10000);//reutilizar timer o usar uno nuevo 
                    estadoActual = EstadosFSM.COMER;
                }
                break;
            case EstadosFSM.COMER:
                timer--;
                if(timer == 0)
                {
                    //this.walkTo() fuera del restaurante
                    //
                    timer = 5000;//reutilizar timer o usar uno nuevo 
                    estadoActual = EstadosFSM.VAGAR;
                }
                break;
        }

       
    }

    void FSM_Vagar()
    {
        switch (estadoVagar)
        {
            case VagarFSM.PENSAR:
                posVagar = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

                estadoVagar = VagarFSM.VAGAR;
                break;
            case VagarFSM.VAGAR:
                walkTo(posVagar);
                if (isInPosition(posVagar))
                    estadoVagar = VagarFSM.PENSAR;
                break;
        }
    }

    public void concedeMesa(int mesa)
    {
        mesaActual = mesa;
        avisoMaitre = true;
        //////////
    }

    public void sentar()
    {
        avisoSentarse = true;
    }

    public void servir()//su usara en camarero
    {
        servido = true;
    }

    public void atender()//se usará en el camarero
    {
        atendido = true;
    }

    public bool estaDecidido()
    {
        return decidido;
    }
}