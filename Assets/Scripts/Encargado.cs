using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encargado : Gato
{
    private enum EstadosFSM {  PATRULLAR, SIGUIENTE_PUNTO, IR_CAMARERO, BRONCA, BRONCA_EN_CURSO}
    private EstadosFSM estadoActual;

    private Transform [] pathNodes;
    private Transform currentNode;
    private int nodeNumber;
    private Catmarero catmarero;

    private float timer;

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
                walkTo(catmarero.transform.position - transform.forward);
                catmarero.lookAtEncargado(gameObject);
                estadoActual = EstadosFSM.BRONCA;
                break;

            case EstadosFSM.BRONCA:
                rotateTowards(catmarero.transform.position);
                if (isInPosition() && isLookingTowards(catmarero.transform.position))
                {
                    angry();
                    timer = 1.6f;
                    estadoActual = EstadosFSM.BRONCA_EN_CURSO;
                }
                break;

            case EstadosFSM.BRONCA_EN_CURSO:
                timer -= Time.deltaTime;
                Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("Angry"));
                if(timer <= 0)
                {
                    catmarero.volverAlTrabajo();
                    estadoActual = EstadosFSM.SIGUIENTE_PUNTO;
                }
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Catmarero"))
        {
            catmarero = other.transform.GetComponentInParent<Catmarero>();
            if (catmarero.isDistraido())
            {
                estadoActual = EstadosFSM.IR_CAMARERO;
                walkTo(catmarero.transform.position - transform.forward);
            }

        }
    }
}
