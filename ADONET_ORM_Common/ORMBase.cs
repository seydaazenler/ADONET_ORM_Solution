using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Common
{
    public class ORMBase<ET, OT> : IORM<ET>
        where ET : class, new()
        where OT : class, new()
    {
        public bool Delete(ET Entity)
        {
            throw new NotImplementedException();
        }

        public bool Insert(ET Entity)
        {
            throw new NotImplementedException();
        }

        public List<ET> Select()
        {
            Type type = typeof(ET);
            string querySentence = "Select * from ";
            var attributes = type.GetCustomAttributes(typeof(Table),false);
            if (attributes!=null && attributes.Any())
            {
                Table tbl = (Table)attributes[0];
                querySentence += tbl.tableName; //select * from Kitaplar
            }
            SqlCommand command = new SqlCommand(querySentence, Tools.MySqlDBConnection);
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            //geçici return
            return new List<ET>();
        }

        public bool Update(ET Entity)
        {
            throw new NotImplementedException();
        }
    }
}
