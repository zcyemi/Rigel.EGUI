#pragma once
#include "rg_inc.h"

namespace rg
{
#ifndef RG_IMAGE_H
#define RG_IMAGE_H

	enum RG_IMAGE_TYPE
	{
		RG_IMAGE_TYPE_NONE,
		RG_IMAGE_TYPE_RAW,
		RG_IMAGE_TYPE_TARGA,
		RG_IMAGE_TYPE_BMP,
		RG_IMAGE_TYPE_PNG,
		RG_IMAGE_TYPE_JPG
	};

	class RgImage
	{
	public:
		RgImage(){}
		~RgImage(){
			Release();
		}

		void Release()
		{
			RG_RELEASE_A(m_data);
		}

		const RgUInt& GetWidth() const {
			return m_width;
		}
		const RgUInt& GetHeight()const {
			return m_height;
		}

		RgByte* GetData()const { return m_data; }
	private:
		RgUInt m_width;
		RgUInt m_height;

		RgByte* m_data;
		RG_IMAGE_TYPE m_image_type = RG_IMAGE_TYPE_NONE;
	};

	RgImage::RgImage()
	{
	}

	RgImage::~RgImage()
	{
	}

#pragma region Targa
	struct RGIMAGE_TARGA_HEADER
	{
		RgByte data1[12];
		RgWord width;
		RgWord height;
		RgByte bpp;
		RgByte data2;
	};

	bool RGIMAGE_LOAD_TARGA(FILE* fileptr, RgImage ** img)
	{
		return false;
	}
#pragma endregion

#pragma region Bitmap
	struct RGIMAGE_BITMAP_HEADER
	{
		WORD bfType;
		DWORD bfSize;
	};
#pragma endregion



#endif // !RG_IMAGE_H

}