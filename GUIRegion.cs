using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    
    public struct GUIRegionBufferBlockInfo
    {
        public int Start;
        public int Count;
    }

    public abstract class GUIRegion
    {
        public GUIRegionBufferBlockInfo BlockInfoRect;
        public GUIRegionBufferBlockInfo BlockInfoText;

        public string DebugInfo = "GUIRegion";

        private int m_order = 0;
        private bool m_overlayFocus = false;

        public int Order
        {
            get { return m_order; }
            set { m_order = value; }
        }

        private bool m_focused = false;
        public bool IsFocused { get { return m_focused; } set { m_focused = value; } }

        protected Vector4 m_rect;
        public Vector4 Rect { get { return m_rect; } set { m_rect = value; } }


        public GUIRegion(GUIForm form,int order = 0)
        {
            m_order = order;
        }


        internal void ProcessGUIEvent(RigelGUIEvent e)
        {
            GUI.StartGUIRegion(this);

            OnGUI(e);

            GUI.EndGUIRegion(this);

        }

        protected abstract void OnGUI(RigelGUIEvent e);

        public virtual bool CheckFocused(RigelGUIEvent e)
        {
            return GUIUtility.RectContainsCheck(m_rect, e.Pointer) || m_overlayFocus;
        }

        public void SetOverlayFocuse(bool focus)
        {
            m_overlayFocus = focus;
        }

    }
}
