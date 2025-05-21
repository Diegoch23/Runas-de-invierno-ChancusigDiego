using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraTobias : MonoBehaviour
{
    public GameObject Tobias;
 
   
    void Update()
    {
        Vector3 position = transform.position;
        position.x = Tobias.transform.position.x;
        transform.position = position;
    }
}
