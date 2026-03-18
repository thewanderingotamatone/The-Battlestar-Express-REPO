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
                string[] directions = { "NONE", "LEFT", "RIGHT", "UP", "DOWN" };
                parsedData[0] = directions[Mathf.RoundToInt(Mathf.PingPong((Time.time), 4))];
            }

            switch (parsedData[0])
            {
                case "NONE":
                    direction = GestureDirection.none;
                    break;
                case "LEFT":
                    direction = GestureDirection.left;
                    break;
                case "RIGHT":
                    direction = GestureDirection.right;
                    break;
                case "UP":
                    direction = GestureDirection.up;
                    break;
                case "DOWN":
                    direction = GestureDirection.down;
                    break;
                default:
                    direction = GestureDirection.none;
                    break;
            }


        }
    }
}

