using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
                Task.Run(() => { SingletonSampleJob.Instance.RunBackgroundWork(ct); }, ct);

                var command = new RootCommand(console);

                command.RegisterCommand(new FillOrderCommand(context));
                command.RegisterCommand(new EditOrder(context));
                command.RegisterCommand(new QueryAuditTrail(context));
                command.RegisterCommand(new QueryOrder(context));
                command.RegisterCommand(new ToggleController(context));
                command.RegisterCommand(new JobController(context));

                var commandEngine = new CommandEngine(command);

                commandEngine.Run(args);
            }
        }

        internal class JobController : ActionCommandBase
        {
            private readonly SampleDb _context;
            private static readonly List<SampleJob> Jobs = new List<SampleJob>();

            public JobController(SampleDb context)
                : base("job", "Check Job")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Job Status: {0}", SingletonSampleJob.Instance.IsRunning);

                if (SingletonSampleJob.Instance.IsRunning)
                {
                    OutputInformation("Running non-static job");
                    var job = new SampleJob(_context);
                    await job.RunAsync(null);
                    await Task.Delay(1000);
                    OutputInformation("Non-static Job Status: {0}", job.IsRunning);
                    Jobs.Add(job);
                }

                return true;
            }
        }

        internal class ToggleController : ActionCommandBase
        {
            private readonly SampleDb _context;
            private static TestController _controller;

            public ToggleController(SampleDb context)
                : base("toggle", "Toggle test controller")
            {
                _context = context;

                if (_controller == null)
                    _controller = new TestController(_context);
            }

            public override Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Toggling controller");
                OutputInformation("Test controller will change orders city to 'NYC'");

                if (_context.ContainsController(_controller))
                {
                    OutputInformation("Controller is off");
                    _context.RemoveController(_controller);
                }
                else
                {
                    OutputInformation("Controller is on");
                    _context.AddController(_controller);
                }

                return Task.FromResult(true);
            }
        }

        internal class QueryOrder : ActionCommandBase
        {
            private readonly SampleDb _context;

            public QueryOrder(SampleDb context)
                : base("order", "Check last Order")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Last Order");

                var lastitem = await _context.Orders.OrderByDescending(x => x.OrderID).FirstOrDefaultAsync();

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
            private readonly SampleDb _context;

            public QueryAuditTrail(SampleDb context)
                : base("audit", "Check last AuditTrail entry")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Last Audit Trail");

                var lastitem = await _context.AuditTrailEntries.OrderByDescending(x => x.AuditId).FirstOrDefaultAsync();

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
            private readonly SampleDb _context;

            public EditOrder(SampleDb context)
                : base("edit", "Edit last Order entry")
            {
                _context = context;
            }

            public override async Task<bool> InvokeAsync(string paramList)
            {
                OutputInformation("Last Order");

                var lastitem = await _context.Orders.OrderByDescending(x => x.OrderID).FirstOrDefaultAsync();

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
            private readonly SampleDb _context;

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

                var order = new Order
                {
                    CustomerName = companies[rand.Next(companies.Length - 1)],
                    IsShipped = rand.Next(10) > 5,
                    ShipperCity = shipperCities[rand.Next(shipperCities.Length - 1)],
                    ShippedDate = DateTime.Now.AddDays(1 - rand.Next(10)),
                    OrderType = rand.Next(30),
                    Amount = 10
                };

                OutputInformation("OrderID {0}", order.OrderID);
                OutputInformation("OrderType {0}", order.OrderType);
                OutputInformation("ShippedDate {0}", order.ShippedDate);
                OutputInformation("ShipperCity {0}", order.ShipperCity);

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

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