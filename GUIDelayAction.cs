using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIDelayAction
    {

        private List<Action> m_actions = new List<Action>();

        public GUIDelayAction()
        {

        }

        public void Call(Action action)
        {
            m_actions.Add(action);
        }

        public void Invoke()
        {
            foreach(var m in m_actions)
            {
                m.Invoke();
            }
            m_actions.Clear();
        }
    }
}
