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

        public bool OnHover = false;

        //ab
        public Vector4 Rect;

        public override void Reset()
        {
            Rect = Vector4.zero;
            Contract = null;
            OnDropped = false;
            DropData = null;
            OnHover = false;
        }


        public bool CheckOver(Vector2 pointer)
        {
            if (GUIUtility.RectContainsCheck(Rect, pointer))
            {
                OnHover = true;
                return true;
            }
            return false;
        }

        public void Draw(Action onhoverdraw)
        {
            if (OnHover)
            {
                if(onhoverdraw != null)
                {
                    onhoverdraw.Invoke();
                }
                OnHover = false;
            }
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
