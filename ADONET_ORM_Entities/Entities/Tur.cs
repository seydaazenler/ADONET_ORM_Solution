using ADONET_ORM_Commonn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Entities.Entities
{
    [Table(tableName ="Turler",IdentityColumn ="TurId", PrimaryColumn ="TurId")]
    public class Tur
    {
       
        public int TurId { get; set; }
        public string TurAdi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }


               
   

    }
}
