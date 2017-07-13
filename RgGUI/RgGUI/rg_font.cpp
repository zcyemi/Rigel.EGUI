#include "rg_font.h"

namespace rg
{
	namespace font
	{

		void RgFont_FreeType_Init()
		{
			if (s_ftInited) return;
			FT_Error err;
			err = FT_Init_FreeType(&s_ftLibrary);

			if (err)
			{
				RgLogE() << "FreeType init error:" << err;
				return;
			}
			RgLogD() << "FreeType init done!" << err;
			s_ftInited = true;

		}

		void RgFont_FreeType_Release()
		{

		}

		bool RgFontFreeType::LoadFont(LPCSTR fontpath, RgFontFreeType *& font)
		{
			if (!s_ftInited) return false;
			FT_Error err;
			FT_Face face = nullptr;
			err = FT_New_Face(s_ftLibrary, fontpath, 0, &face);
			if (err)
			{
				RgLogE() << "FreeType load error:" << err;
				return false;
			}

			RgLogD() << "FreeType load done!";
			font = new RgFontFreeType(face);
			return true;
		}

		void RgFontFreeType::SetPixelSize(unsigned int width, unsigned int height)
		{
			FT_Error err = FT_Set_Pixel_Sizes(m_pFtFace, width, height);
			if (err)
			{
				RgLogD() << "freetype set pixel size error:" << err;
			}
			else
			{
				RgLogD() << "freetype set pixel size done!"<<width<<height;
			}
		}

		unsigned int RgFontFreeType::GetCharIndex(unsigned long charcode)
		{
			return FT_Get_Char_Index(m_pFtFace, charcode);
		}

		bool RgFontFreeType::LoadGlyph(unsigned int index)
		{
			FT_Error err = FT_Load_Glyph(m_pFtFace, index, FT_LOAD_DEFAULT);
			if (err)
			{
				RgLogE() << "freetype load glyph error "<<err;
				return false;
			}
			return true;
		}

		bool RgFontFreeType::RenderGlyph(FT_Render_Mode rendermode)
		{
			FT_Error err = FT_Render_Glyph(m_pFtFace->glyph, rendermode);
			if (err)
			{
				RgLogE() << "freetype render glyph error";
				return false;
			}
			Glyph = m_pFtFace->glyph;
			return true;
		}

		bool RgFontFreeType::LoadChar(unsigned long charcode, long rendermode)
		{
			FT_Error err = FT_Load_Char(m_pFtFace, charcode, rendermode);
			if (err)
			{
				RgLogE() << "freetype load char error:" << err;
				return false;
			}

			Glyph = m_pFtFace->glyph;
			return true;
		}

		RgFontFreeType::~RgFontFreeType()
		{
			Release();
		}

		RgFontFreeType::RgFontFreeType(FT_Face face)
		{
			RgLogW() << "font type" << (int)&face;
			m_pFtFace = face;
		}

		void RgFontFreeType::Release()
		{
			delete m_pFtFace;
			m_pFtFace = nullptr;
			Glyph = nullptr;
		}

	}
}
