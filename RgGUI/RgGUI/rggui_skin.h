#pragma once
#include "rg_include.h"
namespace rg
{
	namespace gui
	{
		const RgVec2 SKIN_WINDOW_DEFAULT_SIZE = RgVec2(200, 100);
		const RgVec2 SKIN_WINDOW_DEFUALT_POSISION = RgVec2(10, 10);

		struct RgGuiWindowSkin
		{

		};


		struct RgGuiSkin
		{
			RgGuiWindowSkin WindowSkin;

			RgGuiSkin();
		};


		extern RgGuiSkin RGGUI_SKIN_DEFAULT;
	}
}