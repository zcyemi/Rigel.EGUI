#include "rg_image.h"
#include <fstream>

namespace rg
{
#pragma region targa
	struct IMAGE_TARGA_HEADER
	{
		unsigned char data1[12];
		unsigned short width;
		unsigned short height;
		unsigned char bpp;
		unsigned char data2;
	};
	bool LoadTarga(FILE * fileptr, RgImage ** img)
	{
		int error, bpp, imagesize, index, i, j, k;
		IMAGE_TARGA_HEADER targaFileHeader;
		unsigned int count;
		unsigned char* targaImage;
		count = (unsigned int)fread(&targaFileHeader, sizeof(targaFileHeader), 1, fileptr);
		if (count != 1)
		{
			return false;
		}

		int height = (int)targaFileHeader.height;
		int width = (int)targaFileHeader.width;
		bpp = (int)targaFileHeader.bpp;

		if (bpp != 32)
		{
			RgLogE() << "current targa not support 24bit .tag file";
			return false;
		}

		imagesize = width*height * 4;
		targaImage = new unsigned char[imagesize];
		if (!targaImage)
		{
			return false;
		}

		count = (unsigned int)fread(targaImage, 1, imagesize, fileptr);
		if (count != imagesize)
		{
			return false;
		}

		error = fclose(fileptr);
		if (error != 0)
		{
			return false;
		}

		(*img) = new RgImage();

		RgImage * rgimg = (*img);

		unsigned char* m_data = new unsigned char[imagesize];

		index = 0;
		k = 0;

		for (j = 0; j<height; j++)
			for (i = 0; i < width; i++)
			{
				m_data[index] = targaImage[k + 2];
				m_data[index + 1] = targaImage[k + 1];
				m_data[index + 2] = targaImage[k + 0];
				m_data[index + 3] = targaImage[k + 3];

				k += 4;
				index += 4;
			}

		delete[] targaImage;
		targaImage = 0;

		rgimg->SetData(width, height, m_data);

		RgLogD() << "load targa image done" << width << height;
	}
#pragma endregion


	bool RgImageLoad(const WCHAR * filename, RgImage ** img, RgImageType imgtype)
	{
		FILE *fileptr;
		int error = _wfopen_s(&fileptr, filename, L"rb");
		if (error != 0)
		{
			return false;
		}

		switch (imgtype)
		{
		case rg::RgImageType_Targa:
			return LoadTarga(fileptr, img);
			break;
		default:
			break;
		}

		return false;
	}

	bool RgImageLoad(std::wstring filename, RgImage ** img, RgImageType imgtype)
	{
		return RgImageLoad(filename.c_str(), img, imgtype);
	}

	void RgImageSave(const WCHAR * filename, unsigned char * data, unsigned int width, unsigned int height, RgImageType imgtype)
	{
		std::ofstream img(filename);
		const char *b = reinterpret_cast<char*>(data);
		img.write(b, width*height);
		img.close();
	}


	

	void RgImage::SetData(int width, int height, unsigned char * data)
	{
		m_width = width;
		m_height = height;
		m_data = data;
	}

	int RgImage::Width()
	{
		return m_width;
	}

	int RgImage::Height()
	{
		return  m_height;
	}

	unsigned char* RgImage::Data()
	{
		return m_data;
	}

	void RgImage::Release()
	{
		delete[] m_data;
		m_data = nullptr;
		m_width = 0;
		m_height = 0;
	}

	RgImage::~RgImage()
	{
		Release();
	}

}

