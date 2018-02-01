using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public enum GUIScrollType : byte
    {
        Vertical = 1,
        Horizontal = 2,
        All = 3
    }

    public class GUIObjScrollView : GUIObjBase
    {
        public override void Reset()
        {

        }

        public Vector2 Draw(Vector4 rectab,Vector2 pos,GUIScrollType scrolltype)
        {
            return pos;
        }
    }
}
