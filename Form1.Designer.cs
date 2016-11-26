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
            this.TurnLabel = new System.Windows.Forms.Label();
            this.Event1Label = new System.Windows.Forms.Label();
            this.Event2Label = new System.Windows.Forms.Label();
            this.Event3Label = new System.Windows.Forms.Label();
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
            this.TotalTurnsToExploreLabel.Location = new System.Drawing.Point(10, 36);
            this.TotalTurnsToExploreLabel.Name = "TotalTurnsToExploreLabel";
            this.TotalTurnsToExploreLabel.Size = new System.Drawing.Size(145, 16);
            this.TotalTurnsToExploreLabel.TabIndex = 1;
            this.TotalTurnsToExploreLabel.Text = "Total turns to explore: 0";
            // 
            // WorkersScoutingLabel
            // 
            this.WorkersScoutingLabel.AutoSize = true;
            this.WorkersScoutingLabel.Location = new System.Drawing.Point(10, 55);
            this.WorkersScoutingLabel.Name = "WorkersScoutingLabel";
            this.WorkersScoutingLabel.Size = new System.Drawing.Size(125, 16);
            this.WorkersScoutingLabel.TabIndex = 2;
            this.WorkersScoutingLabel.Text = "Workers scouting: 0";
            // 
            // TurnLabel
            // 
            this.TurnLabel.AutoSize = true;
            this.TurnLabel.Location = new System.Drawing.Point(10, 80);
            this.TurnLabel.Name = "TurnLabel";
            this.TurnLabel.Size = new System.Drawing.Size(48, 16);
            this.TurnLabel.TabIndex = 3;
            this.TurnLabel.Text = "Turn: 0";
            // 
            // Event1Label
            // 
            this.Event1Label.AutoSize = true;
            this.Event1Label.Location = new System.Drawing.Point(10, 96);
            this.Event1Label.Name = "Event1Label";
            this.Event1Label.Size = new System.Drawing.Size(59, 16);
            this.Event1Label.TabIndex = 4;
            this.Event1Label.Text = "0 - Event";
            // 
            // Event2Label
            // 
            this.Event2Label.AutoSize = true;
            this.Event2Label.Location = new System.Drawing.Point(10, 112);
            this.Event2Label.Name = "Event2Label";
            this.Event2Label.Size = new System.Drawing.Size(59, 16);
            this.Event2Label.TabIndex = 5;
            this.Event2Label.Text = "1 - Event";
            // 
            // Event3Label
            // 
            this.Event3Label.AutoSize = true;
            this.Event3Label.Location = new System.Drawing.Point(10, 128);
            this.Event3Label.Name = "Event3Label";
            this.Event3Label.Size = new System.Drawing.Size(59, 16);
            this.Event3Label.TabIndex = 6;
            this.Event3Label.Text = "2 - Event";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 322);
            this.Controls.Add(this.Event3Label);
            this.Controls.Add(this.Event2Label);
            this.Controls.Add(this.Event1Label);
            this.Controls.Add(this.TurnLabel);
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
        private System.Windows.Forms.Label TurnLabel;
        private System.Windows.Forms.Label Event1Label;
        private System.Windows.Forms.Label Event2Label;
        private System.Windows.Forms.Label Event3Label;
    }
}

