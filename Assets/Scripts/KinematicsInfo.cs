using JetBrains.Annotations;
using System.Collections;
using System.Data;
using UnityEngine;

namespace Assets.Scripts
{
    public class KinematicsInfo
    {

        private Vector3 _position;
        private bool _isLocked;

        public Vector3 position { get { return _position; } set { if (this._isLocked) this._position = value; } }
        public bool isLocked { get { return _isLocked; } set { this._isLocked = value; } }

        public void Set(float x, float y, float z)
        {
            this._position.x = x;
            this._position.y = y;
            this._position.z = z;
            //_position = new Vector3(x, y, z);
        }

        public KinematicsInfo()
        {
            this._isLocked = false;
            this._position = new Vector3();
            this._position.Set(0, 0, 0);
        }

        public float[] positions
        {
            get
            {
                float[] result = new float[3];
                result[0] = _position.x;
                result[1] = _position.y;
                result[2] = _position.z;

                return result;
            }

            set
            {
                Set(value[0], value[1], value[2]);
            }
        }

        public void setPosition(float position, int axisIndex)
        {
            switch (axisIndex)
            {
                case 0:
                    this._position.x = position;    
                    break;
                case 1:
                    this._position.y = position;
                    break;
                case 2:
                    this._position.z = position;
                    break;
            }
        }
        
    }
}