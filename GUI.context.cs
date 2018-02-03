﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Collections;
using Rigel.GUI.Component;

namespace Rigel.GUI
{
    public static partial class GUI
    {

        private static GUIForm m_form = null;
        private static GUILayer m_layer = null;
        private static GUIRegion m_region = null;

        private static IGUIBuffer BufRect { get; set; }
        private static IGUIBuffer BufText { get; set; }

        private static int DepthBase;
        private static float DepthValue = 0;
        private static readonly float DepthStep = 0.0001f;

        public static GUIForm Form { get { return m_form; } }


        internal static GUIFrame m_frame;
        private static GUIFrame Frame { get { return m_frame; } }

        internal static GUIAreaInfo CurArea;
        internal static GUILayoutInfo CurLayout;

        internal static IFontInfo Font { get; private set; }

        public static RigelGUIEvent Event { get; private set; }

        //ObjPool
        [TODO("Refactoring","Move to form for multipal form draw")]
        private static GUIObjPool<GUIObjTabView> s_poolTabView = new GUIObjPool<GUIObjTabView>();
        private static GUIObjPool<GUIObjScrollView> s_poolScrollView = new GUIObjPool<GUIObjScrollView>();


        internal static GUIObjTabView GetObjTabView(Vector4 rect, Action<GUIObjTabView> createFunction = null) 
        {
            return s_poolTabView.Get(GUIUtility.GetHash(rect, GUIObjType.TabView), createFunction);
        }

        internal static GUIObjScrollView GetObjScrollView(Vector4 rect,Action<GUIObjScrollView> createFuction = null)
        {
            return s_poolScrollView.Get(GUIUtility.GetHash(rect, GUIObjType.ScrollView), createFuction);
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

        

        internal static void StartGUIRegion(GUIRegion region)
        {
            if (m_layer == null) throw new Exception();
            if (m_region != null) throw new Exception();

            m_region = region;

            DepthBase = m_layer.Order - m_region.Order;
            DepthValue = DepthBase;

            //Process Buffer and Offset
            BufRect = m_layer.GetBufferRect(m_region);
            BufText = m_layer.GetBufferText(m_region);


            region.BlockInfoRect.Start = BufRect.Count;
            //region.BlockInfoText.Start = BufText.Count;

            BeginArea(region.Rect);

        }
        internal static void EndGUIRegion(GUIRegion region)
        {
            EndArea();

            region.BlockInfoRect.Count = BufRect.Count - region.BlockInfoRect.Start;
            //region.BlockInfoText.Count = BufText.Count - region.BlockInfoText.Start;

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
    }
}
