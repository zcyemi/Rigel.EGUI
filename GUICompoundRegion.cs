using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUICompoundRegion : IGUIView
    {
        private Vector4 m_rect = new Vector4(0, 0, 400, 300);
        public Vector4 Rect { get { return m_rect; } }

        public bool IsFocused { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Order { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CheckFocused(RigelGUIEvent e)
        {
            throw new NotImplementedException();
        }

        public void Init(int order, GUIForm form)
        {
            throw new NotImplementedException();
        }

        public void OnGUI(RigelGUIEvent e)
        {
            throw new NotImplementedException();
        }

        public void OnRegionEnd(IGUIBuffer bufferRect, IGUIBuffer bufferText)
        {
            throw new NotImplementedException();
        }

        public void OnRegionStart(IGUIBuffer bufferRect, IGUIBuffer bufferText)
        {
            throw new NotImplementedException();
        }

        public void ProcessGUIEvent(RigelGUIEvent e)
        {
            throw new NotImplementedException();
        }

        public void SetOverlayFocuse(bool focus)
        {
            throw new NotImplementedException();
        }

        public void SetRect(Vector4 rect)
        {
            throw new NotImplementedException();
        }
    }
}
