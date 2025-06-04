namespace WinFormsApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            btnSaveAsPdf = new Button();
            btnScanToView = new Button();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            SuspendLayout();
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Location = new Point(25, 12);
            webView.Name = "webView";
            webView.Size = new Size(613, 426);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            // 
            // btnSaveAsPdf
            // 
            btnSaveAsPdf.Location = new Point(676, 77);
            btnSaveAsPdf.Name = "btnSaveAsPdf";
            btnSaveAsPdf.Size = new Size(90, 23);
            btnSaveAsPdf.TabIndex = 1;
            btnSaveAsPdf.Text = "SaveAsPdf";
            btnSaveAsPdf.UseVisualStyleBackColor = true;
            btnSaveAsPdf.Click += btnSaveAsPdf_Click;

            btnScanToView.Location = new Point(676, 37);
            btnScanToView.Name = "btnScanToView";
            btnScanToView.Size = new Size(90, 23);
            btnScanToView.TabIndex = 1;
            btnScanToView.Text = "ScanToView";
            btnScanToView.UseVisualStyleBackColor = true;
            btnScanToView.Click += btnScanToView_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnScanToView);
            Controls.Add(btnSaveAsPdf);
            Controls.Add(webView);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            ResumeLayout(false);

            this.Load += new System.EventHandler(this.Window_Loaded);
            this.FormClosing += Window_Closing;

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private Button btnSaveAsPdf;
        private Button btnScanToView;
    }
}
