using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Collections;

namespace Rigel.GUI
{
    internal class GUIFrame
    {
        public Stack<GUIAreaInfo> AreaStack = new Stack<GUIAreaInfo>();
        public Stack<GUILayoutInfo> LayoutStack = new Stack<GUILayoutInfo>();
        public bool OnDragDrop = false;

        public Vector4 RootRect;


        public void Reset(GUIForm form,RigelGUIEvent e)
        {
            AreaStack.Clear();
            LayoutStack.Clear();

            RootRect = form.Rect;

            if(e.EventType == RigelGUIEventType.MouseDragLeave)
            {
                OnDragDrop = false;
            }
            else if(e.EventType == RigelGUIEventType.MouseDragUpdate)
            {
                if (OnDragDrop)
                {
                    OnDragDrop = true;
                }
            }
        }

        /// <summary>
        /// return false when verfy failed.
        /// </summary>
        /// <returns></returns>
        public bool EndFrame()
        {
            if (AreaStack.Count != 0) return false;

            return true;
        }
    }
}
