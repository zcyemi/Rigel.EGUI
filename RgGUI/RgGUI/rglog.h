#pragma once
#include "RgInclude.h"
#include <string>
#include <sstream>
#include <iostream>
#include <stdlib.h>


#define RgLogD RgLogger(__FILE__,__FUNCTION__,__LINE__).Debug
#define RgLogW RgLogger(__FILE__,__FUNCTION__,__LINE__).Warning
#define RgLogE RgLogger(__FILE__,__FUNCTION__,__LINE__).Error


namespace rg
{
	class RgLogger
	{
	public:
		RgLogger(const char * file, const char * function, int line);
		~RgLogger();
	public:
		RgLogger& Debug();
		RgLogger& Warning();
		RgLogger& Error();

	public:

		inline RgLogger& operator <<(const char * v)
		{
			m_sstream << v;
			return space();
		}
		inline RgLogger& operator <<(int v)
		{
			m_sstream << v;
			return space();
		}
		inline RgLogger& operator <<(unsigned short t)
		{
			m_sstream << t;
			return space();
		}
		inline RgLogger& operator <<(char t)
		{
			m_sstream << t;
			return space();
		}
		inline RgLogger& operator<<(unsigned int t)
		{
			m_sstream << t;
			return space();
		}

		inline RgLogger& space()
		{
			m_sstream << ' ';
			return *this;
		}

	private:
		std::stringstream m_sstream;
		const char * m_file;
		const char * m_func;
		int m_line;

		HANDLE m_Stdout;
	};
}