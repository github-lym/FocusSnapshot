namespace FocusSnapshot
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Btn_Snapshot = new System.Windows.Forms.Button();
            this.Btn_Browse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Btn_Snapshot
            // 
            this.Btn_Snapshot.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_Snapshot.BackgroundImage")));
            this.Btn_Snapshot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Btn_Snapshot.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Snapshot.Location = new System.Drawing.Point(107, 5);
            this.Btn_Snapshot.Name = "Btn_Snapshot";
            this.Btn_Snapshot.Size = new System.Drawing.Size(98, 82);
            this.Btn_Snapshot.TabIndex = 0;
            this.Btn_Snapshot.UseVisualStyleBackColor = true;
            this.Btn_Snapshot.Click += new System.EventHandler(this.Btn_Snapshot_Click);
            // 
            // Btn_Browse
            // 
            this.Btn_Browse.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_Browse.BackgroundImage")));
            this.Btn_Browse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Btn_Browse.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Browse.Location = new System.Drawing.Point(3, 5);
            this.Btn_Browse.Name = "Btn_Browse";
            this.Btn_Browse.Size = new System.Drawing.Size(98, 82);
            this.Btn_Browse.TabIndex = 3;
            this.Btn_Browse.UseVisualStyleBackColor = true;
            this.Btn_Browse.Click += new System.EventHandler(this.Btn_Browse_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 91);
            this.Controls.Add(this.Btn_Browse);
            this.Controls.Add(this.Btn_Snapshot);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "FocusWindows&Snapshot";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_Snapshot;
        private System.Windows.Forms.Button Btn_Browse;
    }
}

