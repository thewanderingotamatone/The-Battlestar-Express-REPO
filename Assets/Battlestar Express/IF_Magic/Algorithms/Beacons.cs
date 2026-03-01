using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Beacon{
    public string id;
    public int strength;

    public Beacon(string _id, int _strength)
    {
        id = _id;
        strength = _strength;
    }
}

namespace Magic.Algorithms
{
    public class Beacons : Algorithm
    {

        // properties
        [Header("Properties")]
        public bool state;
        public bool mode;
        [ConditionalHide("mode")]
        public string id;
        public int strength;

        [Header("Parameters")]
        [SerializeField]
        private bool beaconManual = false;
        [SerializeField]
        [ConditionalHide("beaconManual")]
        private string beaconID = "";
        [SerializeField]
        [Range(0, 150)]
        private int beaconStrength = 50;
        [SerializeField]
        [Range(0, 5)]
        private int beaconTime = 1;
        [SerializeField]
        private bool beaconState = false;
        [Header("Functions")]
        [SerializeField]
        private bool sweep = false;
        [SerializeField]
        private bool empty = false;
        [SerializeField]
        private bool setMode = false;
        [SerializeField]
        private bool setStrength = false;
        [SerializeField]
        private bool setTime = false;
        [SerializeField]
        private bool setState = false;
       
        [Header("   ")]
        public List<Beacon> beacons;
       
        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();

            if (sweep){
                Sweep();
                sweep = false;
            }

            if (setState){
                if (state != beaconState){
                    SetState(beaconState);
                }
                setState = false;
            }

            if (setStrength){
                SetStrength(beaconStrength);
                setStrength = false;
            }

            if (setTime){
                SetTime(beaconTime);
                setTime = false;
            }

            if (setMode){
                SetMode(!beaconManual, beaconID);
                setMode = false;
            }

            if (empty){
                Empty();
                empty = false;
            }

            if (parsedData.Length > 1){

                string beaconRaw = parsedData[1];
                beaconRaw = beaconRaw.Substring(0, beaconRaw.Length - 2);

                string[] _properties = beaconRaw.Split(":", 4);

                if (_properties[0] == "1"){
                    state = true;
                } else {
                    state = false;
                }

                if (_properties[1] == "+"){
                    mode = true;
                } else {
                    mode = false;
                    if (_properties[1] == "-"){
                        id = "";
                    } else {
                        id = _properties[1];
                    }
                }
                int.TryParse( _properties[2], out strength);

                string[] _beaconData = _properties[3].Split(",");

                if (_beaconData.Length > 1){
                
                    if (mode){
                        int wiggle = _beaconData.Length-2; // TODO:

                        for (int i = 0; i <_beaconData.Length; i+=2){
                            int rssi = 0;
                            string data = "";

                            if (i >= wiggle){ // TODO:

                                if (_beaconData.Length > 1){
                                    data = _beaconData[i];
                                    int.TryParse( _beaconData[i+1], out rssi);
                                }
                            
                            } else {
                                data = _beaconData[i+1];
                                int.TryParse( _beaconData[i+2], out rssi);
                            }

                            rssi = -1 * rssi;

                            Beacon b = new Beacon(data, rssi);

                            // TODO:
                            if (!data.Substring(data.Length-2).Equals(",,")){
                                if (!beacons.Exists(x => x.id == data)) beacons.Add(b);
                            }
                        }
                    } else {
                        int wiggle = _beaconData.Length-6; // TODO:

                        string address = "";

                        for (int i = 0; i <_beaconData.Length; i+=6){
                            int rssi = 0;
                            string data = "";

                            if (i >= wiggle){ // TODO:

                                if (_beaconData.Length > 5){
                                    data = _beaconData[i] + "," + _beaconData[i+2] + "," + _beaconData[i+3] + "," + _beaconData[i+4] + "," + _beaconData[i+5];
                                    int.TryParse( _beaconData[i+1], out rssi);
                                    address = _beaconData[i+3];
                                }
                            
                            } else {
                                data = _beaconData[i+1] + "," + _beaconData[i+3] + "," + _beaconData[i+4] + "," + _beaconData[i+5];
                                int.TryParse( _beaconData[i+2], out rssi);
                                address = _beaconData[i+4];
                            }

                            rssi = -1 * rssi;

                            Beacon b = new Beacon(data, rssi);

                            if (!beacons.Exists(x => x.id.Contains(address))){
                                beacons.Add(b);
                            }
                        }
                    }
                }
            }
        }

        // tell hardware to scan for beacons
        public void Sweep(){
            base.Output("7,0\n");
        }

        // set between dynamic scan or constant scan
        public void SetState(bool state){
            if (state){
                base.Output("7,1,1\n");
            } else {
                base.Output("7,1,0");
            }
        }

        // set id to scan for or magic id
        public void SetMode(bool mode, string id = ""){

            if (mode){
                base.Output("7,2,+");
            } else {
                if (id.Length > 0){
                    string action = "7,2," + id;
                    base.Output(action);
                } else {
                    base.Output("7,2,-");
                }
            }

        }
        
        // set scan range
        public void SetStrength(int strength){
            string action = "7,3," + strength.ToString() + "\n";
            base.Output(action);
        }

        // set scan time
        public void SetTime(int time){
            string action = "7,4," + time.ToString() + "\n";
            base.Output(action);
        }

        public void Empty()
        {
            beacons.Clear();
        }
    }
}

