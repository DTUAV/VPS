#include "../include/kf_mpc_controller4/file_operator.h"
int main(int argc, char *argv[])
{
    std::string data_file_name = "test_data.csv";
    std::vector<std::vector<std::string> > data;
    ReadCsvFile(data,data_file_name);
    std::cout<<data.size()<<std::endl;
    std::cout<<data.at(0).size()<<std::endl;
    std::vector<vector<float> > float_data;
    cout<<data.at(0).at(0)<<" string"<<endl;
    StringToFloat(data,float_data);
    cout<<float_data.size()<<endl;
    cout<<float_data.at(0).at(0)<<" "<<float_data.at(0).at(1)<<" "<<float_data.at(0).at(2)<<endl;
    return 0;
}
