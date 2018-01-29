using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    /// <summary>
    /// Inversed array for efficient depth test in graphics rendering.
    /// </summary>
    public class GUIBufferInverseArray<T> where T:struct
    {
        protected T[] m_data;

        public int Count { get; protected set; }
        protected int Pos;

        protected int Capacity { get; private set; }

        public int ItemByte { get; private set; }
        public int SizeInByte
        {
            get
            {
                return Count * ItemByte;
            }
        }

        protected float m_resizeScale;

        public GUIBufferInverseArray(int capacity, float resizeScale = 2.0f)
        {
            Count = 0;
            Pos = capacity - 1;

            m_data = new T[capacity];
            Capacity = capacity;
            m_resizeScale = resizeScale;

            ItemByte = Utility.SizeOf<T>();
        }

        public void Clear()
        {
            Count = 0;
            Pos = Capacity - 1;
        }

        public void Dispoe()
        {
            Count = 0;
            Capacity = 0;
            Pos = 0;
            m_data = null;
        }

        public T[] GetRawArray()
        {
            return m_data;
        }

        public void AddItem(T item)
        {
            if (Count == Capacity)
            {
                int newSize = Mathf.CeilToInt(Capacity * m_resizeScale);
                Resize(newSize);
            }
            m_data[Pos] = item;
            Pos--;
            Count++;
        }

        public void Resize(int newsize)
        {
            if (newsize <= Capacity) return;

            T[] newdata = new T[newsize];
            Array.Copy(m_data, Pos + 1, newdata, newsize - Count, Count);

            m_data = newdata;
            Capacity = newsize;
        }
    }
}
