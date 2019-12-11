using SmartQuant;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenQuant
{
    /// <summary>
    /// 一定要先加载投资组合，才加载委托
    /// </summary>
    public class QuantFileHelper
    {
        private Framework framework;
        private int last_order_id;
        // 由于SMACrossover中有OCA单，导致关联撤单报错，所以跳过一定范围
        private int skip_oca = 4;
        public QuantFileHelper(Framework framework)
        {
            this.framework = framework;
        }

        public void Close()
        {
            framework.InstrumentServer.Close();
            framework.PortfolioServer.Close();
            framework.OrderServer.Close();
        }

        public void Dispose()
        {
            framework.InstrumentServer.Dispose();
            framework.PortfolioServer.Dispose();
            framework.OrderServer.Dispose();
        }

        public IEnumerable<Instrument> LoadInstrumentsFromFramework()
        {
            framework.InstrumentManager.Load();
            return framework.InstrumentManager.Instruments;
        }

        public Portfolio LoadPortfolioFromFramework(string strategy_name)
        {
            return framework.PortfolioManager.Load(strategy_name);
        }

        public List<ExecutionMessage> LoadMessagesFromFramework(string strategy_name)
        {
            framework.OrderManager.Load(strategy_name);
            var msgs = framework.OrderManager.Messages;
            UpdateLastOrderId(msgs);
            return msgs;
        }

        public IEnumerable<Instrument> LoadInstrumentsFromFile(string filename)
        {
            framework.InstrumentServer = new FileInstrumentServer(framework, filename);
            framework.InstrumentServer.Open();
            return LoadInstrumentsFromFramework();
        }

        public Portfolio LoadPortfolioFromFile(string strategy_name, string filename)
        {
            framework.PortfolioServer = new FilePortfolioServer(framework, filename);
            framework.PortfolioServer.Open();
            return LoadPortfolioFromFramework(strategy_name);
        }

        public List<ExecutionMessage> LoadMessagesFromFile(string strategy_name, string filename)
        {
            framework.OrderServer = new FileOrderServer(framework, filename);
            framework.OrderServer.Open();
            return LoadMessagesFromFramework(strategy_name);
        }

        /// <summary>
        /// 注意：订单信息原本机制是追加写入
        /// </summary>
        /// <param name="strategy_name"></param>
        /// <param name="filename"></param>
        /// <param name="messages"></param>
        public void AppendMessagesToFile(string strategy_name, string filename, List<ExecutionMessage> messages, bool isAppend)
        {
            if(!isAppend)
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }

            var server = new FileOrderServer(framework, filename);
            server.Open();
            server.SeriesName = strategy_name;

            foreach (var m in messages)
            {
                if (m.TypeId == EventType.ExecutionCommand)
                {
                    server.Save(m);
                }
                else if (m.TypeId == EventType.ExecutionReport)
                {
                    server.Save(m);
                }
                else
                {
                    server.Save(m);
                }
            }
            server.Close();
        }

        public void SavePortfolioToFile(string strategy_name, string filename, Portfolio portfolio)
        {
            var server = new FilePortfolioServer(framework, filename);
            server.Open();
            server.Save(portfolio);
            server.Close();
        }

        public void SaveInstrumentsToFile(string filename, IEnumerable<Instrument> instruments)
        {
            var server = new FileInstrumentServer(framework, filename);
            server.Open();
            foreach (var i in instruments)
            {
                server.Save(i);
            }
            server.Close();
        }

        /// <summary>
        /// 演示如何删除一个定单，以及对应的回报
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public List<ExecutionMessage> RemoveByOrderId(List<ExecutionMessage> messages, int order_id)
        {
            return messages.Where(m => m.OrderId != order_id).ToList();
        }

        /// <summary>
        /// 更新可用的ID号
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public int UpdateLastOrderId(List<ExecutionMessage> messages)
        {
            last_order_id = messages.Max(x=>x.OrderId) + skip_oca;
            return last_order_id;
        }

        /// <summary>
        /// 添加定单并全成
        /// 主要用于改变仓位
        /// 
        /// 可将这个时间的记录改成16点，表示此单是人工修改
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="datetime"></param>
        /// <param name="portfolio"></param>
        /// <param name="instrument"></param>
        /// <param name="type"></param>
        /// <param name="side"></param>
        /// <param name="subSide"></param>
        /// <param name="qty"></param>
        /// <param name="price"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<ExecutionMessage> AddOrder(List<ExecutionMessage> messages, DateTime datetime,
            string portfolio,
            string instrument, OrderType type, OrderSide side, SubSide subSide,
            double qty, double price = 0, string text = "Manual"
            )
        {
            // 撤单与OCA单混在一起，会导致出错
            // 一起撤单时，正好撤到下一个的OCA单
            var cmd = _AddCommand(datetime, portfolio, instrument, type, side, subSide, qty, price, text);
            messages.Add(cmd);
            var rpt = _AddReport(cmd);
            messages.Add(rpt);
            return messages;
        }

        private ExecutionCommand _AddCommand(DateTime datetime,
            string portfolio,
            string instrument, OrderType type, OrderSide side, SubSide subSide,
            double qty, double price = 0, string text = "")
        {
            var _provider = framework.ExecutionSimulator;
            var _portfolio = framework.PortfolioManager.GetByName(portfolio);
            var _instrument = framework.InstrumentManager.Instruments[instrument];

            var order = new Order(_provider, _portfolio, _instrument, type, side, qty, price);
            order.SubSide = subSide;
            order.Id = ++last_order_id;
            order.DateTime = datetime;
            order.Text = text;
            var result = new ExecutionCommand(ExecutionCommandType.Send, order);
            return result;
        }

        private ExecutionReport _AddReport(ExecutionCommand command)
        {
            var result = new ExecutionReport(command);
            result.DateTime = command.DateTime;
            result.ExecType = ExecType.ExecTrade;
            result.OrdStatus = OrderStatus.Filled;
            result.LastQty = command.Qty;
            result.LastPx = command.Price;
            result.AvgPx = command.Price;
            return result;
        }
    }
}

