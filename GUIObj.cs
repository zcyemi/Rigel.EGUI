using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public enum GUIObjType : byte
    {
        ScrollView = 1,
        DragRegion = 2,
        TextInput = 3,
        TabView = 4,
    }

    public abstract class GUIObjBase
    {
        public bool Checked = false;
        public abstract void Reset();
    }

    internal class GUIObjPool<T> where T : GUIObjBase, new()
    {
        public Dictionary<long, T> m_objects = new Dictionary<long, T>(8);
        private Stack<T> m_pool = new Stack<T>();


        public T Get(long hash, Action<T> createFunction = null)
        {
            if (m_objects.ContainsKey(hash))
            {
                var obj = m_objects[hash];
                obj.Checked = true;
                return m_objects[hash];
            }
            else
            {
                T obj = null;

                if (m_pool.Count == 0)
                {
                    obj = new T();
                }
                else
                {
                    obj = m_pool.Pop();
                }
                if (createFunction != null)
                {
                    createFunction(obj);
                }
                m_objects.Add(hash, obj);
                obj.Checked = true;
                return obj;
            }
        }

        public void OnFrame()
        {
            int count = m_objects.Count;
            if (count == 0) return;
            var keys = new List<long>(m_objects.Keys);

            foreach (var k in keys)
            {
                var obj = m_objects[k];

                if (!obj.Checked)
                {
                    m_objects.Remove(k);
                    obj.Reset();
                    m_pool.Push(obj);
                    continue;
                }
                obj.Checked = false;
            }
        }

    }
}
