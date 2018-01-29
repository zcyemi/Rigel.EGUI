using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIBufferArray<T> where T : struct
    {
        protected T[] m_data;

        public int Count { get; protected set; }
        protected int Capacity { get; private set; }

        public int ItemByte { get; private set; }
        public int SizeInByte
        {
            get
            {
                return Count * ItemByte;
            }
        }

        public int BufferSize { get; protected set; }

        protected float m_resizeScale;

        public GUIBufferArray(int capacity,float resizeScale = 2.0f)
        {
            m_data = new T[capacity];
            Capacity = capacity;

            m_resizeScale = resizeScale;

            ItemByte = Utility.SizeOf<T>();

            BufferSize = ItemByte * capacity;
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Dispose()
        {
            Count = 0;
            Capacity = 0;

            BufferSize = 0;

            m_data = null;
        }

        public T[] GetRawArray()
        {
            return m_data;
        }

        public void AddItem(T item)
        {
            if(Count == Capacity)
            {
                int newSize = Mathf.CeilToInt(Capacity * m_resizeScale);
                Resize(newSize);
            }
            m_data[Count] = item;
            Count++;
        }

        public void Resize(int newsize)
        {
            if (newsize <= Capacity) return;

            T[] newdata = new T[newsize];
            m_data.CopyTo(newdata, 0);
            m_data = newdata;
            Capacity = newsize;

            BufferSize = Capacity * ItemByte;
        }

    }
}
