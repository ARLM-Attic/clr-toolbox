﻿namespace MarcelJoachimKloubert.CryptCommander.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.StatusStrip_Main = new System.Windows.Forms.StatusStrip();
            this.MenuStrip_Main = new System.Windows.Forms.MenuStrip();
            this.SplitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.FileBrowser_Left = new MarcelJoachimKloubert.CryptCommander.Controls.FileBrowser();
            this.FileBrowser_Right = new MarcelJoachimKloubert.CryptCommander.Controls.FileBrowser();
            this.ToolStripMenuItem_File = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer_Main)).BeginInit();
            this.SplitContainer_Main.Panel1.SuspendLayout();
            this.SplitContainer_Main.Panel2.SuspendLayout();
            this.SplitContainer_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip_Main
            // 
            this.StatusStrip_Main.Location = new System.Drawing.Point(0, 539);
            this.StatusStrip_Main.Name = "StatusStrip_Main";
            this.StatusStrip_Main.Size = new System.Drawing.Size(984, 22);
            this.StatusStrip_Main.TabIndex = 0;
            this.StatusStrip_Main.Text = "statusStrip1";
            // 
            // MenuStrip_Main
            // 
            this.MenuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_File});
            this.MenuStrip_Main.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip_Main.Name = "MenuStrip_Main";
            this.MenuStrip_Main.Size = new System.Drawing.Size(984, 24);
            this.MenuStrip_Main.TabIndex = 1;
            this.MenuStrip_Main.Text = "menuStrip1";
            // 
            // SplitContainer_Main
            // 
            this.SplitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer_Main.Location = new System.Drawing.Point(0, 24);
            this.SplitContainer_Main.Name = "SplitContainer_Main";
            // 
            // SplitContainer_Main.Panel1
            // 
            this.SplitContainer_Main.Panel1.Controls.Add(this.FileBrowser_Left);
            // 
            // SplitContainer_Main.Panel2
            // 
            this.SplitContainer_Main.Panel2.Controls.Add(this.FileBrowser_Right);
            this.SplitContainer_Main.Size = new System.Drawing.Size(984, 515);
            this.SplitContainer_Main.SplitterDistance = 489;
            this.SplitContainer_Main.TabIndex = 2;
            // 
            // FileBrowser_Left
            // 
            this.FileBrowser_Left.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileBrowser_Left.Location = new System.Drawing.Point(0, 0);
            this.FileBrowser_Left.Name = "FileBrowser_Left";
            this.FileBrowser_Left.Size = new System.Drawing.Size(489, 515);
            this.FileBrowser_Left.TabIndex = 0;
            // 
            // FileBrowser_Right
            // 
            this.FileBrowser_Right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileBrowser_Right.Location = new System.Drawing.Point(0, 0);
            this.FileBrowser_Right.Name = "FileBrowser_Right";
            this.FileBrowser_Right.Size = new System.Drawing.Size(491, 515);
            this.FileBrowser_Right.TabIndex = 1;
            // 
            // ToolStripMenuItem_File
            // 
            this.ToolStripMenuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_File_Exit});
            this.ToolStripMenuItem_File.Name = "ToolStripMenuItem_File";
            this.ToolStripMenuItem_File.Size = new System.Drawing.Size(37, 20);
            this.ToolStripMenuItem_File.Text = "&File";
            // 
            // ToolStripMenuItem_File_Exit
            // 
            this.ToolStripMenuItem_File_Exit.Name = "ToolStripMenuItem_File_Exit";
            this.ToolStripMenuItem_File_Exit.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem_File_Exit.Text = "&Exit";
            this.ToolStripMenuItem_File_Exit.Click += new System.EventHandler(this.ToolStripMenuItem_File_Exit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.SplitContainer_Main);
            this.Controls.Add(this.StatusStrip_Main);
            this.Controls.Add(this.MenuStrip_Main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip_Main;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CryptCommander";
            this.MenuStrip_Main.ResumeLayout(false);
            this.MenuStrip_Main.PerformLayout();
            this.SplitContainer_Main.Panel1.ResumeLayout(false);
            this.SplitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer_Main)).EndInit();
            this.SplitContainer_Main.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip_Main;
        private System.Windows.Forms.MenuStrip MenuStrip_Main;
        private System.Windows.Forms.SplitContainer SplitContainer_Main;
        private Controls.FileBrowser FileBrowser_Left;
        private Controls.FileBrowser FileBrowser_Right;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_File;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_File_Exit;
    }
}

