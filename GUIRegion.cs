using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    

    public abstract class GUIRegion
    {
        private int m_order = 0;

        private bool m_focused = false;
        public bool IsFocused { get { return m_focused; } }

        protected Vector4 m_rect;


        public GUIRegion(GUIForm form,int order = 0)
        {
            m_order = order;
        }


        internal void ProcessGUIEvent(RigelGUIEvent e)
        {

            GUIDraw.StartGUIRegion(this);

            OnGUI(e);

            GUIDraw.EndGUIRegion(this);

        }

        protected abstract void OnGUI(RigelGUIEvent e);

    }
}
