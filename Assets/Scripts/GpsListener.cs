using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GpsListener : MonoBehaviour
{
    
    public static GpsListener Instance { set; get; }

    public static float latitude;

    public static float longitude;

    public static string latString = "unAssigned";

    public static string longString = "Unassigned";
    
    private bool isGpsReady;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine((StartLocationService()));
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            latString = "Gps disabled";
            Debug.Log("User has not enabled gps");
            yield break;
        }
        Input.location.Start();
        int maxWait = 0;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait < 100)
        {
            latString = "init " + Convert.ToString(maxWait);
            maxWait++;
            yield return new WaitForSeconds(1);
            
        }

        if (maxWait >= 100)
        {
            Debug.Log("Timed out");
                yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            latString = "failed";
            Debug.Log("Unable to determine location of device");
            yield break;
        }

       // isGpsReady = true;
        StartCoroutine(updateGPS());

        yield break;
    }

    // Update is called once per frame
    IEnumerator updateGPS()
    {
        float UPDATE_TIME = 1f; //Every  3 seconds
        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            latString = Convert.ToString(latitude);
            longString = Convert.ToString(longitude);
            yield return updateTime;
        }
    }
        
      
            

    }

