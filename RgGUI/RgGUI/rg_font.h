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
			static bool LoadFont(LPCSTR fontpath, RgFontFreeType* font);

			~RgFontFreeType();
		private:
			FT_Face *m_pFtFace;
			RgFontFreeType(FT_Face * face);

			void Release();
		};
	}
}