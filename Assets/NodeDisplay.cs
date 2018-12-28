using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeDisplay : MonoBehaviour
{
    public Text Text;

    public int Id;

    void Start()
    {
        Text.text = Id.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
