using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIMultiView : GUIView
    {
        private GUIContent m_layoutContent;

        private GUIContent m_contentLeft;
        private GUIContent m_contentRight;

        private Vector4 m_rectLeft;
        private Vector4 m_rectRight;

        protected List<GUIContent> m_contents = new List<GUIContent>();

        private List<GUIRegionBufferBlockInfo> m_blockRect = new List<GUIRegionBufferBlockInfo>();
        private List<GUIRegionBufferBlockInfo> m_blockText = new List<GUIRegionBufferBlockInfo>();
        

        public GUIMultiView(GUIContent mainContent):base()
        {
            m_layoutContent = new GUISimpleContent(GUIDrawFunc);

            SetContent(m_layoutContent);

            m_contentLeft = mainContent;
            m_contentRight = new GUISimpleContent(GUIDrawTestContent);

            m_contents.Add(m_contentLeft);
            m_contents.Add(m_contentRight);
        }

        public void AddContent(GUIContent content)
        {
            m_contents.Add(content);
            m_blockRect.Add(new GUIRegionBufferBlockInfo());
            m_blockText.Add(new GUIRegionBufferBlockInfo());
        }

        public void RemoveContent(GUIContent content)
        {
            if (!m_contents.Contains(content)) return;
            int index = m_contents.IndexOf(content);
            m_contents.RemoveAt(index);
            m_blockRect.RemoveAt(index);
            m_blockText.RemoveAt(index);
        }

        public override bool CheckFocused(RigelGUIEvent e)
        {
            bool focused = base.CheckFocused(e);

            return focused;
        }

        private void GUIDrawTestContent(RigelGUIEvent e,GUIView view)
        {
            GUI.Rect(view.Rect, RigelColor.Green);
        }

        private void GUIDrawFunc(RigelGUIEvent e,GUIView view)
        {
            GUI.RectAbsolute(m_rect, IsFocused ? RigelColor.Red : RigelColor.Blue);

            m_rectLeft = view.Rect.Padding(20);
            m_rectLeft.z *= 0.5f;

            m_rectRight = m_rectLeft;
            m_rectRight.x += m_rectRight.z;

            GUI.RectAbsolute(m_rectLeft, RigelColor.White);

            GUI.RectAbsolute(m_rectRight, RigelColor.Green);

        }

        public sealed override void ProcessGUIEvent(RigelGUIEvent e)
        {
            for(int i=0;i< m_contents.Count;i++)
            {
                var content = m_contents[i];


            }
        }
    }
}
