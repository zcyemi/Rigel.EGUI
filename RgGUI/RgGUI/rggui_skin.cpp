#include "rggui_skin.h"

namespace rg
{
	namespace gui
	{
		const char * SKIN_FONT_DEFAULT = "arial.ttf";

		RgGuiSkin::RgGuiSkin()
		{
			FONT_TYPE = SKIN_FONT_DEFAULT;
		}
		RgGuiSkin RGGUI_SKIN_DEFAULT;
	}
}


