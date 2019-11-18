using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gato : MonoBehaviour {

    public NavMeshAgent agent;
    protected Mundo mundo;
    private float walkingSpeed = 3f;
    private float runningSpeed = 6f;
    private bool estaSentado = false;

    void Awake()
    {
        mundo = FindObjectOfType<Mundo>();
        agent = transform.GetComponent<NavMeshAgent>();
    }

    public bool isInPosition(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= 1.1f;
    }

	public void walkTo(Vector3 destination){
        agent.speed = walkingSpeed;
        //Animación de caminar
        agent.SetDestination(destination);
    }

    public void runTo(Vector3 destination){
        agent.speed = runningSpeed;
        //Animación de correr
        agent.SetDestination(destination);

    }

    protected void pick(GameObject plato){
        //Animación de coger
        Destroy(plato);
    }

    protected bool isAt(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) < 0.5f;
    }

    //Animación inversa a pick
    protected void set(string plato) { }

    protected void idle(){}

    //Se podría cambiar como unico del cliente que es el unico que se sienta
    protected void sitDown(Vector3 lookAt){
        //animacion;
        estaSentado = true;
    }

    protected void getUp(){
        //animacion
        estaSentado = false;
    }

    protected void eat(/*Item food*/){}

    protected void cook(/*Item food*/){}

    protected void play(/*Item item*/){}

    protected void angry(Vector3 lookAt){}

    protected void shamed(Vector3 lookAt){}

}
