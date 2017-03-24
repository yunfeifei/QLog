using System;

namespace QLog.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetLogger();
            for (int i = 0; i < 1000; i++)
            {
                logger.Debug("Debug message!");
                logger.Trace("Trace message!");
                logger.Info("Info message!");
                logger.Error("Error message!");
                logger.Fatal("Fatal message!");
                logger.Warn("Fatal message!");
            }
            
            Console.Read();
        }
    }
}
