using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public enum GUIOptionType
    {
        Grid,
        Width,
        Height,
    }

    public class GUIOption
    {
        public static GUIOptionGrid Grid(float val)
        {
            return new GUIOptionGrid(val);
        }
        public static GUIOptionWidth Width(int w)
        {
            return new GUIOptionWidth(w);
        }
        public static GUIOptionHeight Height(int h)
        {
            return new GUIOptionHeight(h);
        }
    }

    public class GUIOption<T> : GUIOption
    {
        public GUIOptionType Type;
        public T Value;

        public GUIOption(GUIOptionType type, T value)
        {
            this.Type = type;
            this.Value = value;
        }
    }

    

    public class GUIOptionGrid : GUIOption<float>
    {
        public GUIOptionGrid(float value) : base(GUIOptionType.Grid, value)
        {

        }
    }
    public class GUIOptionWidth : GUIOption<int>
    {
        public GUIOptionWidth(int value) : base(GUIOptionType.Width, value)
        {

        }
    }
    public class GUIOptionHeight : GUIOption<int>
    {
        public GUIOptionHeight(int value) : base(GUIOptionType.Height, value)
        {

        }
    }


    public static class GUIOptionExtension
    {
        public static void GetOption<T1>(this GUIOption[] options, out T1 opt1) where T1 : GUIOption
        {
            foreach (var o in options)
            {
                if (o is T1)
                {
                    opt1 = (T1)o;
                    return;
                }
            }
            opt1 = null;
        }

        public static void GetOption<T1, T2>(this GUIOption[] options, out T1 opt1, out T2 opt2) where T1 : GUIOption where T2 : GUIOption
        {
            opt1 = null;
            opt2 = null;
            foreach (var o in options)
            {
                if (o is T1)
                {
                    opt1 = (T1)o;
                    if (opt2 != null) return;
                }
                else if (o is T2)
                {
                    opt2 = (T2)o;
                    if (opt1 != null) return;
                }
            }
        }

        public static void GetOption<T1, T2, T3>(this GUIOption[] options, out T1 opt1, out T2 opt2, out T3 opt3) where T1 : GUIOption where T2 : GUIOption where T3 : GUIOption
        {
            opt1 = null;
            opt2 = null;
            opt3 = null;
            foreach (var o in options)
            {
                if ((opt1 == null) && o is T1)
                {
                    opt1 = (T1)o;
                    if (opt2 != null && opt3 != null) return;
                }
                else if ((opt2 == null) && o is T2)
                {
                    opt2 = (T2)o;
                    if (opt1 != null && opt3 != null) return;
                }
                else if (opt3 == null && o is T3)
                {
                    opt3 = (T3)o;
                    if (opt1 != null && opt2 != null) return;
                }
            }
        }
    }
}
