using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUISimpleView : IGUIView
    {
        protected IGUIContent m_content;

        private GUIRegionBufferBlockInfo m_blockInfoRect;
        private GUIRegionBufferBlockInfo m_blockInfoText;
        protected Vector4 m_rect = new Vector4(0, 0, 200, 200);
        public Vector4 Rect { get { return m_rect; } }
        private bool m_overlayFocus = false;
        private int m_order = 0;
        private bool m_focused = false;


        public bool IsFocused { get { return m_focused; }set { m_focused = value; } }
        public int Order { get { return m_order; }set { m_order = value; } }


        public GUISimpleView(IGUIContent content)
        {
            m_content = content;
            m_content.Region = this;
        }


        public bool CheckFocused(RigelGUIEvent e)
        {
            return GUIUtility.RectContainsCheck(m_rect, e.Pointer) || m_overlayFocus;
        }

        public void Init(int order, GUIForm form)
        {
            m_order = order;
        }

        public void OnRegionEnd(IGUIBuffer bufferRect, IGUIBuffer bufferText)
        {
            GUI.EndArea();

            m_blockInfoRect.Count = bufferRect.Count - m_blockInfoRect.Start;
            m_blockInfoText.Count = bufferText.Count - m_blockInfoText.Start;
        }

        public void OnRegionStart(IGUIBuffer bufferRect, IGUIBuffer bufferText)
        {
            m_blockInfoRect.Count = bufferRect.Count;
            m_blockInfoText.Count = bufferText.Count;

            GUI.BeginArea(m_rect);
        }

        public virtual void ProcessGUIEvent(RigelGUIEvent e)
        {
            GUI.StartGUIRegion(this);

            OnGUI(e);

            GUI.EndGUIRegion(this);
        }

        protected virtual void OnGUI(RigelGUIEvent e)
        {
            if (m_content != null) m_content.OnGUI(e);
        }

        public void SetOverlayFocuse(bool focus)
        {
            m_overlayFocus = focus;
        }

        public void SetRect(Vector4 rect)
        {
            m_rect = rect;
        }
    }
}
