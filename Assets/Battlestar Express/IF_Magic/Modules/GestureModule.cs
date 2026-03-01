using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class GestureModule : Module
    {
        // property
        public enum GestureDirection { none, up, down, left, right };
        [Header("Properties")]
        [SerializeField] public GestureDirection direction = GestureDirection.none;

        public override void Start()
        {
            moduleNumber = 3;
            dataType = DataTypes.String;

            dataLength = 1;

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                string[] directions = { "0", "3", "4", "1", "2" };
                parsedData[0] = directions[Mathf.RoundToInt(Mathf.PingPong((Time.time), 4))];
            }
            //Debug.Log(parsedData[0]);

            switch (parsedData[0])
            {
                case "0":
                    direction = GestureDirection.none;
                    break;
                case "4":
                    direction = GestureDirection.left;
                    break;
                case "3":
                    direction = GestureDirection.right;
                    break;
                case "2":
                    direction = GestureDirection.up;
                    break;
                case "1":
                    direction = GestureDirection.down;
                    break;
                default:
                    direction = GestureDirection.none;
                    break;
            }


        }
    }
}

