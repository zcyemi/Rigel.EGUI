using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    internal class GUIObjDropRect : GUIObjBase
    {

        public string Contract = null;
        public bool OnDropped = false;
        public object DropData = null;
        public object DropContext = null;

        public bool OnHover = false;

        //ab
        public Vector4 Rect;

        public override void Reset()
        {
            Rect = Vector4.zero;
            Contract = null;
            OnDropped = false;
            DropData = null;
            DropContext = null;
            OnHover = false;
        }


        internal bool CheckOver(Vector2 pointer)
        {
            if (GUIUtility.RectContainsCheck(Rect, pointer))
            {
                OnHover = true;
                return true;
            }
            return false;
        }

        public bool GetHoverStatus()
        {
            if (OnHover)
            {
                OnHover = false;
                return true;
            }
            return false;
        }

        public bool CheckDropped()
        {
            if (OnDropped)
            {
                OnDropped = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
