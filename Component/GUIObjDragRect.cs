using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    internal class GUIObjDragRect : GUIObjBase
    {
        private GUIDragState m_dragStage = new GUIDragState();
        public GUIDragState DragStage { get
            {
                return m_dragStage;
            } }

        public override void Reset()
        {
            m_dragStage.Reset();
        }
    }
}
