using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIContextDraw<T>
    {
        public Action<T> draw;
        public Vector4 AreaRect;

        public T content;
        public static GUIContextDraw<T> Make(T self,Action<T> t)
        {
            return new GUIContextDraw<T>
            {
                content = self,
                draw = t,
                AreaRect = GUI.CurAreaRect
            };
        }

        public void Draw(Object o)
        {
            GUI.BeginAreaAbsolute(AreaRect);

            draw.Invoke(content);

            GUI.EndArea();

        }
    }
}
