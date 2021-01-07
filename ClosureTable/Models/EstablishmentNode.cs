using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClosureTable.Models
{
    public class EstablishmentNode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int AncestorId { get; set; }
        public virtual Establishment Ancestor { get; set; }

        public int OffspringId { get; set; }
        public virtual Establishment Offspring { get; set; }

        public int Separation { get; set; }
    }
}
