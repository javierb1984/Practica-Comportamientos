using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encargado : Gato
{
    private enum EstadosFSM {  PATRULLAR, SIGUIENTE_PUNTO, IR_CAMARERO, BRONCA}
    private EstadosFSM estadoActual;

    private Transform [] pathNodes;
    private Transform currentNode;
    private int nodeNumber;

    // Start is called before the first frame update
    void Start()
    {
        GameObject path = mundo.pathEncargado;
        pathNodes = new Transform[path.transform.childCount];

        for(int i = 0; i < path.transform.childCount; i++)
        {
            pathNodes[i] = path.transform.GetChild(i);
        }
        nodeNumber = 0;

        estadoActual = EstadosFSM.SIGUIENTE_PUNTO;
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
            case EstadosFSM.SIGUIENTE_PUNTO:
                currentNode = pathNodes[nodeNumber];
                nodeNumber = (nodeNumber + 1) % pathNodes.Length;
                walkTo(currentNode.position);
                estadoActual = EstadosFSM.PATRULLAR;
                break;

            case EstadosFSM.PATRULLAR:
                if (isInPosition())
                {
                    estadoActual = EstadosFSM.SIGUIENTE_PUNTO;
                }

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
