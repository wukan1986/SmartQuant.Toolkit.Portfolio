using System;

using System.IO;
using System.Linq;

using SmartQuant;

namespace OpenQuant
{
    class Program
    {
        /// <summary>
        /// 注意：Order是追加写入
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string strategy_name = "SMACrossover";

            string portfolios_input = @"C:\Users\Kan\AppData\Roaming\SmartQuant Ltd\OpenQuant 2014\data\portfolios_1.quant";
            string portfolios_output = @"C:\Users\Kan\AppData\Roaming\SmartQuant Ltd\OpenQuant 2014\data\portfolios_2.quant";

            string orders_input = @"C:\Users\Kan\AppData\Roaming\SmartQuant Ltd\OpenQuant 2014\data\orders_1.quant";
            string orders_output = @"C:\Users\Kan\AppData\Roaming\SmartQuant Ltd\OpenQuant 2014\data\orders_2.quant";

            string instruments_input = @"C:\Users\Kan\AppData\Roaming\SmartQuant Ltd\OpenQuant 2014\data\instruments_1.quant";
            string instruments_output = @"C:\Users\Kan\AppData\Roaming\SmartQuant Ltd\OpenQuant 2014\data\instruments_2.quant";


            Framework framework = Framework.Current;
            var helper = new QuantFileHelper(framework);
            // 释放原文件的锁定
            helper.Close();
            helper.Dispose();

            var instruments = helper.LoadInstrumentsFromFile(instruments_input);
            var portfolios = helper.LoadPortfolioFromFile(strategy_name, portfolios_input);
            var orders = helper.LoadMessagesFromFile(strategy_name, orders_input);

            Console.WriteLine(orders.Count);

            int order_id = 1;
            // 删除元素
            var msgs = orders.Where(m => m.OrderId == order_id).ToList();
            // 得到最新的ID
            helper.UpdateLastOrderId(msgs);

            Console.WriteLine(msgs.Count);

            {
                // 定单放在同名策略下
                msgs = helper.AddOrder(msgs, new DateTime(2019, 12, 20, 0, 0, 0), "SMACrossover (AAPL)", "AAPL", OrderType.Limit, OrderSide.Buy, SubSide.Undefined, 10, 100);
                // 定单放在其它策略下，不能使用this.Position取
                msgs = helper.AddOrder(msgs, new DateTime(2019, 12, 20, 0, 0, 0), "SMACrossover (MSFT)", "AAPL", OrderType.Limit, OrderSide.Sell, SubSide.Undefined, 10, 100);
            }


            helper.AppendMessagesToFile(strategy_name, orders_output, msgs, false);
            helper.SavePortfolioToFile(strategy_name, portfolios_output, portfolios);
            helper.SaveInstrumentsToFile(instruments_output, instruments);

            helper.Close();

            //new FileInfo(orders_output).CopyTo(orders_output.Replace("_2", ""), true);
            //new FileInfo(portfolios_output).CopyTo(portfolios_output.Replace("_2", ""), true);
            //new FileInfo(instruments_output).CopyTo(instruments_output.Replace("_2", ""), true);

            Console.ReadKey();
        }
    }
}
