using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public static class GUIDraw
    {
        #region State

        private static GUILayer m_layer = null;
        private static GUIRegion m_region = null;
        private static IGUIBuffer BufRect { get; set; }
        private static IGUIBuffer BufText { get; set; }

        internal static void StartGUIRegion(GUIRegion region)
        {
            if (m_layer == null) throw new Exception();
            if (m_region != null) throw new Exception();

            m_region = region;

            //Process Buffer and Offset
            BufRect = m_layer.GetBufferRect(m_region);
            BufText = m_layer.GetBufferText(m_region);

        }
        internal static void EndGUIRegion(GUIRegion region)
        {
            BufRect = null;
            BufText = null;

            if (m_region != region) throw new Exception();
            m_region = null;
        }

        internal static void StartGUILayer(GUILayer layer)
        {
            if (m_layer != null) throw new Exception();
            m_layer = layer;
        }
        internal static void EndGUILayer(GUILayer layer)
        {
            if (m_layer != layer) throw new Exception();
            m_layer = null;
        }

#endregion

        public static void Rect(Vector4 rect,Vector4 color)
        {
            BufRect.AddVertices(new Vector4(rect.x, rect.y, 0.5f, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x, rect.y, 0.5f, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x, rect.y, 0.5f, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x, rect.y, 0.5f, 1), color, Vector2.zero);
        }
    }
}
