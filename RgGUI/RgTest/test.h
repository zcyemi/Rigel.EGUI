#pragma once
#include <rapidjson\document.h>
#include <iostream>

using namespace rapidjson;
void Test()
{
	const char * json = "{\"hello\":\"world\",\"test\":1}";

	Document doc;
	doc.Parse(json);

	std::cout << doc["hello"].GetString() << std::endl;
}