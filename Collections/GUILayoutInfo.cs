using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Collections
{
    internal struct GUILayoutInfo
    {
        public bool AlignHorizontal;
        /// <summary>
        /// relate to current area
        /// </summary>
        public Vector2 Offset;
        /// <summary>
        /// child object size
        /// </summary>
        public Vector3 SizeMax;
        /// <summary>
        /// remain area size from offset point.
        /// </summary>
        public Vector2 RemainSize;


        public void Reset()
        {
            AlignHorizontal = false;
            Offset = Vector2.zero;
            SizeMax = Vector3.zero;
        }
    }
}
