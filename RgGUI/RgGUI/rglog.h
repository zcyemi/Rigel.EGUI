#pragma once
#include "RgInclude.h"
#include <string>
#include <sstream>

#define RgLogD RgLogger(__FILE__,__FUNCTION__,__LINE__).Debug


namespace rg
{
	class RgLogger
	{
	public:
		RgLogger(const char * file, const char * function, int line);
		~RgLogger();
	public:
		RgLogger& Debug();

	public:

		RgLogger& operator <<(const char * v);
		RgLogger& operator <<(int v);

	private:
		std::stringstream m_sstream;
	};
}