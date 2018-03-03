using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public enum DropRectStatus
    {
        None,
        OnHover,
        OnDrop,
    }

    public class GUIDropRectInfo
    {
        public DropRectStatus Staus;
        public object Target;
        public object TargetContext;

        public void Reset()
        {
            Staus = DropRectStatus.None;
            Target = null;
            TargetContext = null;
        }
    }


    internal class GUIObjDropRect : GUIObjBase
    {
        internal GUIDropRectInfo m_info = new GUIDropRectInfo();
        internal GUIDropRectInfo Info { get
            {
                return m_info;
            } }
        public string Contract = null;

        //Absolute
        public Vector4 Rect;

        public override void Reset()
        {
            m_info.Reset();

            Rect = Vector4.zero;
            Contract = null;
        }


        internal bool CheckOver(Vector2 pointer)
        {
            if (GUIUtility.RectContainsCheck(Rect, pointer))
            {
                return true;
            }
            return false;
        }

        public void SetStatus(DropRectStatus status,object target = null,object context = null)
        {
            m_info.Staus = status;
            m_info.Target = target;
            m_info.TargetContext = context;
        }

        public bool GetHoverStatus()
        {
            if (m_info.Staus == DropRectStatus.OnHover)
            {
                m_info.Staus = DropRectStatus.None;
                return true;
            }
            return false;
        }

    }
}
