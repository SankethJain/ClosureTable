using ClosureTable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClosureTable
{
    class Program
    {
        static void Main(string[] args)
        {
            var svc = new EstablishmentService();

            var establishments = svc.GetAll().GetAwaiter().GetResult();


            //var e = establishments.Where(x => x.ID == 7).FirstOrDefault();
            //var e1 = new Establishment()
            //{
            //    Name = "sa"
            //};
            //svc.Insert(e1, e).Wait();

            //var e = establishments.Where(x => x.ID == 7).FirstOrDefault();
            //svc.Delete(e).Wait();

            var e1 = establishments.Where(x => x.ID == 6).FirstOrDefault();
            var newParent = establishments.Where(x => x.ID == 3).FirstOrDefault();
            svc.SimpleMove(e1, newParent).Wait();
        }
    }
}
