namespace EgyptMapHack {
    partial class Form1 {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.AcceleratedExploration = new System.Windows.Forms.CheckBox();
            this.TotalTurnsToExploreLabel = new System.Windows.Forms.Label();
            this.WorkersScoutingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AcceleratedExploration
            // 
            this.AcceleratedExploration.AutoSize = true;
            this.AcceleratedExploration.Location = new System.Drawing.Point(13, 13);
            this.AcceleratedExploration.Name = "AcceleratedExploration";
            this.AcceleratedExploration.Size = new System.Drawing.Size(169, 20);
            this.AcceleratedExploration.TabIndex = 0;
            this.AcceleratedExploration.Text = "Accelerated exploration";
            this.AcceleratedExploration.UseVisualStyleBackColor = true;
            this.AcceleratedExploration.CheckedChanged += new System.EventHandler(this.AcceleratedExploration_CheckedChanged);
            // 
            // TotalTurnsToExploreLabel
            // 
            this.TotalTurnsToExploreLabel.AutoSize = true;
            this.TotalTurnsToExploreLabel.Location = new System.Drawing.Point(12, 36);
            this.TotalTurnsToExploreLabel.Name = "TotalTurnsToExploreLabel";
            this.TotalTurnsToExploreLabel.Size = new System.Drawing.Size(135, 16);
            this.TotalTurnsToExploreLabel.TabIndex = 1;
            this.TotalTurnsToExploreLabel.Text = "Total turns to explore:";
            // 
            // WorkersScoutingLabel
            // 
            this.WorkersScoutingLabel.AutoSize = true;
            this.WorkersScoutingLabel.Location = new System.Drawing.Point(10, 55);
            this.WorkersScoutingLabel.Name = "WorkersScoutingLabel";
            this.WorkersScoutingLabel.Size = new System.Drawing.Size(115, 16);
            this.WorkersScoutingLabel.TabIndex = 2;
            this.WorkersScoutingLabel.Text = "Workers scouting:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 322);
            this.Controls.Add(this.WorkersScoutingLabel);
            this.Controls.Add(this.TotalTurnsToExploreLabel);
            this.Controls.Add(this.AcceleratedExploration);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Egypt map hack";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox AcceleratedExploration;
        private System.Windows.Forms.Label TotalTurnsToExploreLabel;
        private System.Windows.Forms.Label WorkersScoutingLabel;
    }
}

