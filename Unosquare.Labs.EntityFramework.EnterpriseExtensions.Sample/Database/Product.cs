using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        public string Name { get; set; }
    }
}
