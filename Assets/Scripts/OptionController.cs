using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace optionSpace
{
    public class OptionController : MonoBehaviour
    {
        public static OptionController Instance {  get; private set; }

        //MainGame�Ő�������z��̗v�f��
        public int objectCountToSet = 0;

        public static int maxTurn = 0;

        public static int maxLife = 0;

        public static int maxPoint = 0;

        public float choiceTime = 60f;

        public float openTime = 60f;

        public bool clickNext = false;
        // Start is called before the first frame update
        void Start()
        {


            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            maxPoint = 10;

            maxTurn = 4;

            maxLife = 4;

            objectCountToSet = 8;
        }

        // Update is called once per frame
        void Update()
        {
            DataManager.Instance.objectCount = objectCountToSet;
        }
    }
}