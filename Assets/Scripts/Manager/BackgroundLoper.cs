using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoper : MonoBehaviour
{
    public float speed = 10f;
    public Renderer bgRend;
    void Update()
    {
        bgRend.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0f);
    }
}
