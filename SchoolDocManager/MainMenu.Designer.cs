namespace SchoolDocManager
{
    partial class MainMenu
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
            this.ListFiles = new System.Windows.Forms.Button();
            this.SearchFiles = new System.Windows.Forms.Button();
            this.Welcum = new System.Windows.Forms.Label();
            this.UpdateData = new System.Windows.Forms.Button();
            this.FolderPath = new System.Windows.Forms.TextBox();
            this.selectFolderButton = new System.Windows.Forms.PictureBox();
            this.btnHelp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.selectFolderButton)).BeginInit();
            this.SuspendLayout();
            // 
            // ListFiles
            // 
            this.ListFiles.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.ListFiles.FlatAppearance.BorderSize = 3;
            this.ListFiles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.ListFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ListFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ListFiles.Location = new System.Drawing.Point(106, 154);
            this.ListFiles.Name = "ListFiles";
            this.ListFiles.Size = new System.Drawing.Size(299, 81);
            this.ListFiles.TabIndex = 0;
            this.ListFiles.Text = "Список файлов";
            this.ListFiles.UseVisualStyleBackColor = true;
            this.ListFiles.Click += new System.EventHandler(this.ListFiles_Click);
            // 
            // SearchFiles
            // 
            this.SearchFiles.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.SearchFiles.FlatAppearance.BorderSize = 3;
            this.SearchFiles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.SearchFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SearchFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchFiles.Location = new System.Drawing.Point(472, 154);
            this.SearchFiles.Name = "SearchFiles";
            this.SearchFiles.Size = new System.Drawing.Size(299, 81);
            this.SearchFiles.TabIndex = 1;
            this.SearchFiles.Text = "Поиск по документах";
            this.SearchFiles.UseVisualStyleBackColor = true;
            this.SearchFiles.Click += new System.EventHandler(this.SearchFiles_Click);
            // 
            // Welcum
            // 
            this.Welcum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Welcum.AutoSize = true;
            this.Welcum.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Welcum.Location = new System.Drawing.Point(290, 44);
            this.Welcum.Name = "Welcum";
            this.Welcum.Size = new System.Drawing.Size(294, 66);
            this.Welcum.TabIndex = 3;
            this.Welcum.Text = "Добро пожаловать в\r\nDocFinder";
            this.Welcum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateData
            // 
            this.UpdateData.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.UpdateData.FlatAppearance.BorderSize = 3;
            this.UpdateData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.UpdateData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UpdateData.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.UpdateData.Location = new System.Drawing.Point(296, 267);
            this.UpdateData.Name = "UpdateData";
            this.UpdateData.Size = new System.Drawing.Size(266, 65);
            this.UpdateData.TabIndex = 4;
            this.UpdateData.Text = "Обновить данные";
            this.UpdateData.UseVisualStyleBackColor = true;
            this.UpdateData.Click += new System.EventHandler(this.UpdateData_Click);
            // 
            // FolderPath
            // 
            this.FolderPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FolderPath.Location = new System.Drawing.Point(173, 354);
            this.FolderPath.Name = "FolderPath";
            this.FolderPath.Size = new System.Drawing.Size(442, 31);
            this.FolderPath.TabIndex = 5;
            this.FolderPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FolderPath.TextChanged += new System.EventHandler(this.FolderPath_TextChanged);
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Image = global::SchoolDocManager.Properties.Resources.folder;
            this.selectFolderButton.Location = new System.Drawing.Point(633, 342);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(56, 50);
            this.selectFolderButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.selectFolderButton.TabIndex = 8;
            this.selectFolderButton.TabStop = false;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnHelp.FlatAppearance.BorderSize = 3;
            this.btnHelp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnHelp.Location = new System.Drawing.Point(326, 417);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(201, 63);
            this.btnHelp.TabIndex = 9;
            this.btnHelp.Text = "Справка";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(241)))), ((int)(((byte)(238)))));
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.selectFolderButton);
            this.Controls.Add(this.FolderPath);
            this.Controls.Add(this.UpdateData);
            this.Controls.Add(this.Welcum);
            this.Controls.Add(this.SearchFiles);
            this.Controls.Add(this.ListFiles);
            this.Name = "MainMenu";
            this.Size = new System.Drawing.Size(850, 500);
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.Resize += new System.EventHandler(this.MainMenu_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.selectFolderButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ListFiles;
        private System.Windows.Forms.Button SearchFiles;
        private System.Windows.Forms.Label Welcum;
        private System.Windows.Forms.Button UpdateData;
        private System.Windows.Forms.TextBox FolderPath;
        private System.Windows.Forms.PictureBox selectFolderButton;
        private System.Windows.Forms.Button btnHelp;
    }
}
