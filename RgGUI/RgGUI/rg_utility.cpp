#include "rg_utility.h"

namespace rg
{
	namespace util
	{
		static std::wstring s_workpath;

		const std::wstring GetWorkDirectory()
		{
			static bool pathgot = false;
			if (!pathgot)
			{
				WCHAR buf[512];
				GetCurrentDirectory(512, buf);
				s_workpath = std::wstring(buf);
				std::cout << s_workpath.length() << std::endl;
				pathgot = true;
			}

			return s_workpath;
		}
	}
}


