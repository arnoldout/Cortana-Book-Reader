using EpubSharp;
using System.Collections.Generic;

namespace CortanaBookReader
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
        public void parseEpub(string filePath)
        {
            EpubBook book = EpubReader.Read(filePath);

            // Read metadata
            string title = book.Title;
            
            // Get table of contents
            ICollection<EpubChapter> chapters = book.TableOfContents;
            string text = book.ToPlainText();
            text = text;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Form1";
            parseEpub(@"c:\users\olivr\documents\visual studio 2015\Projects\CortanaBookReader\CortanaBookReader\AnimalFarm.epub");
        }

        #endregion
    }
}

