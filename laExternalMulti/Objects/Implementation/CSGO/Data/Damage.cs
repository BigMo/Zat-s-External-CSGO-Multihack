using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data
{
    public class Damage
    {
        #region VARIABLES
        private string text;
        private Vector3 position;
        private Vector2 position2D;
        private long birth;
        private const float LIFETIME = 5000f;
        #endregion

        #region PROPERTIES
        public string Text { get { return text; } }
        public Vector3 Position { get { return position; } }
        public Vector2 Position2D { get { return position2D; } }
        public bool Alive { get { return (float)(DateTime.Now.Ticks - birth) / (float)TimeSpan.TicksPerMillisecond < LIFETIME; } }
        public float Multiplier
        {
            get 
            {
                float val = (float)(DateTime.Now.Ticks - birth) / (float)TimeSpan.TicksPerMillisecond;
                if (val >= LIFETIME)
                    return 0f;
                return 1f / LIFETIME * (LIFETIME - val);
            }
        }
        #endregion

        #region METHODS
        public Damage(string text, Vector3 position)
        {
            this.text = text;
            this.position = position;
            this.position2D = new Vector2(position.X,position.Y);
            this.birth = DateTime.Now.Ticks;
        }

        public void Update()
        {
            this.position.Z += 2f;
        }
        #endregion
    }
}
