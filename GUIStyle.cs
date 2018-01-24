using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIStyle
    {
        public static GUIStyle Current { get { return m_currentStyle; } }
        private static GUIStyle m_currentStyle = new GUIStyle();

        public Vector4 ColorActive = RigelColor.RGBA(28, 151, 234, 255);
        public Vector4 ColorActiveD = RigelColor.RGBA(0, 122, 204, 255);
        public Vector4 ColorSp = RigelColor.RGBA(14, 75, 117, 255);
        public Vector4 ColorDisabled = RigelColor.RGBA(42, 42, 42, 255);

        public Vector4 ColorBackground = RigelColor.RGBA(30, 30, 30, 255);
        public Vector4 ColorBackgroundL1 = RigelColor.RGBA(42, 42, 42, 255);
        public Vector4 ColorBackgroundL2 = RigelColor.RGBA(58, 58, 58, 255);
    }
}
