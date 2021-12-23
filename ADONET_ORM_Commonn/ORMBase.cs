﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Commonn
{
    public class ORMBase<ET, OT> : IORM<ET>
        where ET : class, new()
        where OT : class, new()
    {

        public Type ETType {
            get 
            {
                return typeof(ET);
            }
        }

        public Table TheTable { 
            get
            {
                var attributes = ETType.GetCustomAttributes(typeof(Table), false);
                if (attributes!=null && attributes.Any())
                {
                    Table theTable =(Table)attributes[0];
                    return theTable;
                }
                return null;
            }
        }

        private static OT _current;
        
        public static OT Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new OT();
                }
                return _current;
            }
        }


        public bool Delete(ET Entity)
        {
            int theID = 0;
            PropertyInfo[] properties = ETType.GetProperties();
            foreach (var item in properties)
            {
                if (item.Name == TheTable.PrimaryColumn)
                {
                    theID = (int)item.GetValue(Entity);
                }
            }
            
            string query = $"Delete from {TheTable.tableName} where {TheTable.PrimaryColumn}={theID} ";

            ////1.yol
            //SqlCommand cmd = new SqlCommand(query, Tools.MySqlDBConnection);
            //Tools.OpenTheConnection();

            //int affectedRows = cmd.ExecuteNonQuery();
            //Tools.MySqlDBConnection.Close();

            //2.yol
            int affectedRows = 0;
            using (Tools.MySqlDBConnection)
            {
                SqlCommand cmd = new SqlCommand(query, Tools.MySqlDBConnection);
                Tools.OpenTheConnection();
                affectedRows = cmd.ExecuteNonQuery();
            }
            
            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Insert(ET Entity)
        {
            string query = "";
            string tableName = "";
            string properties = "";
            string values = "";

            PropertyInfo[] propertyArray = ETType.GetProperties();
            SqlCommand cmd = new SqlCommand();

            //insert into sorgu cümlesi için tablodaki Column'ları oluşturacağız

            foreach (var item in propertyArray)
            {
                if (item.Name ==  TheTable.IdentityColumn)
                {
                    continue; //örneğin KitapID gibi kolonlar insert cümlesine eklenmez
                }
                else
                {
                    properties += item.Name + ","; //KitapAdi,Stok,SayfaSayisi...
                }
            }
            properties = properties.TrimEnd(',');

            //insert into (properties) values () values içine alınacak kısmı yazalım

            foreach (var item in propertyArray)
            {
                if (item.Name != TheTable.IdentityColumn)
                {


                    if (item.GetValue(Entity)==null)
                    {
                        values += "null, ";
                    }
                    else if (item.PropertyType.Name.Contains("DateTime"))
                    {
                        DateTime theDateTime;
                        DateTime.TryParse(item.GetValue(Entity).ToString(), out theDateTime);
                        string theDateTimeString = "'" + theDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                        values += theDateTimeString + ",";
                    }
                    else if (item.PropertyType.Name.Contains("Bool"))
                    {
                        bool deger = (bool)item.GetValue(Entity);
                        string degerString = deger ? "1" : "0";
                        values += degerString + ",";
                    }
                    else if (item.PropertyType.Name.Contains("String"))
                    {
                        //'Suç ve Ceza',
                        values += $"'{item.GetValue(Entity)}',";

                    }
                    else
                    {
                        values += item.GetValue(Entity) + ",";
                    }
                }
            }

            values = values.TrimEnd(',');

            //insert into () values ()
            tableName = TheTable.tableName;
            query = $"insert into { tableName} ({properties}) values ({values})";

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.Connection = Tools.MySqlDBConnection;
            Tools.OpenTheConnection();
            int affectedRows = cmd.ExecuteNonQuery();
            Tools.MySqlDBConnection.Close();

            if (affectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<ET> Select()
        {
            Type type = typeof(ET);
            string querySentence = "Select * from ";
            var attributes = type.GetCustomAttributes(typeof(Table), false);
            if (attributes != null && attributes.Any())
            {
                Table tbl = (Table)attributes[0];
                querySentence += tbl.tableName; //select * from Kitaplar
            }
            SqlCommand command = new SqlCommand(querySentence, Tools.MySqlDBConnection);
            Tools.OpenTheConnection();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            //dataTable nesneleri ToList isimli metodu kullanabiliyorlar.
            //Yani ToList<> generic yapılı metot datatable nesnesi Extension(ilave ek) metot oldu.
            //Yani normalde dt nesnesinin böyle bir metodu yok.
            //Ama siz yazılımcı olarak bir metot yazıp o metodu systemdeki datatable vb nesne kullansın isterseniz metot binding özelliğini kullanarak extension metotlar yazabilirsiniz.

            return dt.ToList<ET>();
        }

        public bool Update(ET Entity)
        {
            string query = "";
            string sets = "";

            PropertyInfo[] propertyArray = ETType.GetProperties();
            foreach (var item in propertyArray)
            {
                if (item.Name == TheTable.IdentityColumn)
                {
                    continue;
                }
                if (item.GetValue(Entity) == null)
                {
                    sets += item.Name + "=null,";
                }
                else if (item.PropertyType.Name.Contains("DateTime"))
                {
                    DateTime theDate;
                    DateTime.TryParse(item.GetValue(Entity).ToString(),out theDate);
                    string theDateString = $"'{theDate.ToString("yyyy-MM-dd HH:mm:ss")}'";

                    //örneğin OdunBitisTarihi='2021-12-23 09:30:21',
                    sets += item.Name + "=" + theDateString + ",";
                }
                else if (item.PropertyType.Name.Contains("Bool"))
                {
                    bool deger = (bool)item.GetValue(Entity);
                    string degerString = deger ? "1" : "0";
                    sets += $"{item.Name} = {degerString},";
                }
                else if (item.PropertyType.Name.Contains("String") || item.PropertyType.Name.Contains("Char"))
                {
                    //örneğin KitapAdı = 'Suç ve Ceza',
                    sets += $"{item.Name} = '{item.GetValue(Entity)}',";
                }
                else
                {
                    //örneğin SayfaSayisi = 208,
                    sets += $"{item.Name} = {item.GetValue(Entity)},";
                }
            }

            sets = sets.TrimEnd(',');

            query = $"update {TheTable.tableName} set {sets} ";
            //where koşuluna ihtiyacımız var alanları sınırlandırmamız gerek

            foreach (var item in propertyArray)
            {
                if (item.Name == TheTable.PrimaryColumn)
                {
                    if (item.PropertyType.Name.Contains("string") || item.PropertyType.Name.Contains("Char"))
                    {

                        //örneğin where 'CategoryID = 'ALFKI'
                        query += $"where {item.Name}='{item.GetValue(Entity)}'";
                    }
                    else
                    {
                        //örneğin where KitapID =1
                        query += $"where {item.Name}={item.GetValue(Entity)}";
                    }
                    break;
                }
            }

            int affectedRows = 0;
            using (Tools.MySqlDBConnection)
            {
                SqlCommand cmd = new SqlCommand(query,Tools.MySqlDBConnection);
                Tools.OpenTheConnection();
                affectedRows = cmd.ExecuteNonQuery();
            }



            return affectedRows > 0 ? true : false;
        }

        public ET SelectET(int etID)
        {
            string query = string.Empty;
            var attributes = ETType.GetCustomAttributes(typeof(Table), false);
            if (attributes!=null && attributes.Any())
            {
                Table tbl = attributes[0] as Table;
                query = $"Select * from {tbl.tableName} where {tbl.PrimaryColumn}={etID}";
            }
            DataTable dt = new DataTable();

            using (Tools.MySqlDBConnection)
            {
                SqlCommand cmd = new SqlCommand(query, Tools.MySqlDBConnection);
                Tools.OpenTheConnection();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                
                adapter.Fill(dt);
            }


            return dt.toET<ET>();
        }

      

    }
}


