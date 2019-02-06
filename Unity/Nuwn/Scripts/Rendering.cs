using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rendering : MonoBehaviour
{
    public Behaviour[] stuffToDisable;
    public Renderer[] StuffTONotRender;

    // Use this for initialization
    void Start()
    {

        foreach (var item in stuffToDisable)
        {
            item.enabled = false;
        }

        foreach (var Item in StuffTONotRender)
        {
            Item.enabled = false;
        }

    }
}
