#pragma once
#include "RgInclude.h"

namespace rg {
	namespace gui
	{





		void Render();
		void NewFrame();
		void ShutDown();

		void Begin();
		void End();


		void Text(const char *t);
		bool Button(const char *t);
	}
}