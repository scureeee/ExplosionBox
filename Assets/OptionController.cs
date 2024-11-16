using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionController : MonoBehaviour
{
    //MainGameÇ≈ê∂ê¨Ç∑ÇÈîzóÒÇÃóvëfêî
    public int objectCountToSet = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        objectCountToSet = 12;
        DataManager.instance.objectCount = objectCountToSet;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
