using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimUnity.Sensor
{
    public class ContactSensorData
    {
        private string ContactName;
        private double ContactDir_x;
        private double ContactDir_y;
        private double ContactDir_z;
        private double ContactWorldPoint_x;
        private double ContactWorldPoint_y;
        private double ContactWorldPoint_z;
        public ContactSensorData(string contactName, double contactDir_x, double contactDir_y, double contactDir_z, double contactWorldPoint_x, double contactWorldPoint_y, double contactWorldPoint_z)
        {
            this.ContactName = contactName;
            this.ContactDir_x = contactDir_x;
            this.ContactDir_y = contactDir_y;
            this.ContactDir_z = contactDir_z;
            this.ContactWorldPoint_x = contactWorldPoint_x;
            this.ContactWorldPoint_y = contactWorldPoint_y;
            this.ContactWorldPoint_z = contactWorldPoint_z;

        }

        public ContactSensorData()
        {

        }
        public string getContactName()
        {
            return this.ContactName;

        }
        public void setContactName(string contactName)
        {
            this.ContactName = contactName;
        }
        public double getContractDir_x()
        {
            return this.ContactDir_x;
        }

        public double getContractDir_y()
        {
            return this.ContactDir_y;
        }

        public double getContractDir_z()
        {
            return this.ContactDir_z;
        }
        public void setContractDir_x(double contractDir_x)
        {
            this.ContactDir_x = contractDir_x;
        }

        public void setContractDir_y(double contractDir_y)
        {
            this.ContactDir_y = contractDir_y;
        }

        public void setContractDir_z(double contractDir_z)
        {
            this.ContactDir_z = contractDir_z;
        }
        public double getContactWorldPoint_x()
        {
            return this.ContactWorldPoint_x;
        }
        public double getContactWorldPoint_y()
        {
            return this.ContactWorldPoint_y;
        }

        public double getContactWorldPoint_z()
        {
            return this.ContactWorldPoint_z;
        }

        public void setContactWorldPoint_x(double contactWorldPoint_x)
        {
            this.ContactWorldPoint_x = contactWorldPoint_x;
        }
        public void setContactWorldPoint_y(double contactWorldPoint_y)
        {
            this.ContactWorldPoint_y = contactWorldPoint_y;
        }
        public void setContactWorldPoint_z(double contactWorldPoint_z)
        {
            this.ContactWorldPoint_z = contactWorldPoint_z;
        }
    }
}