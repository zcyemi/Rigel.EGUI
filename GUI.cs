using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Collections;

namespace Rigel.GUI
{
    public static partial class GUI
    {

        /// v0                v1
        /// +-----------------+
        /// |                 |
        /// |                 |
        /// +-----------------+
        /// v4                v3
        ///
        public static void Rect(Vector4 rect, Vector4 color)
        {
            rect = rect.Move(CurArea.Rect);
            RectAbsolute(rect, color);
        }

        public static void RectAbsolute(Vector4 rect,Vector4 color)
        {
            BufRect.AddVertices(new Vector4(rect.x, rect.y, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x + rect.z, rect.y, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x + rect.z, rect.y + rect.w, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x, rect.y + rect.w, DepthValue, 1), color, Vector2.zero);

            DepthValue -= DepthStep;
        }

        public static void Char(Vector4 rect,Vector4 color,char c)
        {

            BufText.AddVertices(new Vector4(rect.x, rect.y, DepthValue, 1), color, Vector2.zero);
            BufText.AddVertices(new Vector4(rect.x + rect.z, rect.y, DepthValue, 1), color, new Vector2(1,0));
            BufText.AddVertices(new Vector4(rect.x + rect.z, rect.y + rect.w, DepthValue, 1), color, new Vector2(1, 1));
            BufText.AddVertices(new Vector4(rect.x, rect.y + rect.w, DepthValue, 1), color, new Vector2(0, 1));

            DepthValue -= DepthStep;
        }

        public static void BeginArea(Vector4 rect)
        {
            CurArea = new GUIAreaInfo()
            {
                Rect = rect.Move(CurArea.Rect.Pos()),
            };
            Frame.AreaStack.Push(CurArea);
            Frame.LayoutStack.Push(CurLayout);

            CurLayout.RectSize = CurArea.Rect.Size() - CurLayout.Offset;
            CurLayout.Reset();
            
        }

        public static void EndArea()
        {
            Frame.AreaStack.Pop();
            CurLayout = Frame.LayoutStack.Pop();

            if (Frame.AreaStack.Count == 0)
            {
                CurArea.Rect = Frame.RootRect;
            }
            else
            {
                CurArea = Frame.AreaStack.Peek();
            }
        }

    }
}
