using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gato : MonoBehaviour {

    private const float TRANSITION_DURATION = 0.5f;

    public NavMeshAgent agent;
    protected Mundo mundo;
    protected GameManager gameManager;
    private float walkingSpeed = 1f;
    private float runningSpeed = 2f;
    private bool estaSentado = false;
    protected Animator animator;

    void Awake()
    {
        mundo = FindObjectOfType<Mundo>();
        gameManager = FindObjectOfType<GameManager>();
        agent = transform.GetComponent<NavMeshAgent>();
        animator = transform.GetComponent<Animator>();
    }

    public bool isInPosition()
    {

        return (agent.remainingDistance < 0.2f);
    }

    //Double-check
    public bool distance(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) <= 0.8;
    }

	public void walkTo(Vector3 destination){
        agent.isStopped = false;
        agent.speed = walkingSpeed;
        animator.Play("Walking");
        /*if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            animator.CrossFadeInFixedTime("Walking", TRANSITION_DURATION, 0);
        }*/
       
        agent.destination = destination;       
    }

    /*public void setAxis()
    {
        animator.SetFloat("Horizontal", transform.right.x);
        animator.SetFloat("Vertical", transform.right.z);
    }*/

    public void runTo(Vector3 destination){
        agent.isStopped = false;
        agent.speed = runningSpeed;
        //animator.SetFloat("Animation", 1);
        animator.Play("Running");
        agent.SetDestination(destination);

    }

    protected void pick(GameObject plato){
        //Animación de coger
        animator.Play("Take");
        Destroy(plato);
    }


    //Animación inversa a pick
    protected void set() {
        animator.Play("Leave");
    }

    protected void idle(){
        agent.isStopped = true;
        animator.Play("Idle");
    }

    //Se podría cambiar como unico del cliente que es el unico que se sienta
    protected void sitDown(){
        agent.isStopped = true;
        //animator.SetFloat("Animation", 4);
        animator.Play("SitDown");
        estaSentado = true;
    }

    protected void rotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f);
    }

    protected bool isLookingTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        float abs = Mathf.Abs(Quaternion.Dot(transform.rotation, lookRotation));
        return (abs >= 0.9999999f);
    }

    protected void getUp(){
        //animacion
        estaSentado = false;
        animator.Play("GetUp");
    }

    protected void wait() {
        agent.isStopped = true;
        //animator.SetFloat("Animation", 2);
        animator.Play("Waiting");
    }


    protected void eat(){
        animator.Play("Eating");
    }

    protected void cook(){
        animator.Play("Cooking");
    }

    protected void play(){
        agent.isStopped = true;
        //animator.SetFloat("Animation", 2);
        animator.Play("Play");
    }

    protected void angry(){
        animator.Play("Angry");
    }

    protected void shamed(){
        animator.Play("Shamed");
    }

    /*void FixedUpdate()
    {
        setAxis();
    }*/
}
