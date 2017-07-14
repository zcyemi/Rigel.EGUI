#pragma once

namespace rg
{

#ifndef RG_HASH_H
#define RG_HASH_H

	int RgHash(const void* data, int data_size, unsigned int seed = 0)
	{
		static unsigned int crc32_lut[256] = { 0 };
		if (!crc32_lut[1])
		{
			const unsigned int polynomial = 0xEDB88320;
			for (unsigned int i = 0; i < 256; i++)
			{
				unsigned int crc = i;
				for (unsigned int j = 0; j < 8; j++)
					crc = (crc >> 1) ^ (unsigned int(-int(crc & 1)) & polynomial);
				crc32_lut[i] = crc;
			}
		}


		unsigned int crc = seed;
		const unsigned char* current = (const unsigned char*)data;

		if (data_size > 0) {
			while (data_size--)
				crc = (crc >> 8) ^ crc32_lut[(crc & 0xFF) ^ *current++];
		}
		else
		{
			while (unsigned char c = *current++)
			{
				crc = (crc >> 8) ^ crc32_lut[(crc & 0xFF) ^ c];
			}
		}
		return ~crc;
	}

#endif // !RG_HASH_H


	
}