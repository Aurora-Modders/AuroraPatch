
namespace AuroraPatch
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonStart = new System.Windows.Forms.Button();
            this.ListPatches = new System.Windows.Forms.ListBox();
            this.LabelPatches = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonStart
            // 
            this.ButtonStart.Location = new System.Drawing.Point(32, 310);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(203, 62);
            this.ButtonStart.TabIndex = 0;
            this.ButtonStart.Text = "Start Aurora";
            this.ButtonStart.UseVisualStyleBackColor = true;
            this.ButtonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            // 
            // ListPatches
            // 
            this.ListPatches.FormattingEnabled = true;
            this.ListPatches.Location = new System.Drawing.Point(29, 47);
            this.ListPatches.Name = "ListPatches";
            this.ListPatches.Size = new System.Drawing.Size(205, 238);
            this.ListPatches.TabIndex = 1;
            // 
            // LabelPatches
            // 
            this.LabelPatches.AutoSize = true;
            this.LabelPatches.Location = new System.Drawing.Point(29, 31);
            this.LabelPatches.Name = "LabelPatches";
            this.LabelPatches.Size = new System.Drawing.Size(81, 13);
            this.LabelPatches.TabIndex = 2;
            this.LabelPatches.Text = "Found patches:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.LabelPatches);
            this.Controls.Add(this.ListPatches);
            this.Controls.Add(this.ButtonStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.ListBox ListPatches;
        private System.Windows.Forms.Label LabelPatches;
    }
}