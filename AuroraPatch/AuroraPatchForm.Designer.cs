
namespace AuroraPatch
{
    partial class AuroraPatchForm
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
            this.ButtonStart          = new System.Windows.Forms.Button();
            this.LabelPatches         = new System.Windows.Forms.Label();
            this.LabelDescription     = new System.Windows.Forms.Label();
            this.ButtonChangeSettings = new System.Windows.Forms.Button();
            this.CheckedListPatches   = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // ButtonStart
            // 
            this.ButtonStart.Location                =  new System.Drawing.Point(32, 310);
            this.ButtonStart.Name                    =  "ButtonStart";
            this.ButtonStart.Size                    =  new System.Drawing.Size(203, 62);
            this.ButtonStart.TabIndex                =  0;
            this.ButtonStart.Text                    =  "Start Aurora";
            this.ButtonStart.UseVisualStyleBackColor =  true;
            this.ButtonStart.Click                   += new System.EventHandler(this.ButtonStart_Click);
            // 
            // LabelPatches
            // 
            this.LabelPatches.AutoSize = true;
            this.LabelPatches.Location = new System.Drawing.Point(29, 31);
            this.LabelPatches.Name     = "LabelPatches";
            this.LabelPatches.Size     = new System.Drawing.Size(81, 13);
            this.LabelPatches.TabIndex = 2;
            this.LabelPatches.Text     = "Found patches:";
            // 
            // LabelDescription
            // 
            this.LabelDescription.AutoSize = true;
            this.LabelDescription.Location = new System.Drawing.Point(247, 53);
            this.LabelDescription.Name     = "LabelDescription";
            this.LabelDescription.Size     = new System.Drawing.Size(63, 13);
            this.LabelDescription.TabIndex = 3;
            this.LabelDescription.Text     = "Description:";
            // 
            // ButtonChangeSettings
            // 
            this.ButtonChangeSettings.Location                =  new System.Drawing.Point(251, 91);
            this.ButtonChangeSettings.Name                    =  "ButtonChangeSettings";
            this.ButtonChangeSettings.Size                    =  new System.Drawing.Size(141, 41);
            this.ButtonChangeSettings.TabIndex                =  4;
            this.ButtonChangeSettings.Text                    =  "Change settings";
            this.ButtonChangeSettings.UseVisualStyleBackColor =  true;
            this.ButtonChangeSettings.Click                   += new System.EventHandler(this.ButtonChangeSettings_Click);
            // 
            // CheckedListPatches
            // 
            this.CheckedListPatches.CheckOnClick         =  true;
            this.CheckedListPatches.FormattingEnabled    =  true;
            this.CheckedListPatches.Location             =  new System.Drawing.Point(32, 47);
            this.CheckedListPatches.MultiColumn          =  true;
            this.CheckedListPatches.Name                 =  "CheckedListPatches";
            this.CheckedListPatches.Size                 =  new System.Drawing.Size(205, 229);
            this.CheckedListPatches.Sorted               =  true;
            this.CheckedListPatches.TabIndex             =  5;
            this.CheckedListPatches.ItemCheck            += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListPatches_ItemCheck);
            this.CheckedListPatches.SelectedValueChanged += new System.EventHandler(this.CheckedListPatches_SelectedValueChanged);
            this.CheckedListPatches.MouseDown            += new System.Windows.Forms.MouseEventHandler(this.CheckedListPatches_MouseClick);
            this.CheckedListPatches.MouseMove            += new System.Windows.Forms.MouseEventHandler(this.CheckedListPatches_MouseUp);
            // 
            // AuroraPatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CheckedListPatches);
            this.Controls.Add(this.ButtonChangeSettings);
            this.Controls.Add(this.LabelDescription);
            this.Controls.Add(this.LabelPatches);
            this.Controls.Add(this.ButtonStart);
            this.Name       =  "AuroraPatchForm";
            this.Text       =  "AuroraPatch";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AuroraPatchForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.CheckedListBox CheckedListPatches;

        #endregion

        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.Label  LabelPatches;
        private System.Windows.Forms.Label  LabelDescription;
        private System.Windows.Forms.Button ButtonChangeSettings;
    }
}