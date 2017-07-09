#pragma once
#include "rg_include.h"
#include "rggui_enum.h"
#include "rg_memalloc.h"
#include "rggui_skin.h"
#include "rg_font.h"
#include "rggui_iconset.h"
namespace rg
{
	namespace gui
	{
		typedef unsigned int RgGuiDrawIdx;
		typedef unsigned int RgU32;
		typedef unsigned int RgGuiDrawWindowStyle;

		struct RgGuiDrawList;
		class RgGuiTextRender;


#define COLOR_WHITE = 0xffffffff;
#define COLOR_BLACK = 0xff000000;



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

		struct RgGuiWindowDesc
		{
			RgVec2 Size = SKIN_WINDOW_DEFAULT_SIZE;
			RgVec2 Position = SKIN_WINDOW_DEFUALT_POSISION;
			RgGuiDrawWindowStyle Style = RgGuiWindowStyle_Default;
			RgGuiWindowSkin * Skin = 0;

			RgGuiWindowDesc(RgVec2 pos,RgVec2 size = SKIN_WINDOW_DEFAULT_SIZE , RgGuiDrawWindowStyle style = RgGuiWindowStyle_Default, RgGuiWindowSkin * skin = 0);
			RgGuiWindowDesc();
		};

		struct RgGuiWindow
		{
			const char * Name;
			unsigned int ID;
			RgVec2 Pos;
			RgVec2 Size;

			RgGuiDrawList * DrawList;

			RgGuiDrawWindowStyle Style;
			RgGuiWindowSkin Skin;

			RgVec2 temp_layoutOffset;

			RgGuiWindow(const char * name, RgGuiWindowDesc* desc);

			void DrawSelf();

			void SetSize(RgVec2& s);
			void SetPosition(RgVec2& p);
			void SetStyle(RgGuiDrawWindowStyle s);
			void SetSkin(RgGuiWindowSkin* skin);

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

			void SetLastRectUV(const RgVec2& lt, const RgVec2& rb);
			
			void SetColor(RgU32 col);
			void SetColor(byte r, byte g, byte b, byte a);


			void Reset();
		};

		struct RgGuiFont
		{
			font::RgFontFreeType *FontType;

			const char * FontFilePath;
			RgGuiFont();
			~RgGuiFont();
			RgGuiFont(const char* fontpath);


			bool LoadFont(const char* fontpath);
			void Release();
		};

		struct RgGuiContext
		{
			RgGuiWindow* CurrentWindow;
			RgGuiDrawList DrawList;
			RgGuiIO IO;
			RgVector<RgGuiWindow *> Windows;
			RgU32 ScreenWidth, ScreenHeight;
			RgGuiSkin Skin;
			RgGuiFont Font;
			bool(*RenderDrawListFunction)(RgGuiDrawList* data);

			void SetScreenSize(RgU32 w, RgU32 h);
			void SetSkin(RgGuiSkin skin);

		};


		extern RgMemAlloc g_RgGuiMemAlloc;
		extern RgGuiContext g_RgGuiContext;
		extern RgGuiContext *GRgGui;

	}
}