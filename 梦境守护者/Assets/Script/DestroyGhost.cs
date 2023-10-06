using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGhost : MonoBehaviour
{
    [SerializeField] float destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
