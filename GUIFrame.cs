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

        public Vector4 RootRect;


        public void Reset(GUIForm form)
        {
            AreaStack.Clear();
            LayoutStack.Clear();

            RootRect = form.Rect;
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
