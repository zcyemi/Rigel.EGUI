#pragma once
#include "rg_include.h"
#pragma comment(lib,"freetype.lib")

#include <ft2build.h>
#include FT_FREETYPE_H
namespace rg
{
	namespace font
	{
		static FT_Library s_ftLibrary;
		static bool s_ftInited = false;

		void RgFont_FreeType_Init();
		void RgFont_FreeType_Release();


		class RgFontFreeType
		{
		public:
			static bool LoadFont(LPCSTR fontpath, RgFontFreeType*& font);

			void SetPixelSize(unsigned int width, unsigned int height);
			unsigned int GetCharIndex(unsigned long charcode);
			bool LoadGlyph(unsigned int index);
			bool RenderGlyph(FT_Render_Mode rendermode);
			bool LoadChar(unsigned long charcode, long rendermode = FT_LOAD_RENDER);

			~RgFontFreeType();
		public:
			FT_GlyphSlot Glyph = 0;
		private:
			FT_Face m_pFtFace;
			RgFontFreeType(FT_Face face);

			void Release();
		};
	}
}