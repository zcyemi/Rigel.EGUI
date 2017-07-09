#include "rggui_iconset.h"

namespace rg
{
	namespace gui
	{



		void RgGui_IconSetTextureSize(unsigned int size)
		{
			s_IconTextureSize = size;
			s_IconUVsize = RgVec2(16.0f / size);
			s_IconUVsize.y = -s_IconUVsize.y;

			RgLogW() << "set texture szie" << s_IconTextureSize;
		}

		void RgGui_IconGetUV(ICON::RgGuiIcon icontype, RgVec2 * uv)
		{
			if (s_IconTextureSize == 0) return;
			unsigned int index = icontype;
			static int lineCount = s_IconTextureSize / 16;
			unsigned int row = index / lineCount;
			unsigned int col = index %lineCount;

			(*uv).x = col * s_IconUVsize.x;
			(*uv).y = (row + 1.0f) * s_IconUVsize.x;
		}

		RgVec2& RgGui_IconGetUVsize()
		{
			return s_IconUVsize;
		}

	}
}

