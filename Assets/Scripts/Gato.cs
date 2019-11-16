using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gato : MonoBehaviour {

    public NavMeshAgent agent;
    protected Mundo mundo;
    private float walkingSpeed = 3.5f;
    private float runningSpeed = 7f;

    void Awake()
    {
        mundo = FindObjectOfType<Mundo>();
        agent = transform.GetComponent<NavMeshAgent>();
        //agent.Warp(mundo.cojines[0].transform.position);
    }

	protected void walkTo(Vector3 destination){
        agent.speed = walkingSpeed;
        //Animación de caminar
        agent.SetDestination(destination);
    }

    protected void runTo(Vector3 destination){
        agent.speed = runningSpeed;
        //Animación de correr
        agent.SetDestination(destination);

    }

    protected void pick(GameObject plato){
        //Animación de coger
        Destroy(plato);
    }

    //Animación inversa a pick
    protected void set(string plato) { }

    protected void idle(){}

    protected void sitDown(Vector3 lookAt){}

    protected void getUp(){}

    protected void eat(/*Item food*/){}

    protected void cook(/*Item food*/){}

    protected void play(/*Item item*/){}

    protected void angry(Vector3 lookAt){}

    protected void shamed(Vector3 lookAt){}

}
