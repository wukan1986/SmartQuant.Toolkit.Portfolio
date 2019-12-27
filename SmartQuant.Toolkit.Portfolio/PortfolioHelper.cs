using CsvHelper;
using SmartQuant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQuant.Toolkit.Portfolio
{
    public class PositionItem
    {
        public string Parent { get; set; }
        public string PortfolioName { get; set; }
        public string Symbol { get; set; }

        public PositionSide Side { get; set; }

        public double Qty { get; set; }

        public double EntryPrice { get; set; }
        public DateTime EntryDate { get; set; }

        public override string ToString()
        {
            return $"{Parent},{PortfolioName},{Symbol},{Side},{Qty},{EntryPrice},{EntryDate:yyyy-MM-dd HH:mm:ss}";
        }
    }

    public class PortfolioHelper
    {
        public static void AddPosition(
            Framework framework,
            SmartQuant.Portfolio pf,
            DateTime dateTime,
            Instrument instrument, byte currencyId,
            OrderSide side, SubSide subSide,
            double qty, double price,
            string text)
        {
            // 创建Order
            var order = new Order(null, pf, instrument, OrderType.Limit, side, qty, price, 0, TimeInForce.Day, 0, text);
            order.SubSide = subSide;
            // 想记录下来就得注册
            framework.OrderManager.Register(order);

            // 模拟下单
            var executionCommand = new ExecutionCommand(ExecutionCommandType.Send, order);
            executionCommand.DateTime = dateTime;
            executionCommand.IsLoaded = true;
            framework.OrderManager.Messages.Add(executionCommand);
            order.OnExecutionCommand(executionCommand);
            framework.EventServer.OnEvent(executionCommand);

            // 直接保存不管用
            //if (framework.StrategyManager.Persistence == StrategyPersistence.Save || framework.StrategyManager.Persistence == StrategyPersistence.Full)
            //{
            //    framework.OrderServer.Save(executionCommand);
            //}

            // 模拟成交
            var executionReport = new ExecutionReport(executionCommand);
            executionReport.OrdStatus = OrderStatus.Filled;
            executionReport.ExecType = ExecType.ExecTrade;
            executionReport.DateTime = dateTime;
            executionReport.LastPx = price;
            executionReport.LastQty = qty;
            executionReport.LeavesQty = 0;
            executionReport.CumQty = qty;
            executionReport.IsLoaded = true;
            // 成交不需要额外添加
            // framework.OrderManager.Messages.Add(executionReport);
            order.OnExecutionReport(executionReport);
            framework.EventServer.OnEvent(executionReport);
        }

        public static void AddPosition2(
            Framework framework,
            SmartQuant.Portfolio pf,
            DateTime dateTime,
            Instrument instrument, byte currencyId,
            OrderSide side, SubSide subSide,
            double qty, double price,
            string text)
        {
            // 强行改变Portfolio，无法存盘，所以被淘汰

            var order = new Order(null, pf, instrument, OrderType.Limit, side, qty, price, 0, TimeInForce.Day, 0, text);
            order.SubSide = subSide;

            var er = new ExecutionReport(order);
            er.DateTime = dateTime;
            er.CurrencyId = currencyId;
            er.LastQty = qty;
            er.LastPx = price;
            er.Text = text;
            var f = new Fill(er);
            pf.Add(f);
        }

        public static void AddAccountValue(
            Framework framework,
            SmartQuant.Portfolio pf,
            DateTime dateTime,
            byte currencyId, double old_value, double new_value, string text)
        {
            double diff = new_value - old_value;
            if (diff == 0)
                return;

            pf.Account.Deposit(dateTime, diff, currencyId, text);

            Console.WriteLine("Portfolio:{3};AccountValue: {0} + {1} = {2}", old_value, diff, new_value, text);
        }

        public static void AddLongPosition(
            Framework framework,
            SmartQuant.Portfolio pf,
            DateTime dateTime,
            Instrument instrument, byte currencyId,
            double old_qty, double new_qty,
            double price,
            string text)
        {
            double diff = new_qty - old_qty;
            if (diff == 0)
                return;

            if (diff > 0)
            {
                AddPosition(framework, pf, dateTime, instrument, currencyId, OrderSide.Buy, SubSide.Undefined, diff, price, "Long+" + text);
            }
            else
            {
                AddPosition(framework, pf, dateTime, instrument, currencyId, OrderSide.Sell, SubSide.Undefined, -diff, price, "Long-" + text);
            }

            Console.WriteLine("Portfolio:{0};Instrument:{1};LongPositionQty:{2} + {3} = {4}",
                    pf.Name, instrument.Symbol, old_qty, diff, new_qty);
        }

        public static void AddShortPosition(
            Framework framework,
            SmartQuant.Portfolio pf,
            DateTime dateTime,
            Instrument instrument, byte currencyId,
            double old_qty, double new_qty,
            double price,
            string text)
        {
            double diff = new_qty - old_qty;
            if (diff == 0)
                return;

            if (diff > 0)
            {
                // 数量变多是开仓
                AddPosition(framework, pf, dateTime, instrument, currencyId, OrderSide.Sell, SubSide.SellShort, diff, price, "Short+" + text);
            }
            else
            {
                AddPosition(framework, pf, dateTime, instrument, currencyId, OrderSide.Buy, SubSide.BuyCover, -diff, price, "Short-" + text);
            }

            Console.WriteLine("Portfolio:{0};Instrument:{1};ShortPositionQty:{2} + {3} = {4}",
                    pf.Name, instrument.Symbol, old_qty, diff, new_qty);
        }

        public static List<T> from_csv<T>(string path)
        {
            List<T> list = new List<T>();

            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    using (var csv = new CsvReader(sr))
                    {
                        // Dynamic
                        var records = csv.GetRecords<T>();

                        foreach (var r in records)
                        {
                            list.Add(r);
                        }
                    }
                }
            }

            return list;
        }

        public static void to_csv<T>(string path, List<T> list)
        {
            // 保存时只根据index来，很有可能每列对应数据位置不对，需要修正
            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    using (var csv = new CsvWriter(sw))
                    {
                        csv.WriteRecords(list);
                    }
                }
            }
        }
    }
}


/*

// 强行改变Portfolio，无法存盘，所以被淘汰
var er = new ExecutionReport(order);
er.DateTime = dateTime;
er.CurrencyId = currencyId;
er.LastQty = qty;
er.LastPx = price;
er.Text = text;
var f = new Fill(er);
pf.Add(f);

pf.Account.Add(dateTime, diff, currencyId, text);
     
*/
