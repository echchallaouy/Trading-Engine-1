using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTradingApplication.Core
{
    interface IConsoleTradingApplication
    {
        void ExitApplication();
        void Run();
        void ShowHelp();
    }
}
