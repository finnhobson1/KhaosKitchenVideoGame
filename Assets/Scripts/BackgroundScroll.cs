using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour
{
    private float XscrollSpeedPerSec; // = 4*22.5f
    private float YscrollSpeedPerSec = 40;
    //private float maxYsize = 2023f;
    private float maxXsize = 1174f;
    private float maxYsize = 2160f +100;


    //private float maxYsize = 1011.5f;
    //private float maxXsize = 587f;

    private float startY;
    private float startX;
    private float offsetY;
    private float offsetX;

    void Start ()
    {
        startY = transform.position.y;
        startX = transform.position.x;
        offsetY = 0f;
        offsetX = 0f;
        XscrollSpeedPerSec = (maxXsize * YscrollSpeedPerSec) / maxYsize;

    }

    void MoveBackground()
    {



        if (offsetY >= maxYsize)
        {
            offsetY -= maxYsize;
            offsetY += 10;
        }
         if (offsetX >= maxXsize)
        {
            offsetX -= maxXsize;
            offsetX += 5.19469f;
        }
        offsetY += YscrollSpeedPerSec * Time.deltaTime;
        offsetX += XscrollSpeedPerSec * Time.deltaTime;

        transform.position = new Vector2(startX + offsetX, startY + offsetY);

    }

    void Update ()
    {
        //if(updateCount%10 == 0){
            MoveBackground();
        //}
        //updateCount++;


    }
}
