#pragma once
#include "rg_include.h"

namespace rg
{
	namespace gui
	{
		namespace ICON
		{
			enum RgGuiIcon
			{
				ICON_NONE = 0,
				#include "Data\rggui_iconset_map.h"
			};
		}

		static unsigned int s_IconTextureSize = 0;
		static RgVec2 s_IconUVsize = 0.001f;

		void RgGui_IconSetTextureSize(unsigned int size);
		

		void RgGui_IconGetUV(ICON::RgGuiIcon icontype, RgVec2* uv);
		
		RgVec2& RgGui_IconGetUVsize();
		

	}
}