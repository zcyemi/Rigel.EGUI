#pragma once
#include "rggui_defines.h"

namespace rg
{
	namespace gui
	{

		RgMemAlloc g_RgGuiMemAlloc;
		RgGuiContext g_RgGuiContext;
		RgGuiContext *GRgGui = &g_RgGuiContext;

#pragma region RgGuiWindow
		RgGuiWindow::RgGuiWindow(const char * name) :Name(name)
		{
			ID = RgHash(name, 0);
			//TODO:
			this->DrawList = g_RgGuiMemAlloc.New<RgGuiDrawList>();
		}

		void RgGuiWindow::DrawSelf()
		{
			DrawList->AddRect(Pos, Pos + Size);
		}

		void RgGuiWindow::SetSize(RgVec2 & s)
		{
			Size = s;
		}

		void RgGuiWindow::SetPosition(RgVec2 & p)
		{
			Pos = p;
		}

		void RgGuiWindow::Move(RgVec2 & offset)
		{
			Pos.x += offset.x;
			Pos.y += offset.y;
		}

		void RgGuiWindow::Begin()
		{
			DrawList->Reset();
			DrawSelf();
		}

		void RgGuiWindow::End()
		{

		}
#pragma endregion

#pragma region RgGuiDrawList
		void RgGuiDrawList::AddRect(const RgVec2 & lt, const RgVec2 & rb)
		{
			RgU32 col = 0xffffffff;
			RgGuiDrawVert v1(lt, RgVec2(), col);
			RgGuiDrawVert v2(RgVec2(rb.x, lt.y), RgVec2(), col);
			RgGuiDrawVert v3(rb, RgVec2(), col);
			RgGuiDrawVert v4(RgVec2(lt.x, rb.y), RgVec2(), col);

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

			IndicesIndex += 6;
			VertexCount += 4;
		}


		RgGuiDrawList::RgGuiDrawList()
		{
			RgLogD() << "create draw list";
		}

		void RgGuiDrawList::Reset()
		{
			IndicesIndex = 0;
			VertexCount = 0;

			IndicesBuffer.Size = 0;
			VertexBuffer.Size = 0;
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
#pragma endregion


#pragma region RgGuiContext
		void RgGuiContext::SetScreenSize(RgU32 w, RgU32 h)
		{
			ScreenWidth = w;
			ScreenHeight = h;

			RgLogD() << "set screen size" << w << " " << h;
		}
#pragma endregion

	}
}

