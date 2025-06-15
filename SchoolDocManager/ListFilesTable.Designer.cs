namespace SchoolDocManager
{
    partial class ListFilesTable
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
            this.CancelMenu = new System.Windows.Forms.Button();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.currentFolderLabel = new System.Windows.Forms.Label();
            this.BackButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.RenameButton = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.Имя = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Тип = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Размер = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ДатаСоздания = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.changeDisplayButton = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.changeDisplayButton)).BeginInit();
            this.SuspendLayout();
            // 
            // CancelMenu
            // 
            this.CancelMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CancelMenu.Location = new System.Drawing.Point(3, 3);
            this.CancelMenu.Name = "CancelMenu";
            this.CancelMenu.Size = new System.Drawing.Size(71, 44);
            this.CancelMenu.TabIndex = 34;
            this.CancelMenu.Text = "<--";
            this.CancelMenu.UseVisualStyleBackColor = true;
            this.CancelMenu.Click += new System.EventHandler(this.CancelMenu_Click);
            // 
            // searchTextBox
            // 
            this.searchTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.searchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.searchTextBox.ForeColor = System.Drawing.Color.Gray;
            this.searchTextBox.Location = new System.Drawing.Point(80, 9);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(710, 38);
            this.searchTextBox.TabIndex = 32;
            this.searchTextBox.Text = "Поиск документов или папки";
            this.searchTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // currentFolderLabel
            // 
            this.currentFolderLabel.AutoSize = true;
            this.currentFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.currentFolderLabel.Location = new System.Drawing.Point(109, 60);
            this.currentFolderLabel.Name = "currentFolderLabel";
            this.currentFolderLabel.Size = new System.Drawing.Size(193, 24);
            this.currentFolderLabel.TabIndex = 36;
            this.currentFolderLabel.Text = "Родительская папка";
            // 
            // BackButton
            // 
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BackButton.Location = new System.Drawing.Point(13, 53);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(90, 34);
            this.BackButton.TabIndex = 35;
            this.BackButton.Text = "Назад";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DeleteButton.Location = new System.Drawing.Point(426, 451);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(421, 46);
            this.DeleteButton.TabIndex = 38;
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
            this.RenameButton.TabIndex = 37;
            this.RenameButton.Text = "Переименовать";
            this.RenameButton.UseVisualStyleBackColor = true;
            this.RenameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // listView
            // 
            this.listView.BackColor = System.Drawing.SystemColors.Control;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Имя,
            this.Тип,
            this.Размер,
            this.ДатаСоздания});
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(13, 93);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(822, 352);
            this.listView.TabIndex = 39;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
            // 
            // Имя
            // 
            this.Имя.Text = "Имя";
            this.Имя.Width = 410;
            // 
            // Тип
            // 
            this.Тип.Text = "Тип";
            this.Тип.Width = 140;
            // 
            // Размер
            // 
            this.Размер.Text = "Размер";
            this.Размер.Width = 110;
            // 
            // ДатаСоздания
            // 
            this.ДатаСоздания.Text = "Дата создания";
            this.ДатаСоздания.Width = 150;
            // 
            // changeDisplayButton
            // 
            this.changeDisplayButton.Image = global::SchoolDocManager.Properties.Resources.IconPanel;
            this.changeDisplayButton.Location = new System.Drawing.Point(796, 7);
            this.changeDisplayButton.Name = "changeDisplayButton";
            this.changeDisplayButton.Size = new System.Drawing.Size(51, 44);
            this.changeDisplayButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.changeDisplayButton.TabIndex = 40;
            this.changeDisplayButton.TabStop = false;
            this.changeDisplayButton.Click += new System.EventHandler(this.ChangeDisplayButton_Click);
            // 
            // ListFilesTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.changeDisplayButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.RenameButton);
            this.Controls.Add(this.currentFolderLabel);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.CancelMenu);
            this.Controls.Add(this.searchTextBox);
            this.Name = "ListFilesTable";
            this.Size = new System.Drawing.Size(850, 500);
            this.Load += new System.EventHandler(this.ListFilesTable_Load);
            this.Resize += new System.EventHandler(this.ListFilesPanel_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.changeDisplayButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button CancelMenu;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Label currentFolderLabel;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button RenameButton;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader Имя;
        private System.Windows.Forms.ColumnHeader Тип;
        private System.Windows.Forms.ColumnHeader Размер;
        private System.Windows.Forms.ColumnHeader ДатаСоздания;
        private System.Windows.Forms.PictureBox changeDisplayButton;
    }
}
