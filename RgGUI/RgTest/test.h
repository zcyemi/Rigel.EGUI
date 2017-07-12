#pragma once
#include <rapidjson\document.h>
#include <iostream>
#include <unordered_map>
#include <rg_include.h>

using namespace rapidjson;

std::unordered_map<char, int *> text_map;


void textDraw(const char* t)
{

}

void Test()
{

	textDraw("Unordered map is an associative container that contains key-value pairs with unique keys. Search, insertion, and removal of elements have average constant-time complexity.");

	//const char * json = "{\"hello\":\"world\",\"test\":1}";

	//Document doc;
	//doc.Parse(json);

	//std::cout << doc["hello"].GetString() << std::endl;

	

}