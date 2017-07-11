#pragma once
#include <rapidjson\document.h>
#include <iostream>
#include <unordered_map>
#include <rg_include.h>

using namespace rapidjson;


void textDraw(const char* t)
{
	static std::unordered_map<char, unsigned int> charmap;
	while (*t)
	{
		charmap[*t] +=1;
		++t;
	}


	for (auto p : charmap)
	{
		RgLogD() << p.first << p.second;
	}

}

void Test()
{

	textDraw("Unordered map is an associative container that contains key-value pairs with unique keys. Search, insertion, and removal of elements have average constant-time complexity.");

	//const char * json = "{\"hello\":\"world\",\"test\":1}";

	//Document doc;
	//doc.Parse(json);

	//std::cout << doc["hello"].GetString() << std::endl;

	

}