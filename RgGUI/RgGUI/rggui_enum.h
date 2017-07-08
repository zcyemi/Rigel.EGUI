#pragma once

namespace rg
{
	namespace gui
	{


		enum RgGuiKey
		{
			RgGuiKey_Space,
			RgGuiKey_Enter,
			RgGuiKey_Tab,
			RgGuiKey_LeftArrow,
			RgGuiKey_RightArrow,
			RgGuiKey_UpArrow,
			RgGuiKey_DownArrow,
			RgGuiKey_PageUp,
			RgGuiKey_PageDown,
			RgGuiKey_Backspace,
			RgGuiKey_Escape,
			RgGuiKey_COUNT
		};


		enum  RgGuiWindowStyle
		{
			RgGuiWindowStyle_Default = 1,
			RgGuiWindowStyle_Header = 1 << 1,
			RgGuiWindowStyle_Caption = 1 << 2,
			RgGuiWindowStyle_Callapse = 1 << 3,
		};

	}
}