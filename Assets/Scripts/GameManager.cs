﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject maitre;
    public GameObject cliente;
    public GameObject catmarero;

    private float countdown;
    private int numClientes;

    void Start()
    {
        countdown = Random.Range(10, 20);
        numClientes = 1;
        Instantiate(maitre);
        Instantiate(catmarero);
        Instantiate(cliente);
    }

    void Update()
    {
        if (numClientes < 11)
        {
            if (countdown <= 0)
            {
                countdown = Random.Range(10, 20);
                Instantiate(cliente);
                numClientes++;
            }

            countdown -= Time.deltaTime;
        }
    }

}
