using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class BaumelScript : MonoBehaviour
{
    public List<Transform> angels;
    public float frequency;

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(angels[0].rotation, angels[1].rotation,
            (Mathf.Sin(frequency * Time.time) + 1) / 2);
    }
}