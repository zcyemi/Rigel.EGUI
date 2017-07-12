#pragma once
#include "rg_include.h"
#include "rggui_defines.h"

#include <unordered_map>

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

			std::vector<char> m_charTemp;
			std::unordered_map<char, unsigned int> m_charMap;
			unsigned int m_charIndex = 0;

		private:
		};
	}
}