using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effort;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;
using Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample.Database;

namespace Unosquare.Labs.EntityFramework.EnterpriseExtensions.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var console = new ClientConsole();
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            var connection = DbConnectionFactory.CreateTransient();

            using (var context = new SampleDb(connection, "geo"))
            {
                console.WriteLine("Welcome to EF Test Console, type help to check available commands",
                    OutputLevel.Information, null);

                context.Products.AddRange(new[]
                {
                    new Product {Name = "CocaCola"},
                    new Product {Name = "Pepsi"},
                    new Product {Name = "Starbucks"},
                    new Product {Name = "Donut"}
                });

                context.SaveChanges();

                var ct = new System.Threading.CancellationToken();
                Task.Run(() => { SampleJob.Instance.RunBackgroundWork(ct); }, ct);

                var command = new RootCommand(console);

                command.RegisterCommand(new FillOrderCommand(context));
                command.RegisterCommand(new EditOrder(context));
                command.RegisterCommand(new QueryAuditTrail(context));
                command.RegisterCommand(new QueryOrder(context));
                command.RegisterCommand(new ToggleController(context));
                command.RegisterCommand(new JobController());

                var commandEngine = new CommandEngine(command);

                commandEngine.Run(args);
            }
        }

        internal class JobController : ActionCommandBase
        {
            public JobController()
                : base("job", "Check Job")
            {
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Job Status: {0}", SampleJob.Instance.IsRunning);

                return true;
            }
        }

        internal class ToggleController : ActionCommandBase
        {
            private SampleDb _context;
            private static TestController controller;

            public ToggleController(SampleDb context)
                : base("toggle", "Toggle test controller")
            {
                _context = context;

                if (controller == null)
                    controller = new TestController(_context);
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Toggling controller");
                OutputInformation("Test controller will change orders city to 'NYC'");

                if (_context.ContainsController(controller))
                {
                    OutputInformation("Controller is off");
                    _context.RemoveController(controller);
                }
                else
                {
                    OutputInformation("Controller is on");
                    _context.AddController(controller);
                }

                return true;
            }
        }

        internal class QueryOrder : ActionCommandBase
        {
            private SampleDb _context;

            public QueryOrder(SampleDb context)
                : base("order", "Check last Order")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Last Order");

                var lastitem = _context.Orders.OrderByDescending(x => x.OrderID).FirstOrDefault();

                if (lastitem != null)
                {
                    OutputInformation("OrderID {0}", lastitem.OrderID);
                    OutputInformation("OrderType {0}", lastitem.OrderType);
                    OutputInformation("ShippedDate {0}", lastitem.ShippedDate);
                    OutputInformation("ShipperCity {0}", lastitem.ShipperCity);
                }

                return true;
            }
        }

        internal class QueryAuditTrail : ActionCommandBase
        {
            private SampleDb _context;

            public QueryAuditTrail(SampleDb context)
                : base("audit", "Check last AuditTrail entry")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Last Audit Trail");

                var lastitem = _context.AuditTrailEntrys.OrderByDescending(x => x.AuditId).FirstOrDefault();

                if (lastitem != null)
                {
                    OutputInformation("UserId {0}", lastitem.UserId);
                    OutputInformation("TableName {0}", lastitem.TableName);
                    OutputInformation("Action {0}", lastitem.Action);
                    OutputInformation("JsonBody {0}", lastitem.JsonBody);
                }

                return true;
            }
        }

        internal class EditOrder : ActionCommandBase
        {
            private SampleDb _context;

            public EditOrder(SampleDb context)
                : base("edit", "Edit last Order entry")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Last Order");

                var lastitem = _context.Orders.OrderByDescending(x => x.OrderID).FirstOrDefault();

                if (lastitem != null)
                {
                    lastitem.ShipperCity = "Modified";
                    _context.SaveChanges();
                }

                return true;
            }
        }

        internal class FillOrderCommand : ActionCommandBase
        {
            private SampleDb _context;

            public FillOrderCommand(SampleDb context)
                : base("fillorder", "Add some orders to database")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                var shipperCities = new[]
                {
                    "Guadalajara, JAL, Mexico", "Los Angeles, CA, USA", "Portland, OR, USA", "Leon, GTO, Mexico",
                    "Boston, MA, USA"
                };

                var companies = new[]
                {
                    "Unosquare LLC", "Advanced Technology Systems", "Super La Playa", "Vesta", "Microsoft", "Oxxo",
                    "Simian"
                };

                var rand = new Random();
                var products = _context.Products.ToArray();

                var order = new Order
                {
                    CustomerName = companies[rand.Next(companies.Length - 1)],
                    IsShipped = rand.Next(10) > 5,
                    ShipperCity = shipperCities[rand.Next(shipperCities.Length - 1)],
                    ShippedDate = DateTime.Now.AddDays(1 - rand.Next(10)),
                    OrderType = rand.Next(30),
                    Amount = 10
                };

                //for (var k = 0; k < rand.Next(10); k++)
                //{
                //    order.Details.Add(new OrderDetail
                //    {
                //        Price = rand.Next(10),
                //        Description = "Product ID" + rand.Next(1000),
                //        Quantity = rand.Next(10),
                //        ProductID = products[rand.Next(products.Length - 1)].ProductID
                //    });
                //}

                // order.Amount = order.Details.Sum(x => x.Price*x.Quantity);

                OutputInformation("OrderID {0}", order.OrderID);
                OutputInformation("OrderType {0}", order.OrderType);
                OutputInformation("ShippedDate {0}", order.ShippedDate);
                OutputInformation("ShipperCity {0}", order.ShipperCity);

                _context.Orders.Add(order);

                _context.SaveChanges();

                OutputInformation("After save");

                OutputInformation("OrderID {0}", order.OrderID);
                OutputInformation("OrderType {0}", order.OrderType);
                OutputInformation("ShippedDate {0}", order.ShippedDate);
                OutputInformation("ShipperCity {0}", order.ShipperCity);

                return true;
            }
        }
    }
}