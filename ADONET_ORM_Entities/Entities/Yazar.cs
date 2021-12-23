using ADONET_ORM_Commonn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Entities.Entities
{
    [Table(tableName ="Yazarlar", IdentityColumn ="YazarId", PrimaryColumn ="YazarId")]
    public class Yazar
    {
        
        public int YazarId { get; set; }
        public DateTime KayitTarihi { get; set; }
        public string YazarAdSoyad { get; set; }
        public bool SilindiMi { get; set; }
     


    }
}
