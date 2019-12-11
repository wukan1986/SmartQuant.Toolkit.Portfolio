namespace SmartQuant.Toolkit.Portfolio.Controls
{
    partial class PortfolioControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeGridView1 = new AdvancedDataGridView.TreeGridView();
            this.PortfolioName = new AdvancedDataGridView.TreeGridColumn();
            this.Symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Long = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Short = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccountValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EntryPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EntryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_refresh = new System.Windows.Forms.Button();
            this.comboBox_show_type = new System.Windows.Forms.ComboBox();
            this.button_export = new System.Windows.Forms.Button();
            this.button_import = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // treeGridView1
            // 
            this.treeGridView1.AllowUserToAddRows = false;
            this.treeGridView1.AllowUserToDeleteRows = false;
            this.treeGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PortfolioName,
            this.Symbol,
            this.Amount,
            this.Long,
            this.Short,
            this.AccountValue,
            this.EntryPrice,
            this.EntryDate});
            this.treeGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.treeGridView1.ImageList = null;
            this.treeGridView1.Location = new System.Drawing.Point(3, 54);
            this.treeGridView1.Name = "treeGridView1";
            this.treeGridView1.Size = new System.Drawing.Size(979, 285);
            this.treeGridView1.TabIndex = 0;
            this.treeGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.treeGridView1_CellEndEdit);
            // 
            // PortfolioName
            // 
            this.PortfolioName.DefaultNodeImage = null;
            this.PortfolioName.HeaderText = "PortfolioName";
            this.PortfolioName.Name = "PortfolioName";
            this.PortfolioName.ReadOnly = true;
            this.PortfolioName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PortfolioName.Width = 200;
            // 
            // Symbol
            // 
            this.Symbol.HeaderText = "Symbol";
            this.Symbol.Name = "Symbol";
            this.Symbol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Amount
            // 
            this.Amount.HeaderText = "Amount";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            this.Amount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Long
            // 
            this.Long.HeaderText = "Long";
            this.Long.Name = "Long";
            this.Long.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Short
            // 
            this.Short.HeaderText = "Short";
            this.Short.Name = "Short";
            this.Short.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // AccountValue
            // 
            this.AccountValue.HeaderText = "AccountValue";
            this.AccountValue.Name = "AccountValue";
            this.AccountValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // EntryPrice
            // 
            this.EntryPrice.HeaderText = "EntryPrice";
            this.EntryPrice.Name = "EntryPrice";
            this.EntryPrice.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // EntryDate
            // 
            this.EntryDate.HeaderText = "EntryDate";
            this.EntryDate.Name = "EntryDate";
            this.EntryDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.EntryDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.EntryDate.Width = 140;
            // 
            // button_refresh
            // 
            this.button_refresh.Location = new System.Drawing.Point(148, 14);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(75, 23);
            this.button_refresh.TabIndex = 1;
            this.button_refresh.Text = "Refresh";
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // comboBox_show_type
            // 
            this.comboBox_show_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_show_type.FormattingEnabled = true;
            this.comboBox_show_type.Location = new System.Drawing.Point(11, 14);
            this.comboBox_show_type.Name = "comboBox_show_type";
            this.comboBox_show_type.Size = new System.Drawing.Size(121, 21);
            this.comboBox_show_type.TabIndex = 2;
            // 
            // button_export
            // 
            this.button_export.Location = new System.Drawing.Point(267, 14);
            this.button_export.Name = "button_export";
            this.button_export.Size = new System.Drawing.Size(75, 23);
            this.button_export.TabIndex = 3;
            this.button_export.Text = "Export";
            this.button_export.UseVisualStyleBackColor = true;
            this.button_export.Click += new System.EventHandler(this.button_export_Click);
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(353, 14);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(75, 23);
            this.button_import.TabIndex = 4;
            this.button_import.Text = "Import";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // PortfolioControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_export);
            this.Controls.Add(this.comboBox_show_type);
            this.Controls.Add(this.button_refresh);
            this.Controls.Add(this.treeGridView1);
            this.Name = "PortfolioControl";
            this.Size = new System.Drawing.Size(985, 342);
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AdvancedDataGridView.TreeGridView treeGridView1;
        private System.Windows.Forms.Button button_refresh;
        private System.Windows.Forms.ComboBox comboBox_show_type;
        private System.Windows.Forms.Button button_export;
        private AdvancedDataGridView.TreeGridColumn PortfolioName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Symbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Long;
        private System.Windows.Forms.DataGridViewTextBoxColumn Short;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccountValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryDate;
        private System.Windows.Forms.Button button_import;
    }
}
