#pragma once
#include "rg_include.h"
#include "rggui_defines.h"


namespace rg
{
	namespace gui
	{
		class RgGuiTextRender
		{
		public:
			RgGuiTextRender();
			~RgGuiTextRender();

			void Release();

		public:
			void SetFont(RgGuiFont* font);

			void DrawTextWithRect(const char* text, RgGuiWindow *window, RgVec2& rectpos, RgVec2& rectsize);

		private:
			RgGuiFont * m_pFont;
			unsigned char * m_pdata;
		};
	}
}