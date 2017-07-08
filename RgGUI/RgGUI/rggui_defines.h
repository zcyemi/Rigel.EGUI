#pragma once
#include "rg_include.h"
#include "rggui_enum.h"
#include "rg_memalloc.h"
namespace rg
{
	namespace gui
	{
		typedef unsigned int RgGuiDrawIdx;
		typedef unsigned int RgU32;

		struct RgGuiDrawList;


#pragma region static
		
#pragma endregion




		struct RgGuiIO
		{
			int KeyMap[RgGuiKey_COUNT];
			bool MouseDown[5];
			float MouseWheel;

			RgVec2 MousePos;

			bool MouseDrawCursor;
			bool KeyCtrl;
			bool KeyShift;
			bool KeyAlt;
			bool KeySuper;

			bool KeyDown[512];
		};

		struct RgGuiWindow
		{
			const char * Name;
			unsigned int ID;
			RgVec2 Pos;
			RgVec2 Size;

			RgGuiDrawList * DrawList;

			RgGuiWindow(const char * name);

			void DrawSelf();
			void SetSize(RgVec2& s);
			void SetPosition(RgVec2& p);
			void Move(RgVec2& offset);

			void Begin();
			void End();
		};

		struct RgGuiDrawVert
		{
			RgVec2 pos;
			RgVec2 uv;
			RgU32 color;
			RgGuiDrawVert();
			RgGuiDrawVert(RgVec2 pos_, RgVec2 uv_, RgU32 col_);
			RgGuiDrawVert(RgVec2 pos_);
			RgGuiDrawVert(RgVec2 pos_,RgU32 col_);
			RgGuiDrawVert(float x, float y);
			RgGuiDrawVert(float x, float y, RgU32 col_);
		};

		struct RgGuiDrawList
		{
			RgVector<RgGuiDrawIdx> IndicesBuffer;
			RgVector<RgGuiDrawVert> VertexBuffer;

			RgU32 IndicesIndex = 0;
			RgU32 VertexCount = 0;

			RgGuiDrawList();

			void AddRect(const RgVec2& lt, const RgVec2& rb);
			void AddRect(float x, float y, float w, float h);
			void AddRect(const RgVec4& r);
			
			void SetColor(RgU32 col);
			void SetColor(byte r, byte g, byte b, byte a);


			void Reset();
		};

		struct RgGuiContext
		{
			RgGuiWindow* CurrentWindow;
			RgGuiDrawList DrawList;
			RgGuiIO IO;
			RgVector<RgGuiWindow *> Windows;
			RgU32 ScreenWidth, ScreenHeight;
			bool(*RenderDrawListFunction)(RgGuiDrawList* data);

			void SetScreenSize(RgU32 w, RgU32 h);

		};


		extern RgMemAlloc g_RgGuiMemAlloc;
		extern RgGuiContext g_RgGuiContext;
		extern RgGuiContext *GRgGui;

	}
}