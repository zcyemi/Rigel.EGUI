using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public abstract class GUIContent
    {
        public abstract void OnGUI(RigelGUIEvent e);
        public GUIView View { get; set; }
        public string ContentName = "Content";
    }

    /// <summary>
    /// Content With Basic background draw
    /// </summary>
    public class GUIRectContent : GUIContent
    {
        public override void OnGUI(RigelGUIEvent e)
        {
            GUI.RectAbsolute(View.Rect, GUIStyle.Current.ColorBackground);
        }
    }

    public class GUISimpleContent : GUIContent
    {
        private Action<RigelGUIEvent, GUIView> m_drawFunction;

        public GUISimpleContent(Action<RigelGUIEvent,GUIView> e)
        {
            m_drawFunction = e;
        }

        public override void OnGUI(RigelGUIEvent e)
        {
            if (m_drawFunction != null) m_drawFunction.Invoke(e, View);
        }
    }
}
