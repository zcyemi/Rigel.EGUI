#pragma once
#include "rg_include.h"
namespace rg
{
	enum RgImageType
	{
		RgImageType_Targa,
	};

	class RgImage
	{
	public:
		void SetData(int width, int height, unsigned char* data);
	private:
		int m_width;
		int m_height;
		unsigned char* m_data;
	};

	bool RgImageLoad(const WCHAR * filename, RgImage** img, RgImageType imgtype);
	bool RgImageLoad(std::wstring filename, RgImage** img, RgImageType imgtype);
}