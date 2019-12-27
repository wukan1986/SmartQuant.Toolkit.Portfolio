using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SmartQuant;
using SmartQuant.Controls;
using AdvancedDataGridView;
using OpenQuant;
using System.IO;

namespace SmartQuant.Toolkit.Portfolio.Controls
{

    public enum ShowType
    {
        All,
        PortfolioOnly,
        PositionOnly,
    }
    public partial class PortfolioControl : FrameworkControl
    {
        public const int IDX_PortfolioName = 0;
        public const int IDX_Symbol = 1;
        public const int IDX_Amount = 2;
        public const int IDX_Long = 3;
        public const int IDX_Short = 4;
        public const int IDX_AccountValue = 5;
        public const int IDX_EntryPrice = 6;
        public const int IDX_EntryDate = 7;

        private bool bAutoRefresh;

        // 主要用于从父节点中添加子节点时找到父节点
        private Dictionary<SmartQuant.Portfolio, TreeGridNode> portfolio_nodes = new Dictionary<SmartQuant.Portfolio, TreeGridNode>();
        // 主要用于修改节点时定位到对应的投资组合
        private Dictionary<TreeGridNode, object> nodes_position_portfolio = new Dictionary<TreeGridNode, object>();

        public PortfolioControl()
        {
            InitializeComponent();

            this.comboBox_show_type.Items.AddRange(Enum.GetNames(typeof(ShowType)));
            this.comboBox_persistence.Items.AddRange(Enum.GetNames(typeof(StrategyPersistence)));
        }

        protected override void OnInit()
        {
            this.comboBox_show_type.SelectedIndex = 0;
            this.comboBox_persistence.SelectedIndex = (int)framework.StrategyManager.Persistence;

            framework.EventManager.Dispatcher.PositionChanged += Dispatcher_PositionChanged;
        }

        protected override void OnClosing(CancelEventArgs args)
        {
            framework.EventManager.Dispatcher.PositionChanged -= Dispatcher_PositionChanged;

            base.OnClosing(args);
        }

        private void Dispatcher_PositionChanged(object sender, PositionEventArgs args)
        {
            if (bAutoRefresh)
            {
                InvokeAction(delegate () { refresh(); });
            }
        }

        private void refresh()
        {
            // 是否可以设计成先添加投资组合
            // 再从根节点回过头来添加持仓，这样上一层的持仓由于一样就不添加了

            int columnIndex = -1;
            int rowIndex = -1;
            if (this.treeGridView1.CurrentCell != null)
            {
                rowIndex = this.treeGridView1.CurrentCell.RowIndex;
                columnIndex = this.treeGridView1.CurrentCell.ColumnIndex;
            }

            // 1. 树结构完全显示
            // 2. 只显示持仓
            // 3. 只显示资金
            ShowType show_type = (ShowType)Enum.Parse(typeof(ShowType), this.comboBox_show_type.SelectedItem.ToString());

            Clear();
            // 显示投资组合
            if (show_type == ShowType.All || show_type == ShowType.PortfolioOnly)
            {
                foreach (var portfolio in framework.PortfolioManager.Portfolios)
                {
                    AddNodes_Portfolio(portfolio);
                }
            }

            if (show_type == ShowType.All || show_type == ShowType.PositionOnly)
            {
                foreach (var portfolio in framework.PortfolioManager.Portfolios.Reverse())
                {
                    AddNodes_Position(portfolio);
                }
            }

            if (rowIndex >= 0 && rowIndex < this.treeGridView1.Rows.Count
                && columnIndex > 0 && columnIndex < this.treeGridView1.Columns.Count)
            {
                // 很奇怪，第一行在刷新时默认选中，所以只能手动清理一下
                this.treeGridView1.Rows[0].Selected = false;
                this.treeGridView1.Rows[rowIndex].Selected = true;
                // this.treeGridView1.Rows[rowIndex].Cells[columnIndex].Selected = true;
            }

            this.comboBox_persistence.SelectedIndex = (int)framework.StrategyManager.Persistence;
        }

        private void Clear()
        {
            this.treeGridView1.Nodes.Clear();
            this.portfolio_nodes.Clear();
            this.nodes_position_portfolio.Clear();
        }

        private TreeGridNodeCollection FindNodesByPortfolio(SmartQuant.Portfolio portfolio)
        {
            // 找到对应的节点然后再，然后再做决定
            TreeGridNodeCollection Nodes = null;
            if (portfolio.Parent == null)
            {
                // 根节点
                Nodes = this.treeGridView1.Nodes;
            }
            else
            {
                // 子节点
                TreeGridNode _node;
                if (portfolio_nodes.TryGetValue(portfolio.Parent, out _node))
                {
                    Nodes = _node.Nodes;
                }
                else
                {
                    Nodes = this.treeGridView1.Nodes;
                }
            }

            return Nodes;
        }

        private TreeGridNode FindNodeByPortfolio(SmartQuant.Portfolio portfolio)
        {
            TreeGridNode _node = null;
            if (portfolio_nodes.TryGetValue(portfolio, out _node))
            {
                return _node;
            }

            return null;
        }

        private void AddPositions(SmartQuant.Portfolio portfolio, TreeGridNodeCollection Nodes, bool hasChildren)
        {
            // 检查是否有持仓
            foreach (var position in portfolio.Positions)
            {
                // 添加持仓
                var entryPrice = 0.0;
                var entyDate = DateTime.Today;
                if (position.Fills.Count > 0)
                {
                    entryPrice = position.Fills.Last().Price;
                    entyDate = position.Fills.Last().DateTime;
                }
                var _node = Nodes.Add(
                    portfolio.Name,
                    position.Instrument.Symbol,
                    position.Amount,
                    position.LongPositionQty,
                    position.ShortPositionQty,
                    null,
                    entryPrice,
                    entyDate
                    );

                _node.Cells[IDX_Symbol].ReadOnly = true;
                _node.Cells[IDX_Long].Style.BackColor = Color.LightYellow;
                _node.Cells[IDX_Short].Style.BackColor = Color.LightYellow;
                _node.Cells[IDX_AccountValue].ReadOnly = true;
                _node.Cells[IDX_EntryPrice].Style.BackColor = Color.LightYellow;
                _node.Cells[IDX_EntryDate].Style.BackColor = Color.LightYellow;

                if (!hasChildren)
                {
                    _node.DefaultCellStyle.BackColor = Color.LightGreen;
                }

                // 记录下，用于回头修改持仓，它没有子节点
                nodes_position_portfolio.Add(_node, position);
            }
        }

        private TreeGridNode AddPortfolio(SmartQuant.Portfolio portfolio, TreeGridNodeCollection Nodes, bool hasChildren)
        {
            // 添加投资组合
            var node = Nodes.Add(
                portfolio.Name,
                null,
                null,
                null,
                null,
                portfolio.AccountValue,
                null,
                DateTime.Today
                );

            node.Cells[IDX_Symbol].ReadOnly = false;
            node.Cells[IDX_Symbol].Style.BackColor = Color.LightYellow;
            node.Cells[IDX_Long].ReadOnly = true;
            node.Cells[IDX_Short].ReadOnly = true;
            node.Cells[IDX_AccountValue].Style.BackColor = Color.LightYellow;
            node.Cells[IDX_EntryDate].Style.BackColor = Color.LightYellow;

            // 记录下，下次添加子节点使用
            portfolio_nodes.Add(portfolio, node);
            // 记录下，用于回头修改持仓，它一般有子节点
            nodes_position_portfolio.Add(node, portfolio);

            if (!hasChildren)
            {
                node.DefaultCellStyle.BackColor = Color.LightPink;
            }

            return node;
        }

        private void AddNodes(SmartQuant.Portfolio portfolio, ShowType show_type)
        {
            // 找到对应的节点然后再，然后再做决定
            TreeGridNodeCollection Nodes = FindNodesByPortfolio(portfolio);

            if (Nodes == null)
                return;
            TreeGridNode node = null;

            bool hasChildren = portfolio.Children.Count > 0;

            switch (show_type)
            {
                case ShowType.All:
                    node = AddPortfolio(portfolio, Nodes, hasChildren);
                    AddPositions(portfolio, node.Nodes, hasChildren);
                    node.Expand();
                    break;
                case ShowType.PortfolioOnly:
                    node = AddPortfolio(portfolio, Nodes, hasChildren);
                    node.Expand();
                    break;
                case ShowType.PositionOnly:
                    // 只显示根节点
                    // 注意，如果出现在父节点上添加持仓将无法处理，所以最好不要使用特别的用法
                    if (hasChildren)
                        return;
                    AddPositions(portfolio, this.treeGridView1.Nodes, hasChildren);
                    break;
            }
        }

        private void AddNodes_Portfolio(SmartQuant.Portfolio portfolio)
        {
            // 找到对应的节点然后再，然后再做决定
            TreeGridNodeCollection Nodes = FindNodesByPortfolio(portfolio);

            if (Nodes == null)
                return;
            TreeGridNode node = null;

            bool hasChildren = portfolio.Children.Count > 0;

            node = AddPortfolio(portfolio, Nodes, hasChildren);
            node.Expand();
        }

        private void AddNodes_Position(SmartQuant.Portfolio portfolio)
        {
            // 找到对应的节点然后再，然后再做决定
            TreeGridNode node = FindNodeByPortfolio(portfolio);

            bool hasChildren = portfolio.Children.Count > 0;

            if (node == null)
            {
                AddPositions(portfolio, this.treeGridView1.Nodes, hasChildren);
            }
            else
            {
                AddPositions(portfolio, node.Nodes, hasChildren);
            }
        }

        private DateTime GetCellDateTime(DataGridViewCell cell)
        {
            DateTime dateTime = DateTime.Today;
            if (cell.Value != null)
            {
                dateTime = Convert.ToDateTime(cell.Value);
            }
            return dateTime;
        }

        private double GetCellDouble(DataGridViewCell cell)
        {
            double db = 0;
            if (cell.Value != null)
            {
                db = Convert.ToDouble(cell.Value);
            }
            return db;
        }

        private void CellEndEdit(SmartQuant.Portfolio pf, SmartQuant.Position pos)
        {
            double price = GetCellDouble(this.treeGridView1.Rows[this.treeGridView1.CurrentCell.RowIndex].Cells[IDX_EntryPrice]);
            DateTime dateTime = GetCellDateTime(this.treeGridView1.Rows[this.treeGridView1.CurrentCell.RowIndex].Cells[IDX_EntryDate]);

            switch (this.treeGridView1.CurrentCell.ColumnIndex)
            {
                case IDX_AccountValue:
                    {
                        double new_data = GetCellDouble(this.treeGridView1.CurrentCell);
                        PortfolioHelper.AddAccountValue(framework, pf, dateTime,
                            pf.Account.CurrencyId, pf.AccountValue, new_data, "TreeGridView");
                    }
                    break;
                case IDX_Short:
                    {
                        if (pos == null)
                        {
                            Console.WriteLine("Error:Position == null");
                            return;
                        }
                        double new_data = GetCellDouble(this.treeGridView1.CurrentCell);
                        if (new_data < 0)
                        {
                            Console.WriteLine("Error:Short must >0");
                            return;
                        }
                        PortfolioHelper.AddShortPosition(framework, pf, dateTime,
                            pos.Instrument, pos.Instrument.CurrencyId,
                            pos.ShortPositionQty, new_data, price, "TreeGridView");
                    }
                    break;
                case IDX_Long:
                    {
                        if (pos == null)
                        {
                            Console.WriteLine("Error:Position == null");
                            return;
                        }
                        double new_data = GetCellDouble(this.treeGridView1.CurrentCell);
                        if (new_data < 0)
                        {
                            Console.WriteLine("Error:Long must >0");
                            return;
                        }
                        PortfolioHelper.AddLongPosition(framework, pf, dateTime,
                            pos.Instrument, pos.Instrument.CurrencyId,
                            pos.LongPositionQty, new_data, price, "TreeGridView");
                    }
                    break;
                case IDX_Symbol:
                    {
                        string new_data = this.treeGridView1.CurrentCell.Value.ToString();
                        if (string.IsNullOrWhiteSpace(new_data))
                        {
                            Console.WriteLine("Error:Symbol is empty");
                            return;
                        }

                        var inst = framework.InstrumentManager.Instruments[new_data];
                        if (inst == null)
                        {
                            Console.WriteLine("Error:Instrument is not exists");
                            return;
                        }

                        pos = pf.GetPosition(inst);
                        if (pos != null)
                        {
                            Console.WriteLine("Error:Position is exists");
                            return;
                        }

                        PortfolioHelper.AddPosition2(framework, pf, dateTime,
                            inst, inst.CurrencyId, OrderSide.Buy, SubSide.Undefined, 0, price, "TreeGridView");
                    }
                    break;
                default:
                    return;
            }
        }

        private void treeGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            object obj;
            if (!nodes_position_portfolio.TryGetValue(this.treeGridView1.CurrentNode, out obj))
                return;

            var pf = obj as SmartQuant.Portfolio;
            var pos = obj as SmartQuant.Position;
            if (pf == null)
            {
                pf = pos.Portfolio;
            }
            if (this.treeGridView1.CurrentCell.Value == null)
            {
                return;
            }

            try
            {
                CellEndEdit(pf, pos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            // 导出时其实可以划分成今昨，分别导出成昨天收盘后的持仓与今天收盘后的持仓即可
            var list = new List<PositionItem>();

            // 先导出投资组合
            foreach (var portfolio in framework.PortfolioManager.Portfolios)
            {
                var parent = portfolio.Parent == null ? "" : portfolio.Parent.Name;
                list.Add(new PositionItem()
                {
                    Parent = parent,
                    PortfolioName = portfolio.Name,
                    Symbol = "",
                    Side = PositionSide.Long,
                    Qty = 0,
                    EntryPrice = 0,
                    EntryDate = DateTime.Today,
                });
            }

            // 再导出持仓
            foreach (var portfolio in framework.PortfolioManager.Portfolios.Reverse())
            {
                var parent = portfolio.Parent == null ? "" : portfolio.Parent.Name;

                foreach (var position in portfolio.Positions)
                {
                    // 注意，这种写法的入场价与入场时间并不对应
                    if (position.Fills.Count == 0)
                        continue;

                    var symbol = position.Instrument.Symbol;
                    var fill = position.Fills.Last();

                    if (position.LongPositionQty > 0)
                    {
                        list.Add(new PositionItem()
                        {
                            Parent = parent,
                            PortfolioName = portfolio.Name,
                            Symbol = symbol,
                            Side = PositionSide.Long,
                            Qty = position.LongPositionQty,
                            EntryPrice = fill.Price,
                            EntryDate = fill.DateTime
                        });
                    }
                    if (position.ShortPositionQty > 0)
                    {
                        list.Add(new PositionItem()
                        {
                            Parent = parent,
                            PortfolioName = portfolio.Name,
                            Symbol = symbol,
                            Side = PositionSide.Short,
                            Qty = position.ShortPositionQty,
                            EntryPrice = fill.Price,
                            EntryDate = fill.DateTime
                        });
                    }
                }
            }

            Console.WriteLine("==================================================");
            Console.WriteLine("Parent,PortfolioName,Symbol,Side,Qty,EntryPrice,EntryDate");
            foreach (var it in list)
            {
                Console.WriteLine(it);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV文件(*.csv)|*.csv|所有文件|*.*";//设置文件类型
            sfd.FileName = "导出持仓";//设置默认文件名
            sfd.AddExtension = true;//设置自动在文件名中添加扩展名
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                PortfolioHelper.to_csv(sfd.FileName, list);
            }
        }

        private void button_import_Click(object sender, EventArgs e)
        {
            // 导入数据时，可以通过导入的方式进行改仓，或记录今昨
            // 比如第一天收盘后多5手，第二天收盘后多2手，按此记录即可
            // 它会在第二天的指定时间平仓3手

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*"; //设置“另存为文件类型”或“文件类型”框中出现的选择内容
            ofd.Title = "打开文件"; //获取或设置文件对话框标题
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var list = PortfolioHelper.from_csv<PositionItem>(ofd.FileName);
            foreach (var it in list)
            {
                var inst = framework.InstrumentManager.GetBySymbol(it.Symbol);
                var pf = framework.PortfolioManager.GetByName(it.PortfolioName);
                if (pf == null)
                {
                    pf = new SmartQuant.Portfolio(it.PortfolioName);
                    framework.PortfolioManager.Add(pf);
                    pf.Parent = framework.PortfolioManager.GetByName(it.Parent);
                }
                if (inst == null)
                    continue;
                var position = pf.GetPosition(inst);
                var old_qty = 0.0;

                if (it.Side == PositionSide.Long)
                {
                    if (position != null)
                    {
                        old_qty = position.LongPositionQty;
                    }
                    PortfolioHelper.AddLongPosition(framework, pf, it.EntryDate, inst, inst.CurrencyId, old_qty, it.Qty, it.EntryPrice, "Import from csv");
                }
                else
                {
                    if (position != null)
                    {
                        old_qty = position.ShortPositionQty;
                    }
                    PortfolioHelper.AddShortPosition(framework, pf, it.EntryDate, inst, inst.CurrencyId, old_qty, it.Qty, it.EntryPrice, "Import from csv");
                }
            }

            // refresh();
        }

        private void comboBox_show_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh();
        }

        private void checkBox_auto_refresh_CheckedChanged(object sender, EventArgs e)
        {
            bAutoRefresh = this.checkBox_auto_refresh.Checked;
        }

        private void comboBox_persistence_SelectedIndexChanged(object sender, EventArgs e)
        {
            //StrategyPersistence sp = (StrategyPersistence)Enum.Parse(typeof(StrategyPersistence), this.comboBox_persistence.SelectedItem.ToString());

            //framework.StrategyManager.Persistence = sp;
            //framework.StrategyManager_.Persistence = sp;
        }

        private void button_export_all_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var helper = new QuantFileHelper(new Framework());

            var strategy_name = framework.StrategyManager.Strategy.Name;
            var portfolio = framework.PortfolioManager.GetByName(strategy_name);

            helper.AppendMessagesToFile(strategy_name, Path.Combine(fbd.SelectedPath, "orders.quant"), framework.OrderManager.Messages, false);
            helper.SavePortfolioToFile(strategy_name, Path.Combine(fbd.SelectedPath, "portfolios.quant"), portfolio);
            helper.SaveInstrumentsToFile(Path.Combine(fbd.SelectedPath, "instruments.quant"), framework.InstrumentManager.Instruments);

            helper.Close();
        }
    }
}
