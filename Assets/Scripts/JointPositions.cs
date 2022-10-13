using JetBrains.Annotations;
using System.Collections;
using System.Data;
using UnityEngine;

namespace Assets.Scripts
{
    public class JointPositions
    {
        // Variaveis privadas
        private int _jointNumber;
        private float[] _positions;
        private bool _isLocked;

        // Variaveis publicas
        public int jointNumber { get { return this._jointNumber; } set { this._jointNumber = value; } }
        public float[] positions 
        { 
            get { return this._positions; }
            set { if (this._isLocked) this._positions = value; } 
        }
        public bool isLocked { get { return this._isLocked; } set { this._isLocked = value; } }

        public JointPositions()
        {
            this._isLocked = false;
            this._jointNumber = 4;
            this._positions = new float[_jointNumber];
            for (int i = 0; i < this._jointNumber; i++) { this._positions[i] = 90.0f; }
        }

    }
}