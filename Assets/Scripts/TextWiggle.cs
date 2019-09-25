using UnityEngine;
using System.Collections;

public class TextWiggle : MonoBehaviour
{
   
    private float maxXDist = 80;
    private float maxYDist = 30;


    private float startY;
    private float startX;
    private float offsetY;
    private float offsetX;

    private bool movingUp;
    private bool movingRight;


    void Start()
    {
        startY = transform.position.y;
        startX = transform.position.x;
        offsetY = 0f;
        offsetX = 0f;
    }

    void MoveText()
    {
        if(movingUp){
            offsetY += 1.46F*Time.deltaTime * maxYDist;
            if(offsetY>=maxYDist){
                movingUp = false;
            }
        }else{
            offsetY -= Time.deltaTime * maxYDist;
            if (offsetY <= -maxYDist)
            {
                movingUp = true;
            }
        }

        if (movingRight)
        {
            offsetX += Time.deltaTime * maxXDist;
            if (offsetX >= maxXDist)
            {
                movingRight = false;
            }
        }
        else
        {
            offsetX -= Time.deltaTime * maxXDist;
            if (offsetX <= -maxXDist)
            {
                movingRight = true;
            }
        }

        transform.position = new Vector2(startX + offsetX, startY + offsetY);


    }

    void Update()
    {
        MoveText();

    }
}
