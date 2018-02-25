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
        public string DroppedInfo = null;
        public Action<Vector4> OnDropOver = null;
        //ab
        public Vector4 Rect;

        public override void Reset()
        {
            Rect = Vector4.zero;
            Contract = null;
            OnDropped = false;
            DroppedInfo = null;
        }


        public bool CheckOver(Vector2 pointer)
        {
            if (GUIUtility.RectContainsCheck(Rect, pointer))
            {
                if (OnDropOver != null) OnDropOver.Invoke(Rect);
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
