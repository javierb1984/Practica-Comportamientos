using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cliente : Gato
{
    private enum EstadosFSM { VAGAR, EN_COLA ,ESPERAR_MAITRE, DECIDIR_MENU, ESPERAR_PEDIDO, COMER};
    private EstadosFSM estadoActual;
    private Mundo mundo;
    private float timer;
    private bool decidido;
    private bool atendido;
    private bool servido;

    //Referencia a posicion del maitre
    //Con esto se stackearan uno encima de otro!!!!
    private Vector3 posColaMaitre;

    // Start is called before the first frame update
    void Start()
    {
        this.mundo = FindObjectOfType<Mundo>();
        this.timer = 5000;//Placeholder
        this.estadoActual = EstadosFSM.VAGAR;
        this.decidido = false;
        this.servido = false;
        this.atendido = false;
        this.posColaMaitre = mundo.puestoMaitre; //Cambiar a cuando esté definido el objeto
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM.VAGAR:

                if (timer == 0)
                {
                    this.walkTo(mundo.puestoMaitre);
                    mundo.pushColaMaitre(this);
                    estadoActual = EstadosFSM.ESPERAR_MAITRE;
                }
                else if (timer == 5000)//placeholder
                {
                    //andar random o idle
                }
                timer--;
                break;

            case EstadosFSM.ESPERAR_MAITRE:
                if (!mundo.estaEnColaMaitre(this))//revisar como hacer esto
                {
                    //andar hasta la mesa y sentarse
                    timer = Random.Range(5000, 10000);//reutilizar timer o usar uno nuevo
                    //mundo.pushCliente();
                    estadoActual = EstadosFSM.DECIDIR_MENU;
                }
                break;

            case EstadosFSM.DECIDIR_MENU:
                if(timer > 0)
                    timer--;
                else if(timer == 0){
                    decidido = true;
                    timer--;
                }
                ///Se puede probar esto o modificar el estado desde el camarero
                ///revisar/hablarlo en clase
                if (atendido)
                {
                    timer = Random.Range(5000, 10000);//reutilizar timer o usar uno nuevo 
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
