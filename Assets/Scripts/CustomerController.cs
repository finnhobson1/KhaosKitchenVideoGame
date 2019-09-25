using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    public float maxSpeed;

    public float xMax;
    public float zMax;
    public float xMin;
    public float zMin;

    private float x;
    private float z;
    private float time;
    private float angle;

    private int junction = -1;

    private GameController gameController;
    private AnimationController animationController;

    public GameObject shirt;
    public List<GameObject> hatParts;


    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        animationController = GameObject.FindGameObjectWithTag("AnimationController").GetComponent<AnimationController>();

        // Walking Down Path into Restaurant
        if (transform.localPosition.x > xMax + 5)
        {
            x = -maxSpeed;
            z = 0;
            transform.localRotation = Quaternion.Euler(0, 270, 0);
            time = 0.0f;
        }

        if (transform.localPosition.x < xMin - 5)
        {
            x = maxSpeed;
            z = 0;
            transform.localRotation = Quaternion.Euler(0, 90, 0);
            time = 0.0f;
        }
    }


    void Update()
    {
        time += Time.deltaTime;
        int rand;

        if (!gameController.isRoundPaused && gameController.isGameStarted)
        {
            // Check Customer Rotation
            if (x == 0)
            {
                if (z > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (z < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                }

            }
            if (z == 0)
            {
                if (x > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                else if (x < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 270, 0);
                }

            }

            // MOVING INSIDE RESTAURANT
            // Left Exit
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -29.6 && transform.localPosition.z < -29.4 && junction != 0)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 3);
                    if (rand == 0 && junction != 3)
                    {
                        x = 0;
                        z = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 1 && junction != 11)
                    {
                        z = 0;
                        x = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 4)
                    {
                        x = 0;
                        z = maxSpeed;
                        picked = true;
                    }
                }             
                junction = 0;

                // Leave if low customer satisfaction
                if (gameController.customerSatisfaction < 20 && animationController.customerNumber > 1)
                {
                    z = 0;
                    x = maxSpeed;
                    animationController.customerNumber--;
                }
            }  

            // Right Exit
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -29.6 && transform.localPosition.z < -29.4 && junction != 1)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 3);
                    if (rand == 0 && junction != 2)
                    {
                        x = 0;
                        z = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 1)
                    {
                        z = 0;
                        x = maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 5)
                    {
                        x = 0;
                        z = maxSpeed;
                        picked = true;
                    }
                }   
                junction = 1;

                // Leave if low customer satisfaction
                if (gameController.customerSatisfaction < 20 && animationController.customerNumber > 1)
                {
                    z = 0;
                    x = -maxSpeed;
                    animationController.customerNumber--;
                }
            }

            // Top Right
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 2)
            {
                if (z != 0)
                {
                    z = 0;
                    x = maxSpeed;
                }
                else if (x != 0)
                {
                    x = 0;
                    z = maxSpeed;
                }
                junction = 2;
            }

            // Top Left
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 3)
            {
                if (z != 0)
                {
                    z = 0;
                    x = -maxSpeed;
                }
                else if (x != 0)
                {
                    x = 0;
                    z = maxSpeed;
                }
                junction = 3;
            }

            // Bottom Left
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 4)
            {
                if (z != 0)
                {
                    z = 0;
                    x = -maxSpeed;
                }
                else if (x != 0)
                {
                    x = 0;
                    z = -maxSpeed;
                }
                junction = 4;
            }

            // Bottom Right
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 5)
            {
                if (z != 0)
                {
                    z = 0;
                    x = maxSpeed;
                }
                else if (x != 0)
                {
                    x = 0;
                    z = -maxSpeed;
                }
                junction = 5;
            }

            // Bottom Right Middle
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 6)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 2);
                    if (rand == 0 && junction != 10)
                    {
                        x = 0;
                        z = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 1 && junction != 7)
                    {
                        z = 0;
                        x = maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 5)
                    {
                        z = 0;
                        x = -maxSpeed;
                        picked = true;
                    }
                }
                junction = 6;
            }

            // Bottom Left Middle 
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 7)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 3);
                    if (rand == 0 && junction != 11)
                    {
                        x = 0;
                        z = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 1 && junction != 6)
                    {
                        z = 0;
                        x = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 4)
                    {
                        z = 0;
                        x = maxSpeed;
                        picked = true;
                    }
                }  
                junction = 7;
            }

            // Top Left Middle
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 8)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 3);
                    if (rand == 0 && junction != 11)
                    {
                        x = 0;
                        z = maxSpeed;
                        picked = true;
                    }
                    if (rand == 1 && junction != 9)
                    {
                        z = 0;
                        x = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 3)
                    {
                        z = 0;
                        x = maxSpeed;
                        picked = true;
                    }
                } 
                junction = 8;
            }

            // Top Right Middle 
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 9)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 3);
                    if (rand == 0 && junction != 10)
                    {
                        x = 0;
                        z = maxSpeed;
                        picked = true;
                    }
                    if (rand == 1 && junction != 2)
                    {
                        z = 0;
                        x = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 8)
                    {
                        z = 0;
                        x = maxSpeed;
                        picked = true;
                    }
                }      
                junction = 9;
            }

            // Right Middle
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -29.6 && transform.localPosition.z < -29.4 && junction != 10)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 4);
                    if (rand == 0 && junction != 6)
                    {
                        x = 0;
                        z = maxSpeed;
                        picked = true;
                    }
                    if (rand == 1 && junction != 9)
                    {
                        x = 0;
                        z = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 11)
                    {
                        z = 0;
                        x = maxSpeed;
                        picked = true;
                    }
                    if (rand == 3 && junction != 1)
                    {
                        z = 0;
                        x = -maxSpeed;
                        picked = true;
                    }
                }         
                junction = 10;
            }

            // Left Middle 
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -29.6 && transform.localPosition.z < -29.4 && junction != 11)
            {
                bool picked = false;
                while (!picked)
                {
                    rand = Random.Range(0, 4);
                    if (rand == 0 && junction != 7)
                    {
                        x = 0;
                        z = maxSpeed;
                        picked = true;
                    }
                    if (rand == 1 && junction != 8)
                    {
                        x = 0;
                        z = -maxSpeed;
                        picked = true;
                    }
                    if (rand == 2 && junction != 0)
                    {
                        z = 0;
                        x = maxSpeed;
                        picked = true;
                    }
                    if (rand == 3 && junction != 10)
                    {
                        z = 0;
                        x = -maxSpeed;
                        picked = true;
                    }
                }          
                junction = 11;
            }

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z + z);
        }

        // Face Camera While Celebrating Complete Round
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }
}
