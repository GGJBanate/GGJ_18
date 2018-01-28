using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Light))]
public class BaumelScript : MonoBehaviour
{
    public float frequency;

    public List<Transform> angels;

    void Update()
    {
        transform.rotation = Quaternion.Lerp(angels[0].rotation, angels[1].rotation, (Mathf.Sin(frequency * Time.time) + 1) / 2);
    }
}
