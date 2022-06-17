#include "../include/kf_mpc_controller4/file_operator.h"
template <class Type>
Type stringToNum(const string& str)
{
    istringstream iss(str);
    Type num;
    iss >> num;
    return num;
}
bool ReadCsvFile(std::vector<std::vector<std::string> > &data, std::string fileName)
{
  std::string lineStr;
  std::ifstream inFile(fileName.c_str(),std::ios::in);
  while (std::getline(inFile, lineStr))
    {
      // 打印整行字符串
      //std::cout << lineStr << std::endl;
      // 存成二维表结构
      std::stringstream ss(lineStr);
      std::string str;
      std::vector<std::string> lineArray;
      // 按照逗号分隔
      while (std::getline(ss, str, ','))
        lineArray.push_back(str);
      data.push_back(lineArray);
    }
}

bool WriteCsvFile(std::vector<std::vector<std::string> > data, std::string fileName)
{
    std::ofstream outFile;
    outFile.open(fileName.c_str(), std::ios::out);
    for(int i = 0;i<data.size();i++)
    {
      for(int j = 0;i<data.at(i).size();j++)
      {
        outFile<<data.at(i).at(j)<<',';
      }
      outFile<<std::endl;
    }
    outFile.close();
}

bool StringToFloat(std::vector<std::vector<std::string> > data_in, std::vector<std::vector<float> > &data_out)
{
  for(int i =0;i<data_in.size();i++)
  {  std::vector<float> tem_data;
    for(int j=0;j<data_in.at(i).size();j++)
    {
      string str_data = data_in.at(i).at(j);
      tem_data.push_back(stringToNum<float>(str_data));
    }
    data_out.push_back(tem_data);
  }
}
