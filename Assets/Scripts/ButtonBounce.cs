using UnityEngine;
using System.Collections;

public class ButtonBounce : MonoBehaviour
{
    public float startWidth;
    public float startHeight;

    public float widthBounceDist = 30;
    public float heightBounceDist = 15;

    public float widthOffset;
    public float heightOffset;


    private bool wExpand;
    private bool hExpand;
    private bool wShrink = false;
    private bool hShrink = false;

    public bool widthBounceFirst;


    // Use this for initialization
    void Start()
    {
        startWidth = GetComponent<RectTransform>().rect.width;
        startHeight = GetComponent<RectTransform>().rect.height;

        wExpand = widthBounceFirst;
        hExpand = !widthBounceFirst;

        widthOffset = 0f;
        heightOffset = 0f;

    }

    void IncreaseWidth()
    {
        widthOffset += widthBounceDist*Time.deltaTime;
        if(widthOffset>widthBounceDist){
            wExpand = false;
            wShrink = true;
        }
    }

    void DecreaseWidth()
    {
        widthOffset -= widthBounceDist*Time.deltaTime;
        if(widthOffset<0){
            wShrink = false;
            hExpand = true;
            widthOffset = 0;
        }
    }

    void IncreaseHeight()
    {
        heightOffset += heightBounceDist*Time.deltaTime;
        if(heightOffset>heightBounceDist){
            hExpand = false;
            hShrink = true;
        }
    }

    void DecreaseHeight()
    {
        heightOffset -= heightBounceDist*Time.deltaTime;
        if(heightOffset<0){
            hShrink = false;
            wExpand = true;
            heightOffset = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(wExpand){
            IncreaseWidth();
        }
        else if(wShrink){
            DecreaseWidth();
        }
        else if(hExpand){
            IncreaseHeight();
        }
        else if(hShrink){
            DecreaseHeight();
        }

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth + widthOffset, startHeight + heightOffset);


    }
}
