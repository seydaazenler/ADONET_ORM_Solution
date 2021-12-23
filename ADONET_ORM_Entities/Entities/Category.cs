using ADONET_ORM_Commonn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Entities.Entities
{
    [Table(PrimaryColumn ="CategoryID", tableName = "Categories", IdentityColumn = "CategoryID")]
    public class Category
    {
        //property isimleri SQL'deki tablolarla aynı
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public override string ToString()
        {
            return CategoryName;
        }
    }
}
