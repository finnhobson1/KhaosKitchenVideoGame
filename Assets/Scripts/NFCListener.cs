using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

using System.Collections.Generic;
using System;
using System.Reflection;

public class NFCListener : MonoBehaviour
{
    public static string nfc = "none";
    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;


    public static string GetValue()
    {
        return nfc;
    }


    public static void SetValue(string val)
    {
        nfc = val;
    }


    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                // Create new NFC Android object
                mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); // Activities open apps
                mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
                sAction = mIntent.Call<String>("getAction"); // result are returned in the Intent object

                if (sAction == "android.nfc.action.TECH_DISCOVERED")
                {
                    print("NFC read");
                    Debug.Log("TAG DISCOVERED");
                    // Get ID of tag
                    AndroidJavaObject mNdefMessage = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
                    if (mNdefMessage != null)
                    {
                        byte[] payLoad = mNdefMessage.Call<byte[]>("getId");
                        string text = System.Convert.ToBase64String(payLoad);
                        nfc = text;
                    }
                }

                else
                {
                    nfc = "not found";
                }
                mActivity = null;
                mIntent = null;
                sAction = null;
            }

            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
                nfc = "error";
            }
        }
    }
}
