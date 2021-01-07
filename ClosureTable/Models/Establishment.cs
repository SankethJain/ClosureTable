using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClosureTable.Models
{
    public class Establishment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<EstablishmentNode> Ancestors { get; set; }
        public virtual ICollection<EstablishmentNode> Offspring { get; set; }
    }
}
