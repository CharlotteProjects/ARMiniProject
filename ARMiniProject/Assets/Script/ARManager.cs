using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour
{
    [SerializeField] GameObject backGroup = null;
    [SerializeField] GameObject bodyGroup = null;

    // Start is called before the first frame update
    void Start()
    {
        backGroup.setActive(true);
        bodyGroup.setActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
