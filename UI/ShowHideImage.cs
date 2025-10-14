using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetHideImage : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    void Start()
    {
        if (rawImage.texture == null)
        {
            rawImage.color = Color.clear;
        }
    }
    public void SetImage(Texture texture)
    {
        rawImage.color = Color.white;
        rawImage.texture = texture;
    }
    public void HideImage()
    {
        rawImage.color = Color.clear;
    }
}
