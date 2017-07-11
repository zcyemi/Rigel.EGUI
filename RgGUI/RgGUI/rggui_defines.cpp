#pragma once
#include "rggui_defines.h"
#include "rggui_textrender.h"

namespace rg
{
	namespace gui
	{

		RgMemAlloc g_RgGuiMemAlloc;
		RgGuiContext g_RgGuiContext;
		RgGuiContext *GRgGui = &g_RgGuiContext;

#pragma region RgGuiWindow

		RgGuiWindow::RgGuiWindow(const char * name, RgGuiWindowDesc * desc)
		{
			ID = RgHash(name, 0);
			//TODO:
			this->DrawList = g_RgGuiMemAlloc.New<RgGuiDrawList>();

			if (desc == nullptr)
			{
				desc = &RgGuiWindowDesc();
			}

			SetStyle(desc->Style);
			SetSkin(desc->Skin);
			SetPosition(desc->Position);
			SetSize(desc->Size);

		}

		void RgGuiWindow::DrawSelf()
		{
			DrawList->SetColor(0xffffffff);
			//background
			DrawList->AddRect(Pos, Pos + Size);

			if ((Style & RgGuiWindowStyle_Header) > 0)
			{
				DrawList->SetColor(0xff999999);
				DrawList->AddRect(Pos, Pos + RgVec2(Size.x,Skin.HEADER_HEIGHT));
			}

			//finish draw header
			temp_layoutOffset.y += Skin.HEADER_HEIGHT;
			temp_layoutOffset += Skin.CONTEXT_OFFSET;
		}

		void RgGuiWindow::SetSize(RgVec2 & s)
		{
			Size = s;
		}

		void RgGuiWindow::SetPosition(RgVec2 & p)
		{
			Pos = p;
		}

		void RgGuiWindow::SetStyle(RgGuiDrawWindowStyle style)
		{
			Style = style;
		}

		void RgGuiWindow::SetSkin(RgGuiWindowSkin * skin)
		{
			if (skin == nullptr)
			{
				Skin = RgGuiWindowSkin(GRgGui->Skin.WindowSkin);
				RgLogD() << "use default skin";
			}
			else
			{
				Skin = RgGuiWindowSkin(*skin);
			}
		}

		void RgGuiWindow::Move(RgVec2 & offset)
		{
			Pos.x += offset.x;
			Pos.y += offset.y;
		}

		void RgGuiWindow::Begin()
		{
			temp_layoutOffset.x = temp_layoutOffset.y = 0;

			DrawList->Reset();
			DrawSelf();
		}

		void RgGuiWindow::End()
		{

		}
#pragma endregion



#pragma region RgGuiDrawVert
		RgGuiDrawVert::RgGuiDrawVert()
		{
		}

		RgGuiDrawVert::RgGuiDrawVert(RgVec2 pos_, RgVec2 uv_, RgU32 col_)
		{
			pos = pos_;
			uv = uv_;
			color = col_;
		}

		RgGuiDrawVert::RgGuiDrawVert(RgVec2 pos_) :pos(pos_), uv(0), color(0)
		{

		}
		RgGuiDrawVert::RgGuiDrawVert(RgVec2 pos_, RgU32 col_):pos(pos_),uv(0),color(col_)
		{
		}
		RgGuiDrawVert::RgGuiDrawVert(float x, float y) : RgGuiDrawVert(x, y, 0)
		{
		}
		RgGuiDrawVert::RgGuiDrawVert(float x, float y, RgU32 col_) : pos(RgVec2(x, y)), uv(0), color(col_)
		{
		}
#pragma endregion


#pragma region RgGuiContext
		void RgGuiContext::SetScreenSize(RgU32 w, RgU32 h)
		{
			ScreenWidth = w;
			ScreenHeight = h;

			RgLogD() << "set screen size" << w << " " << h;
		}
		void RgGuiContext::SetSkin(RgGuiSkin skin)
		{
			Skin = skin;
			bool result = Font.LoadFont(Skin.FONT_TYPE);
			if (!result) RgLogE() << "load font failed";
		}
		void RgGuiContext::Init()
		{
			TextRender = g_RgGuiMemAlloc.New<RgGuiTextRender>();
			TextRender->SetFont(&Font);
		}
		void RgGuiContext::Release()
		{
			TextRender->Release();
			delete TextRender;
			TextRender = nullptr;
		}
#pragma endregion


#pragma region RgGuiDrawList

		static RgU32 s_tempColor = 0xffffffff;

		RgGuiDrawList::RgGuiDrawList()
		{
			RgLogD() << "create draw list";
		}

		void RgGuiDrawList::AddRect(const RgVec2 & lt, const RgVec2 & rb)
		{
			AddRect(lt.x,lt.y,rb.x - lt.x,rb.y - lt.y);
		}

		void RgGuiDrawList::AddRect(float x, float y, float w, float h)
		{
			RgGuiDrawVert v1(x,y, s_tempColor);
			RgGuiDrawVert v2(x+w,y, s_tempColor);
			RgGuiDrawVert v3(x+w,y+h, s_tempColor);
			RgGuiDrawVert v4(x,y+h, s_tempColor);

			v1.uv = RgVec2(0.001f, 0.001f);
			v2.uv = RgVec2(0, 0.001f);
			v3.uv = RgVec2(0.001f, 0);
			v4.uv = RgVec2(0, 0);

			VertexBuffer.push_back(v1);
			VertexBuffer.push_back(v2);
			VertexBuffer.push_back(v3);
			VertexBuffer.push_back(v4);

			IndicesBuffer.push_back(IndicesIndex);
			IndicesBuffer.push_back(IndicesIndex + 1);
			IndicesBuffer.push_back(IndicesIndex + 2);
			IndicesBuffer.push_back(IndicesIndex);
			IndicesBuffer.push_back(IndicesIndex + 2);
			IndicesBuffer.push_back(IndicesIndex + 3);

			IndicesIndex += 4;
			VertexCount += 4;
		}

		void RgGuiDrawList::AddRect(const RgVec4 & r)
		{
			AddRect(r.x, r.y, r.z, r.w);
		}

		void RgGuiDrawList::SetLastRectUV(const RgVec2 & lt, const RgVec2 & rb)
		{
			if (VertexCount < 4)return;
			VertexBuffer[VertexCount - 4].uv = lt;
			VertexBuffer[VertexCount - 3].uv = RgVec2(rb.x,lt.y);
			VertexBuffer[VertexCount - 2].uv = rb;
			VertexBuffer[VertexCount - 1].uv = RgVec2(lt.x, rb.y);

			//RgLogD() << "set uv" << lt.x << lt.y << rb.x << rb.y;
		}

		void RgGuiDrawList::SetColor(RgU32 col)
		{
			s_tempColor = col;
		}

		void RgGuiDrawList::SetColor(byte r, byte g, byte b, byte a)
		{
			s_tempColor = (((((r << 8) | g) << 8) | b)<<8) | a;
		}


		void RgGuiDrawList::Reset()
		{
			IndicesIndex = 0;
			VertexCount = 0;

			IndicesBuffer.Size = 0;
			VertexBuffer.Size = 0;
		}
#pragma endregion


		RgGuiWindowDesc::RgGuiWindowDesc(RgVec2 pos,RgVec2 size, RgGuiDrawWindowStyle style, RgGuiWindowSkin * skin):
			Size(size), Position(pos),Style(style),Skin(skin)
		{
		}

		RgGuiWindowDesc::RgGuiWindowDesc()
		{
		}

		RgGuiFont::RgGuiFont()
		{
			font::RgFont_FreeType_Init();
		}

		RgGuiFont::~RgGuiFont()
		{
			Release();

			
		}

		RgGuiFont::RgGuiFont(const char * fontpath)
		{
			LoadFont(fontpath);
		}

		bool RgGuiFont::LoadFont(const char * fontpath)
		{
			font::RgFont_FreeType_Init();
			return font::RgFontFreeType::LoadFont(fontpath, FontType);
		}

		void RgGuiFont::Release()
		{
			if (FontType)
			{
				delete FontType;
				FontType = nullptr;
				RgLogD() << "release rggui font";
			}
		}

}
}

