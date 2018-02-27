using System;

using Rigel.GUI.Collections;
using Rigel.GUI.Component;

namespace Rigel.GUI
{
    public static partial class GUI
    {

        private static GUIForm m_form = null;
        private static GUILayer m_layer = null;
        private static GUIView m_view = null;

        private static IGUIBuffer BufRect { get; set; }
        private static IGUIBuffer BufText { get; set; }

        private static int DepthBase;
        private static int DepthLevel = 0;
        private static float DepthValue = 0;
        private static readonly float DepthStep = 0.0001f;

        public static GUIForm Form { get { return m_form; } }


        internal static GUIFrame m_frame;
        private static GUIFrame Frame { get { return m_frame; } }

        public static GUIView CurRegion { get { return m_view; } }

        internal static GUIAreaInfo CurArea;
        internal static GUILayoutInfo CurLayout;

        public static Vector4 CurAreaRect
        {
            get { return CurArea.Rect; }
        }
        public static Vector4 CurLayoutOffset
        {
            get { return CurLayout.Offset; }
        }

        internal static IFontInfo Font { get; private set; }

        public static RigelGUIEvent Event { get; private set; }
        public static bool RegionIsFocused {
            get
            {
                if (m_view == null) return false;
                return m_view.IsFocused;
            }
        }

        //ObjPool
        [TODO("Refactoring","Move to form for multiple form draw")]
        private static GUIObjPool<GUIObjTabView> s_poolTabView = new GUIObjPool<GUIObjTabView>();
        private static GUIObjPool<GUIObjScrollView> s_poolScrollView = new GUIObjPool<GUIObjScrollView>();
        private static GUIObjPool<GUIObjMenuDraw> s_poolMenuDraw = new GUIObjPool<GUIObjMenuDraw>();


        internal static GUIObjTabView GetObjTabView(Vector4 rect, Action<GUIObjTabView> createFunction = null) 
        {
            return s_poolTabView.Get(GUIUtility.GetHash(rect, GUIObjType.TabView), createFunction);
        }

        internal static GUIObjScrollView GetObjScrollView(Vector4 rect,Action<GUIObjScrollView> createFuction = null)
        {
            return s_poolScrollView.Get(GUIUtility.GetHash(rect, GUIObjType.ScrollView), createFuction);
        }
        internal static GUIObjMenuDraw GetObjMenuDraw(int menuhash,Vector4 rect,Action<GUIObjMenuDraw> createFunction = null)
        {
            return s_poolMenuDraw.Get(GUIUtility.GetHash(menuhash,rect, GUIObjType.MenuDraw),createFunction);
        }




        /////////////
        //Utilities
        public static Vector4 GetAbsoluteRect(Vector4 rect)
        {
            return rect.Move(GUI.CurArea.Rect.Pos());
        }

        internal static void StartFrame(GUIForm form,RigelGUIEvent e)
        {
            if (m_form != null) throw new Exception();
            m_form = form;
            Event = e;

            m_frame = m_form.Frame;
            m_frame.Reset(m_form);

            Font = m_form.GraphicsBind.FontInfo;

            s_poolTabView.OnFrame();
            s_poolScrollView.OnFrame();
            s_poolMenuDraw.OnFrame();
            s_poolDragRect.OnFrame();
            s_poolDropRect.OnFrame();
        }
        internal static bool EndFrame(GUIForm form)
        {
            if (m_form != form) throw new Exception();
            m_form = null;
            Font = null;

            bool eventUsed = Event.Used;
            Event = null;

            if (!m_frame.EndFrame())
            {
                throw new Exception();
            }
            m_frame = null;

            return eventUsed;
        }

        public static void BeginArea(Vector4 rect,bool clip = false)
        {
            CurArea = new GUIAreaInfo()
            {
                Rect = rect.Move(CurArea.Rect.Pos()).Truncate(),
                Clip = clip
            };
            CurArea.ContentMax = CurArea.Rect.Size();
            Frame.AreaStack.Push(CurArea);
            Frame.LayoutStack.Push(CurLayout);

            CurLayout.RemainSize = CurArea.Rect.Size() - CurLayout.Offset;
            CurLayout.Reset();
        }

        public static void BeginAreaAbsolute(Vector4 rect, bool clip = false)
        {
            CurArea = new GUIAreaInfo()
            {
                Rect = rect.Truncate(),
                Clip = clip
            };
            CurArea.ContentMax = CurArea.Rect.Size();
            Frame.AreaStack.Push(CurArea);
            Frame.LayoutStack.Push(CurLayout);

            CurLayout.RemainSize = CurArea.Rect.Size() - CurLayout.Offset;
            CurLayout.Reset();
        }

        public static void EndArea()
        {
            Frame.AreaStack.Pop();
            CurLayout = Frame.LayoutStack.Pop();

            if (Frame.AreaStack.Count == 0)
            {
                CurArea.Rect = Frame.RootRect;
            }
            else
            {
                CurArea = Frame.AreaStack.Peek();
            }
        }

        
        internal static void SetDrawBuffer(IGUIBuffer rectbuffer,IGUIBuffer textbuffer)
        {
            BufRect = rectbuffer;
            BufText = textbuffer;
        }

        internal static void StartGUIView(GUIView view)
        {
            if (m_layer == null) throw new Exception();
            if (m_view != null) throw new Exception();

            m_view = view;

            DepthBase = m_layer.Order - m_view.Order;
            DepthLevel = 0;
            DepthValue = DepthBase;

            m_view.OnViewStart();

        }
        internal static void EndGUIView(GUIView view)
        {
            if (m_view != view) throw new Exception();

            m_view.OnViewEnd();

            m_view = null;
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

        public static float GetDepth(int increaseStep = 0)
        {
            var ret = DepthValue;
            DepthValue -= increaseStep * DepthStep;
            return DepthValue;
        }

        public static float SetDepth(float depth)
        {
            var lastDepth = DepthValue;
            DepthValue = depth;
            return lastDepth;
        }

        public static int SetDepthLevel(int level = 0)
        {
            var ret = DepthLevel;
            DepthValue += DepthLevel * 0.1f;
            DepthLevel = level;
            DepthValue -= DepthLevel * 0.1f;
            return ret;
        }


        public static int SetDepthLayer(GUILayerType  layer)
        {
            int offset = GUI.CurRegion.Layer.LayerType - layer;
            return SetDepthLevel(offset * 10);
        }

        public static int RestoreDepthLayer()
        {
            return SetDepthLayer(GUI.CurRegion.Layer.LayerType);
        }



        internal static void DrawDebugInfo()
        {
            GUILayout.Label("poolDropRect: " + s_poolDropRect.m_objects.Values.Count);

            var layerwin = m_form.GetLayer(GUILayerType.Window);

            GUILayout.Label(layerwin.SyncAll + " " + layerwin.m_focusedView);
        }


    }
}
