using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encargado : Gato
{
    private enum EstadosFSM {  PATRULLAR, IR_CAMARERO, BRONCA}
    private EstadosFSM estadoActual;

    // Start is called before the first frame update
    void Start()
    {
        estadoActual = EstadosFSM.PATRULLAR;
    }

    // Update is called once per frame
    void Update()
    {
        FSM();
    }

    public void FSM()
    {
        switch (estadoActual)
        {
            case EstadosFSM.PATRULLAR:
                ///caminar en vueltas por el restaurante
                ///if (collider de camarero && estaDistraido)
                ///{
                ///walkTo(camarero);
                ///estadoActual = EstadosFSM.IR_CAMARERO;
                ///}

                break;

            case EstadosFSM.IR_CAMARERO:
                ///if (isInPosition(camarero))
                ///{
                ///angry();
                ///camarero.bronca();
                ///estadoActual = EstadosFSM.BRONCA;
                ///}
                break;

            case EstadosFSM.BRONCA:
                ///if (!estaAngry())
                ///{
                ///estadoActual = EstadosFSM.PATRULLAR
                break;
        }
    }

}
