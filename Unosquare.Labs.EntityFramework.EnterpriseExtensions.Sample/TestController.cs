using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample
{
    class TestController : BusinessRulesController<SampleDb>
    {
        public TestController(SampleDb context) : base(context)
        {
        }

        [BusinessRule(typeof (Order), ActionFlags.Create)]
        public void ChangeOrderCity(Order order)
        {
            // Change the city to NYC always
            order.ShipperCity = "NYC";
        }
    }
}
