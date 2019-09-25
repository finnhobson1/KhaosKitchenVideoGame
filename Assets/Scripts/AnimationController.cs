using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnimationController : MonoBehaviour
{
    public GameController gameController;

    public Image arrow;

    public GameObject chefPrefab;
    public GameObject customerPrefab;
    public GameObject kitchenPrefab;
    public GameObject firePrefab;
    public int customerNumber = 0;

    private bool chefsSpawned = false;
    private int currentRound = 0;
    private int fireCount = 0;


    void Start()
    {
        Instantiate(kitchenPrefab);
    }


    void Update()
    {
        if (gameController.spawnChefs && !chefsSpawned)
        {
            StartCoroutine(SpawnChefs());
            chefsSpawned = true;
        }

        if (currentRound < gameController.roundNumber && gameController.isGameStarted && fireCount > 0)
        {
            DestroyFires();
            fireCount = 0;
        }

        if (currentRound < gameController.roundNumber && gameController.isGameStarted && !gameController.isRoundPaused)
        {
            if (customerNumber < 10) SpawnCustomers();
            currentRound = gameController.roundNumber;
        }

        if (fireCount < gameController.fireCount)
        {
            SpawnFire();
            fireCount = gameController.fireCount;
        }

    }


    private IEnumerator SpawnChefs()
    {
        yield return new WaitForSecondsRealtime(3);
        var players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            Vector3 pos = new Vector3(((2 * i * 40) + 40) / (players.Length * 2) - 20, 0, 0);
            GameObject newChef = Instantiate(chefPrefab, pos, transform.rotation);
            Image newArrow = Instantiate(arrow);
            newArrow.color = players[i].PlayerColour;
            newArrow.transform.SetParent(GameObject.FindGameObjectWithTag("ServerCanvas").transform, false);
            newChef.GetComponent<ChefController>().arrow = newArrow;
        }
    }


    private void SpawnCustomers()
    {
        GameObject customer = Instantiate(customerPrefab, new Vector3(-50, 0.5f, -29.5f), transform.rotation);
        customerNumber++;
        GameObject shirt = customer.GetComponent<CustomerController>().shirt;
        Material shirtColour1 = new Material(shirt.GetComponent<MeshRenderer>().material);
        List<GameObject> hatParts = customer.GetComponent<CustomerController>().hatParts;
        int randShirt1 = UnityEngine.Random.Range(0, 7);
        if (randShirt1 == 0) shirtColour1.color = Color.red;
        if (randShirt1 == 1) shirtColour1.color = Color.yellow;
        if (randShirt1 == 2) shirtColour1.color = Color.green;
        if (randShirt1 == 3) shirtColour1.color = Color.blue;
        if (randShirt1 == 4) shirtColour1.color = Color.cyan;
        if (randShirt1 == 5) shirtColour1.color = Color.magenta;
        if (randShirt1 == 6) shirtColour1.color = Color.white;
        shirt.GetComponent<MeshRenderer>().material = shirtColour1;

        Material hatColour1 = new Material(shirt.GetComponent<MeshRenderer>().material);
        int randHat1 = UnityEngine.Random.Range(0, 3);
        if (randHat1 == 0) hatColour1.color = Color.red;
        if (randHat1 == 1) hatColour1.color = Color.blue;
        if (randHat1 == 2) hatColour1.color = Color.cyan;
        foreach (GameObject part in hatParts)
        {
            part.GetComponent<MeshRenderer>().material = hatColour1;
        }

        customer = Instantiate(customerPrefab, new Vector3(60, 0.5f, -29.5f), transform.rotation);
        customerNumber++;
        shirt = customer.GetComponent<CustomerController>().shirt;
        Material shirtColour2 = new Material(shirt.GetComponent<MeshRenderer>().material);
        int randShirt2 = UnityEngine.Random.Range(0, 7);
        if (randShirt2 == 0) shirtColour2.color = Color.red;
        if (randShirt2 == 1) shirtColour2.color = Color.yellow;
        if (randShirt2 == 2) shirtColour2.color = Color.green;
        if (randShirt2 == 3) shirtColour2.color = Color.blue;
        if (randShirt2 == 4) shirtColour2.color = Color.cyan;
        if (randShirt2 == 5) shirtColour2.color = Color.magenta;
        if (randShirt2 == 6) shirtColour2.color = Color.white;
        shirt.GetComponent<MeshRenderer>().material = shirtColour2;

        Material hatColour2 = new Material(shirt.GetComponent<MeshRenderer>().material);
        int randHat2 = UnityEngine.Random.Range(0, 3);
        if (randHat2 == 0) hatColour2.color = Color.red;
        if (randHat2 == 1) hatColour2.color = Color.blue;
        if (randHat2 == 2) hatColour2.color = Color.cyan;
        foreach (GameObject part in hatParts)
        {
            part.GetComponent<MeshRenderer>().material = hatColour2;
        }
    }


    private void DestroyFires()
    {
        var fires = GameObject.FindGameObjectsWithTag("Fire");
        foreach (var fire in fires)
        {
            Destroy(fire);
        }
    }


    private void SpawnFire()
    {
        float x = UnityEngine.Random.Range(-25, 25);
        float z = UnityEngine.Random.Range(-10, 8);
        Instantiate(firePrefab, new Vector3(x, 0, z), transform.rotation);
    }

}
