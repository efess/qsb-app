using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSB.Server
{
    public class PlayerData
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }

        public string NameBase64 { get; set;}
        public int UpTime { get; set; }
        public int JoinTime { get; set; }
        public int CurrentFrags { get; set; }
        public int TotalFrags { get; set; }
        public decimal FragsPerMinute { get; set; }
        public int Shirt { get; set; }
        public int Pant { get; set; }
        public string Model { get; set; }
        public string Skin { get; set; }

    }
}
