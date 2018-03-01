using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIContentDocker
    {
        private List<GUIContent> m_contents = new List<GUIContent>();

        internal void Draw(Vector4 rectab)
        {
            GUILayout.Label(rectab.ToString());
            GUI.BeginAreaAbsolute(rectab);
            GUILayout.Rect(new Vector2(rectab.z, 25), GUIStyle.Current.ColorBackgroundL2);

            GUI.EndArea();
        }

        public bool AddContent(GUIContent content)
        {
            if (m_contents.Contains(content)) return false;
            m_contents.Add(content);
            return true;
        }



    }
}
