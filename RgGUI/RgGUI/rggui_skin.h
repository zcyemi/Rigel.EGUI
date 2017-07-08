#pragma once

namespace rg
{
	namespace gui
	{
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