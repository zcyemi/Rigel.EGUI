#pragma once
#include "rg_include.h"
namespace rg
{
	namespace gui
	{
		const RgVec2 SKIN_WINDOW_DEFAULT_SIZE = RgVec2(200, 100);
		const RgVec2 SKIN_WINDOW_DEFUALT_POSISION = RgVec2(10, 10);
		const RgVec2 SKIN_BUTTON_SIZE = RgVec2(70, 18);

		const RgVec2 SKIN_ICON_OFFSET = RgVec2(1);
		const RgVec2 SKIN_ICON_SIZE = RgVec2(16);
		const RgVec2 SKIN_ICON_FULLSIZE = RgVec2(18);

		struct RgGuiWindowSkin
		{
			float HEADER_HEIGHT = 18;
			RgVec2 CONTEXT_OFFSET = RgVec2(8, 5);

			float LINE_OFFSET = 3;
		};


		struct RgGuiSkin
		{
			RgGuiWindowSkin WindowSkin;
			const char * FONT_TYPE;
			const char * ICON_TEXTURE_NAME;

			RgGuiSkin();
		};


		extern RgGuiSkin RGGUI_SKIN_DEFAULT;
	}
}