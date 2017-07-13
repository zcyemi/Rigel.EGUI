#pragma once
#include "rg_include.h"
namespace rg
{
	enum RgImageType
	{
		RgImageType_Raw,
		RgImageType_Targa,
	};

	class RgImage
	{
	public:
		void SetData(int width, int height, unsigned char* data);

		int Width();
		int Height();
		unsigned char* Data();

		void Release();
		~RgImage();
	private:
		int m_width;
		int m_height;
		unsigned char* m_data;
	};

	bool RgImageLoad(const WCHAR * filename, RgImage** img, RgImageType imgtype);
	bool RgImageLoad(std::wstring filename, RgImage** img, RgImageType imgtype);

	void RgImageSave(const WCHAR* filename, unsigned char * data, unsigned int width, unsigned int height,RgImageType imgtype);
}