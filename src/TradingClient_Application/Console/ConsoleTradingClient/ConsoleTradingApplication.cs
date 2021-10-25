using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingClient.Core;

namespace ConsoleTradingApplication.Core
{
    class ConsoleTradingApplication : IConsoleTradingApplication
    {
        public ConsoleTradingApplication(ITradingClient tradingClient)
        {
            _optionSet = new OptionSet()
            {
                { "s|start", "Start the trading client", _ => tradingClient.StartTradingClient() },
                { "e|end", "Stop the trading client", _ => tradingClient.StopTradingClient() },
                { "i|inquire", "Inquire as to whether the trading client is active", _ => Console.WriteLine($"{(tradingClient.IsOnline() ? "Online" : "Offline")}")},
                { "n|new=", "Submit a new order", async s => await tradingClient.SubmitNewOrderAsync(CommandLineConverter.CreateNewOrderRequest(s))},
                { "c|cancel=", "Submit a cancel order", async s => await tradingClient.SubmitCancelOrderAsync(CommandLineConverter.CreateCancelOrderRequest(s))},
                { "m|modify=", "Submit a modify order", async s => await tradingClient.SubmitModifyOrderAsync(CommandLineConverter.CreateModifyOrderRequest(s))},
                { "f|fills", "Get a list of your fills", _ => Console.WriteLine($"{string.Join("\n ", tradingClient.GetFills())}")},
                { "p|positions", "Get a list of your positions", _ => Console.WriteLine($"{string.Join("\n ", tradingClient.GetOutstandingPositions())}")},
                { "t|trades", "Get a list of your trades", _ => Console.WriteLine($"{string.Join("\n ", tradingClient.GetTrades())}")},
                { "h|help", "Show the help message that clarifies how to use this application", _ => this.ShowHelp() },
                { "e|exit", "Exit the application", _ => this.ExitApplication() },
            };
            ShowHelp();
        }

        public void ExitApplication()
        {
            throw new ApplicationException();
        }

        public void Run()
        {
            while (true)
            {
                // Collect user input.
                string line = Console.ReadLine();
                try
                {
                    List<string> unknownInputs = _optionSet.Parse(line.Split(' '));
                    if (unknownInputs.Any())
                    {
                        Console.WriteLine($"Unknown commands: {string.Join(", ", unknownInputs)}");
                        ShowHelp();
                    }
                }
                catch (ApplicationException)
                {
                    Console.WriteLine("Exiting application");
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void ShowHelp()
        {
            // Fill in later.
            // Console.WriteLine(@"");
            throw new NotImplementedException();
        }

        private readonly OptionSet _optionSet;
    }
}
