using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject maitre;
    public GameObject cliente;
    public GameObject catmarero;

    private float countdown;

    void Start()
    {
        countdown = Random.Range(1, 10);
        Instantiate(maitre);
        Instantiate(catmarero);
        Instantiate(cliente);
    }

    void Update()
    {
        if(countdown <= 0)
        {
            countdown = Random.Range(5, 20);
            Instantiate(cliente);
        }

        countdown -= Time.deltaTime;
    }

}
