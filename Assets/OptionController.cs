using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionController : MonoBehaviour
{
    //MainGameÇ≈ê∂ê¨Ç∑ÇÈîzóÒÇÃóvëfêî
    public int objectCountToSet = 0;

    public static int maxTurn = 0;

    public static int maxLife = 0;

    public static int maxPoint = 0;
    // Start is called before the first frame update
    void Start()
    {
        maxLife = 7;

        maxTurn = 10;

        maxPoint = 10;
    }

    // Update is called once per frame
    void Update()
    {
        DataManager.Instance.objectCount = objectCountToSet;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
