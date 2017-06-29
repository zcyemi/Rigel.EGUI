#include "rglog.h"

namespace rg
{
	RgLogger::RgLogger(const char * file, const char * function, int line)
	{
	}

	RgLogger::~RgLogger()
	{
		std::cout << m_sstream.str() << std::endl;
	}

	RgLogger & RgLogger::Debug()
	{
		return *this;
	}

	RgLogger & RgLogger::operator<<(const char * v)
	{
		m_sstream << v;
		return *this;
	}

	RgLogger & RgLogger::operator<<(int v)
	{
		m_sstream << v;
		return *this;
	}

}

