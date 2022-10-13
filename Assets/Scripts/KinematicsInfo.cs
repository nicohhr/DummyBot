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
        public bool isLocked { get { return _isLocked; } set { this._isLocked = value; }}

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

    }
}