using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    public RawImage panel;
    public GameObject colourPanel;
    public Text R, O, Y, G, B;
    public bool red, orange, yellow, green, blue;
    public Player player;

    // Use this for initialization
    void Start()
    {
        InitCamera();
    }

    void InitCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name);
            }
        }

        if (backCam == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        backCam.Play();
        panel.texture = backCam;

        camAvailable = true;
    }

    private void OnDisable()
    {
        if (camAvailable) backCam.Stop();
        camAvailable = false;
        red = false;
        blue = false;
        green = false;
        orange = false;
        yellow = false;
    }

    private void OnEnable()
    {
        InitCamera();
        red = false;
        blue = false;
        green = false;
        orange = false;
        yellow = false;
    }


    // Update is called once per frame
    void Update()
    {
        red = false;
        blue = false;
        green = false;
        orange = false;
        yellow = false;

        if (camAvailable)
        {
            //int pixelCount = 0;
            int redCount = 0;
            int orangeCount = 0;
            int yellowCount = 0;
            int greenCount = 0;
            int blueCount = 0;

            Color[] pixels = backCam.GetPixels();

            // Count number of pixels of selected colour in the WebCamTexture
            switch(player.cameraColour)
            {
                case 0:
                    foreach (Color pixel in pixels)
                    {
                        if (pixel.r > 0.7 && pixel.g < 0.3 && pixel.b < 0.3) redCount++;
                    }
                    R.text = redCount.ToString();
                    if (redCount > 10000) red = true;
                    break;
                case 1:
                    foreach (Color pixel in pixels)
                    {
                        if (pixel.r > 0.7 && pixel.g > 0.3 && pixel.g < 0.5 && pixel.b < 0.3) orangeCount++;
                    }
                    O.text = orangeCount.ToString();
                    if (orangeCount > 10000) orange = true;
                    break;
                case 2:
                    foreach (Color pixel in pixels)
                    {
                        if (pixel.r > 0.6 && pixel.g > 0.6 && pixel.b < 0.2) yellowCount++;
                    }
                    Y.text = yellowCount.ToString();
                    if (yellowCount > 10000) yellow = true;
                    break;
                case 3:
                    foreach (Color pixel in pixels)
                    {
                        if (pixel.r < 0.6 && pixel.g > 0.6 && pixel.b < 0.4) greenCount++;
                    }
                    G.text = greenCount.ToString();
                    if (greenCount > 5000) green = true;
                    break;
                case 4:
                    foreach (Color pixel in pixels)
                    {
                        if (pixel.r < 0.3 && pixel.g < 0.5 && pixel.b > 0.6) blueCount++;
                    }
                    B.text = blueCount.ToString();
                    if (blueCount > 5000) blue = true;
                    break;
                default:
                    Debug.Log("No Colour Chosen");
                    break;
            }
          
        }
    }
}
