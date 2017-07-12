#include "rggui_textrender.h"

namespace rg
{
	namespace gui
	{
		RgGuiTextRender::RgGuiTextRender()
		{
		}

		RgGuiTextRender::~RgGuiTextRender()
		{
			Release();
		}

		void RgGuiTextRender::Release()
		{
			m_pFont = nullptr;
		}

		void RgGuiTextRender::SetFont(RgGuiFont * font)
		{
			m_pFont = font;
		}


		void RgGuiTextRender::DrawTextWithRect(const char * text, RgGuiWindow * window, RgVec2 & rectpos, RgVec2 & rectsize)
		{
			//process char
			while (*text)
			{
				auto c = *text;
				if (m_charMap[c] == 0)
				{
					m_charIndex++;
					m_charMap[c] = m_charIndex;

					m_pFont->RenderGlyph(c);
				}
				else
				{

				}

				++text;
			}
		}


	}
}

