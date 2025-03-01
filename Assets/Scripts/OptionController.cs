using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace optionSpace
{
    public class OptionController : MonoBehaviour
    {
        public static OptionController Instance {  get; private set; }

        //MainGameで生成する配列の要素数
        public int objectCountToSet = 0;

        public static int maxTurn = 0;

        public static int maxLife = 0;

        public static int maxPoint = 0;

        public float choiceTime = 60f;

        public float openTime = 60f;

        public bool clickNext = false;

        public bool canselTime = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            maxPoint = 18;

            maxTurn = 10;

            maxLife = 2;

            objectCountToSet = 8;
        }
    }
}