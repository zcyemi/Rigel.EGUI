#include "rglog.h"

void RgLogContext::copy(const RgLogContext & context)
{
	this->m_file = context.m_file;
	this->m_function = context.m_function;
	this->m_line = context.m_line;
}

RgDebug RgLogger::info() const
{
	RgDebug dbg = RgDebug();
	RgLogContext &ctx = dbg.stream->context;
	ctx.copy(m_context);
	dbg.stream->logtype = Info;
	return dbg;
}

RgDebug RgLogger::debug() const
{
	RgDebug dbg = RgDebug();
	RgLogContext &ctx = dbg.stream->context;
	ctx.copy(m_context);
	dbg.stream->logtype = Debug;
	return dbg;
}
