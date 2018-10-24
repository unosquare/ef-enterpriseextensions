namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample
{
    using System.Threading.Tasks;
    using Database;

    internal class TestController : BusinessRulesController<SampleDb>
    {
        public TestController(SampleDb context) : base(context)
        {
        }

        [BusinessRule(typeof (Order), ActionFlags.Create)]
        public async Task ChangeOrderCity(Order order)
        {
            await Task.Delay(1);
            // Change the city to NYC always
            order.ShipperCity = "NYC";
        }
    }
}
