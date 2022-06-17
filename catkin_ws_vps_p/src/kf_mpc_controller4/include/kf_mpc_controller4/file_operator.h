#ifndef FILE_OPERATOR_H
#define FILE_OPERATOR_H
#include "vector"
#include "iostream"
#include "string"
#include <fstream>
#include <sstream>
#include <stdlib.h>
using namespace std;
bool ReadCsvFile(std::vector<std::vector<std::string> > &data,std::string fileName);
bool WriteCsvFile(std::vector<std::vector<std::string> > data,std::string fileName);
bool StringToFloat(std::vector<std::vector<std::string> > data_in,std::vector<std::vector<float> > &data_out);
#endif // FILE_OPERATOR_H
