using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Component;

namespace Rigel.GUI
{
    public static partial class GUI
    {
        //DragDropManager

        private static GUIObjPool<GUIObjDragRect> s_poolDragRect = new GUIObjPool<GUIObjDragRect>();
        private static GUIObjPool<GUIObjDropRect> s_poolDropRect = new GUIObjPool<GUIObjDropRect>();

        internal static GUIObjDragRect GetDragRect(Vector4 rect,Action<GUIObjDragRect> creationFunction = null)
        {
            return s_poolDragRect.Get(GUIUtility.GetHash(rect, GUIObjType.DragRect));
        }

        internal static GUIObjDropRect GetDropRect(Vector4 rectab,Action<GUIObjDropRect> creationFunction = null)
        {
            var ret = s_poolDropRect.Get(GUIUtility.GetHash(rectab, GUIObjType.DropRect), creationFunction);
            ret.Rect = rectab;
            return ret;
        }


        internal static bool HoverDrop(string contract,object content)
        {
            var pool = s_poolDropRect.m_objects;

            foreach (var o in pool.Values)
            {
                if (o.Contract != contract) continue;
                if (o.CheckOver(GUI.Event.Pointer))
                {
                    return true;
                }
            }


            return false;
        }

        internal static bool EmmitDrop(string contract, object content,object context)
        {

            var pool = s_poolDropRect.m_objects;

            foreach(var o in pool.Values)
            {
                if (o.Contract != contract) continue;
                if(o.CheckOver(GUI.Event.Pointer))
                {
                    o.OnDropped = true;
                    o.DropData = content;
                    o.DropContext = context;
                    return true;
                }
            }


            return false;
        }


    }
}
