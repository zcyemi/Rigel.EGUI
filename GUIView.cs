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

    public class GUIView
    {
        protected GUIContent m_content;

        private GUIRegionBufferBlockInfo m_blockInfoRect;
        private GUIRegionBufferBlockInfo m_blockInfoText;
        protected Vector4 m_rect = new Vector4(0, 0, 200, 200);
        public Vector4 Rect { get { return m_rect; } }
        private bool m_overlayFocus = false;
        private int m_order = 0;
        private bool m_focused = false;


        public bool IsFocused { get { return m_focused; }set { m_focused = value; } }
        public int Order { get { return m_order; }set { m_order = value; } }


        public GUIView(GUIContent content)
        {
            m_content = content;
            m_content.View = this;
        }

        public GUIView()
        {

        }

        public void SetContent(GUIContent content)
        {
            m_content = content;
            m_content.View = this;
        }


        public virtual bool CheckFocused(RigelGUIEvent e)
        {
            return GUIUtility.RectContainsCheck(m_rect, e.Pointer) || m_overlayFocus;
        }

        public void Init(int order, GUIForm form)
        {
            m_order = order;
        }

        public virtual void OnRegionEnd(IGUIBuffer bufferRect, IGUIBuffer bufferText)
        {
            GUI.EndArea();

            m_blockInfoRect.Count = bufferRect.Count - m_blockInfoRect.Start;
            m_blockInfoText.Count = bufferText.Count - m_blockInfoText.Start;
        }

        public virtual void OnRegionStart(IGUIBuffer bufferRect, IGUIBuffer bufferText)
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
