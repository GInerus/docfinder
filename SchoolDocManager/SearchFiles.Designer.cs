namespace SchoolDocManager
{
    partial class SearchFiles
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.ResultsListBox = new System.Windows.Forms.ListBox();
            this.CancelMenu = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CancelMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.SearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchTextBox.ForeColor = System.Drawing.Color.Black;
            this.SearchTextBox.Location = new System.Drawing.Point(80, 7);
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(645, 38);
            this.SearchTextBox.TabIndex = 4;
            this.SearchTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SearchButton
            // 
            this.SearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchButton.Location = new System.Drawing.Point(731, 3);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(116, 44);
            this.SearchButton.TabIndex = 33;
            this.SearchButton.Text = "Поиск";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // ResultsListBox
            // 
            this.ResultsListBox.BackColor = System.Drawing.SystemColors.Control;
            this.ResultsListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultsListBox.FormattingEnabled = true;
            this.ResultsListBox.ItemHeight = 16;
            this.ResultsListBox.Location = new System.Drawing.Point(3, 55);
            this.ResultsListBox.Name = "ResultsListBox";
            this.ResultsListBox.Size = new System.Drawing.Size(844, 436);
            this.ResultsListBox.TabIndex = 34;
            // 
            // CancelMenu
            // 
            this.CancelMenu.BackColor = System.Drawing.Color.Transparent;
            this.CancelMenu.Image = global::SchoolDocManager.Properties.Resources.bt_back;
            this.CancelMenu.Location = new System.Drawing.Point(3, 3);
            this.CancelMenu.Name = "CancelMenu";
            this.CancelMenu.Size = new System.Drawing.Size(71, 44);
            this.CancelMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.CancelMenu.TabIndex = 35;
            this.CancelMenu.TabStop = false;
            this.CancelMenu.Click += new System.EventHandler(this.CancelMenu_Click);
            // 
            // SearchFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CancelMenu);
            this.Controls.Add(this.ResultsListBox);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.SearchTextBox);
            this.Name = "SearchFiles";
            this.Size = new System.Drawing.Size(850, 500);
            this.Load += new System.EventHandler(this.ListFilesPanel_Load);
            this.Resize += new System.EventHandler(this.ListFilesPanel_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.CancelMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.ListBox ResultsListBox;
        private System.Windows.Forms.PictureBox CancelMenu;
    }
}
