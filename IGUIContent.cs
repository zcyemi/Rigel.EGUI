using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public interface IGUIContent
    {
        void OnGUI(RigelGUIEvent e);
        IGUIView Region { get; set; }
    }

    public class GUISimpleContent : IGUIContent
    {
        public IGUIView Region { get; set; }
        
        private Action<RigelGUIEvent, IGUIView> m_drawFunction;

        public GUISimpleContent(Action<RigelGUIEvent,IGUIView> e)
        {
            m_drawFunction = e;
        }

        public void OnGUI(RigelGUIEvent e)
        {
            if (m_drawFunction != null) m_drawFunction.Invoke(e, Region);
        }
    }
}
