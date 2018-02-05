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

    public interface IGUIView
    {

        void Init(int order, GUIForm form);
        
        bool CheckFocused(RigelGUIEvent e);
        /// <summary>
        /// When set overlay focus, this region will not lose focuse if mouse pointer failed to contains check.
        /// </summary>
        /// <param name="focus"></param>
        void SetOverlayFocuse(bool focus);

        bool IsFocused { get; set; }
        int Order { get; set; }
        Vector4 Rect { get; }

        void SetRect(Vector4 rect);


        void OnRegionStart(IGUIBuffer bufferRect,IGUIBuffer bufferText);
        void ProcessGUIEvent(RigelGUIEvent e);
        void OnRegionEnd(IGUIBuffer bufferRect, IGUIBuffer bufferText);

    }
}
