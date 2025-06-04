namespace iTasks
{
    partial class frmConsultaTarefasEmCurso
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
            this.gvTarefasEmCurso = new System.Windows.Forms.DataGridView();
            this.btFechar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvTarefasEmCurso)).BeginInit();
            this.SuspendLayout();
            // 
            // gvTarefasEmCurso
            // 
            this.gvTarefasEmCurso.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvTarefasEmCurso.Location = new System.Drawing.Point(16, 15);
            this.gvTarefasEmCurso.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gvTarefasEmCurso.Name = "gvTarefasEmCurso";
            this.gvTarefasEmCurso.RowHeadersWidth = 51;
            this.gvTarefasEmCurso.Size = new System.Drawing.Size(1368, 486);
            this.gvTarefasEmCurso.TabIndex = 0;
            // 
            // btFechar
            // 
            this.btFechar.Location = new System.Drawing.Point(1245, 511);
            this.btFechar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btFechar.Name = "btFechar";
            this.btFechar.Size = new System.Drawing.Size(139, 28);
            this.btFechar.TabIndex = 30;
            this.btFechar.Text = "Fechar";
            this.btFechar.UseVisualStyleBackColor = true;
            this.btFechar.Click += new System.EventHandler(this.btFechar_Click);
            // 
            // frmConsultaTarefasEmCurso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 554);
            this.Controls.Add(this.btFechar);
            this.Controls.Add(this.gvTarefasEmCurso);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmConsultaTarefasEmCurso";
            this.Text = "frmConsultaTarefasEmCurso";
            ((System.ComponentModel.ISupportInitialize)(this.gvTarefasEmCurso)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvTarefasEmCurso;
        private System.Windows.Forms.Button btFechar;
    }
}