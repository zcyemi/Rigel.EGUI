#pragma once
#include "RgInclude.h"
#include <iomanip>
#include <fstream>
#include <sstream>
#include <vector>

class RgLogContext
{
public:
	RgLogContext():m_line(0), m_file(0), m_function(0){}
	RgLogContext(const char * filename,const char *functionname,int line):m_file(filename),m_function(functionname),m_line(line){}
	void copy(const RgLogContext& context);

public:
	int m_line;
	const char *m_function;
	const char * m_file;

};

enum RgLogType
{
	Info,
	Debug,
	Warning,
	Error,
};

class RgDebug
{
private:
	static RgDebug* s_pInstance;

public:
	struct Stream
	{
		Stream():stringstream(),space(true),context(){}
		Stream(std::string *s):stringstream(*s),space(true),context(){}
		std::ostringstream stringstream;
		bool space;
		RgLogContext context;
		RgLogType logtype;
	} *stream;

	RgDebug():stream(new Stream()){}
	inline RgDebug(std::string *s):stream(new Stream(s)){}
	~RgDebug()
	{
		LogToConsole(stream->logtype, stream->context, stream->stringstream.str());
		delete stream;
	}
public:
	
	//opeator overide
	inline RgDebug& operator<<(bool t) { 
		stream->stringstream << (t ? "true" : "false");
		return addspace();
	}

	inline RgDebug& space() {
		stream->space = true;
		stream->stringstream << ' ';
		return *this;
	}

	inline RgDebug& nospace()
	{
		stream->space = false;
		return *this;
	}

	inline RgDebug& addspace()
	{
		if (stream->space)
			stream->stringstream << ' ';
		return *this;
	}

	void LogToConsole(RgLogType type, const RgLogContext &context, std::string log)
	{
		std::string logstr;
		switch (type)
		{
		case Info:
			logstr.append("[Info] ");
			break;
		case Debug:
			logstr.append("[Debug] ");
			break;
		case Warning:
			logstr.append("[Warning] ");
			break;
		case Error:
			logstr.append("[Error] ");
			break;
		default:
			break;
		}

		logstr.append(log);
		logstr.append(".....");
		logstr.append(context.m_file);
		logstr.append(" ");
		logstr.append(context.m_function);
		logstr.append("()");

		std::cout << logstr << " line: " << context.m_line << " " << std::endl;
	}
	


};

#define rgLogD RgLogger(__FILE__,__FUNCTION__,__LINE__).debug
#define rgLogI RgLogger(__FILE__,__FUNCTION__,__LINE__).info
#define rgLogW RgLogger(__FILE__,__FUNCTION__,__LINE__).warning
#define rgLogE RgLogger(__FILE__,__FUNCTION__,__LINE__).error

class RgLogger
{
public:
	RgLogger() :m_context() {};
	RgLogger(const char *filename,const char * functionname,int line):m_context(filename,functionname,line){}

	RgDebug info() const;
	RgDebug debug() const;
	RgDebug warning() const;
	RgDebug error() const;

private:
	RgLogContext m_context;
};