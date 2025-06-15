namespace SchoolDocManager
{
    partial class ListFilesPanel
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
            this.components = new System.ComponentModel.Container();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.RenameButton = new System.Windows.Forms.Button();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.BackButton = new System.Windows.Forms.Button();
            this.currentFolderLabel = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ChangeDisplay = new System.Windows.Forms.PictureBox();
            this.CancelMenu = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChangeDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CancelMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // DeleteButton
            // 
            this.DeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DeleteButton.Location = new System.Drawing.Point(426, 451);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(421, 46);
            this.DeleteButton.TabIndex = 22;
            this.DeleteButton.Text = "Удалить";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // RenameButton
            // 
            this.RenameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RenameButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.RenameButton.Location = new System.Drawing.Point(3, 451);
            this.RenameButton.Name = "RenameButton";
            this.RenameButton.Size = new System.Drawing.Size(417, 46);
            this.RenameButton.TabIndex = 21;
            this.RenameButton.Text = "Переименовать";
            this.RenameButton.UseVisualStyleBackColor = true;
            this.RenameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.searchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.searchTextBox.ForeColor = System.Drawing.Color.Gray;
            this.searchTextBox.Location = new System.Drawing.Point(80, 9);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(710, 38);
            this.searchTextBox.TabIndex = 23;
            this.searchTextBox.Text = "Поиск документов или папки";
            this.searchTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            this.searchTextBox.Enter += new System.EventHandler(this.searchTextBox_Enter);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(13, 93);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(822, 352);
            this.flowLayoutPanel1.TabIndex = 32;
            // 
            // BackButton
            // 
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackButton.Location = new System.Drawing.Point(13, 53);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(90, 34);
            this.BackButton.TabIndex = 0;
            this.BackButton.Text = "Назад";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // currentFolderLabel
            // 
            this.currentFolderLabel.AutoSize = true;
            this.currentFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.currentFolderLabel.Location = new System.Drawing.Point(109, 60);
            this.currentFolderLabel.Name = "currentFolderLabel";
            this.currentFolderLabel.Size = new System.Drawing.Size(193, 24);
            this.currentFolderLabel.TabIndex = 33;
            this.currentFolderLabel.Text = "Родительская папка";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ChangeDisplay
            // 
            this.ChangeDisplay.Image = global::SchoolDocManager.Properties.Resources.IconTable;
            this.ChangeDisplay.Location = new System.Drawing.Point(796, 3);
            this.ChangeDisplay.Name = "ChangeDisplay";
            this.ChangeDisplay.Size = new System.Drawing.Size(51, 51);
            this.ChangeDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ChangeDisplay.TabIndex = 41;
            this.ChangeDisplay.TabStop = false;
            this.ChangeDisplay.Click += new System.EventHandler(this.ChangeDisplay_Click);
            // 
            // CancelMenu
            // 
            this.CancelMenu.BackColor = System.Drawing.Color.Transparent;
            this.CancelMenu.Image = global::SchoolDocManager.Properties.Resources.bt_back;
            this.CancelMenu.Location = new System.Drawing.Point(3, 3);
            this.CancelMenu.Name = "CancelMenu";
            this.CancelMenu.Size = new System.Drawing.Size(71, 44);
            this.CancelMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.CancelMenu.TabIndex = 42;
            this.CancelMenu.TabStop = false;
            this.CancelMenu.Click += new System.EventHandler(this.CancelMenu_Click);
            // 
            // ListFilesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CancelMenu);
            this.Controls.Add(this.ChangeDisplay);
            this.Controls.Add(this.currentFolderLabel);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.searchTextBox);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.RenameButton);
            this.Name = "ListFilesPanel";
            this.Size = new System.Drawing.Size(850, 500);
            this.Load += new System.EventHandler(this.ListFilesPanel_Load);
            this.Resize += new System.EventHandler(this.ListFilesPanel_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.ChangeDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CancelMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button RenameButton;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Label currentFolderLabel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox ChangeDisplay;
        private System.Windows.Forms.PictureBox CancelMenu;
    }
}
