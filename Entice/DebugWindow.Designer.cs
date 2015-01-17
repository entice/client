namespace Entice
{
        internal partial class DebugWindow
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
                        this.console = new System.Windows.Forms.ListView();
                        this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
                        this.SuspendLayout();
                        // 
                        // console
                        // 
                        this.console.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
                        this.console.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
                        this.console.FullRowSelect = true;
                        this.console.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
                        this.console.Location = new System.Drawing.Point(12, 12);
                        this.console.Name = "console";
                        this.console.Size = new System.Drawing.Size(791, 458);
                        this.console.TabIndex = 0;
                        this.console.UseCompatibleStateImageBehavior = false;
                        this.console.View = System.Windows.Forms.View.Details;
                        // 
                        // columnHeader1
                        // 
                        this.columnHeader1.Width = 750;
                        // 
                        // DebugWindow
                        // 
                        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                        this.ClientSize = new System.Drawing.Size(815, 482);
                        this.Controls.Add(this.console);
                        this.Name = "DebugWindow";
                        this.Text = "entice ~ debug";
                        this.ResumeLayout(false);

                }

                #endregion

                private System.Windows.Forms.ListView console;
                private System.Windows.Forms.ColumnHeader columnHeader1;
        }
}