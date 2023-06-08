﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace GestionPharmacetique.Forme
{
    public partial class StatistiqueFrm : Form
    {
        public StatistiqueFrm()
        {
            InitializeComponent();
        }

        public DateTime dateDebut, dateFin;
        private void StatistiqueFrm_Paint(object sender, PaintEventArgs e)
        {
            Graphics mGraphics = e.Graphics;
            Pen pen1 = new Pen(Color.CadetBlue, 0);
            Rectangle area1 = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush(area1, Color.CadetBlue, Color.CadetBlue, LinearGradientMode.BackwardDiagonal);
            mGraphics.FillRectangle(linearGradientBrush, area1);
            mGraphics.DrawRectangle(pen1, area1);
        }
        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics mGraphics = e.Graphics;
            Pen pen1 = new Pen(Color.CadetBlue, 0);
            Rectangle area1 = new Rectangle(0, 0, this.groupBox1.Width - 1, this.groupBox1.Height - 1);
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush(area1, Color.Black, Color.CadetBlue, LinearGradientMode.BackwardDiagonal);
            mGraphics.FillRectangle(linearGradientBrush, area1);
            mGraphics.DrawRectangle(pen1, area1);
        }

        private void StatistiqueFrm_Load(object sender, EventArgs e)
        {
            try
            {
                //Size = new System.Drawing.Size(Form1.width1  , Form1.height1);
                columnHeader2.Width = lstViewInventaire.Width / 2;
                columnHeader3.Width = lstViewInventaire.Width / 10;
                columnHeader4.Width = lstViewInventaire.Width / 10;
                columnHeader5.Width = lstViewInventaire.Width / 10;
                columnHeader6.Width = lstViewInventaire.Width / 10;
                columnHeader7.Width = lstViewInventaire.Width / 10;
                DataTable dataTable = AppCode.ConnectionClass.RapportStatistiqueParMontant(dateDebut,dateFin);
                lstViewInventaire.Items.Clear();
                decimal pourcent = 0;
                decimal montantTotal = 0;
                foreach (DataRow dtRow in dataTable.Rows)
                {
                    montantTotal += decimal.Parse(dtRow.ItemArray[3].ToString());
                }
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                   pourcent = decimal.Parse(dataTable.Rows[i].ItemArray[3].ToString())*100/montantTotal;
                   string pourcentage = null;
                     if(pourcent.ToString().Length>4)
                        {
                            pourcentage = pourcent.ToString().Substring(0, 4) + "%";
                        }else
                     {
                         pourcentage = pourcent.ToString();
                     }
                     var produit = AppCode.ConnectionClass.ListeDesMedicamentParCode(dataTable.Rows[i].ItemArray[5].ToString());
                    var items = new string[]
                    {
                        dataTable.Rows[i].ItemArray[0].ToString().ToUpper(),
                        produit[0].Quantite.ToString(),
                        dataTable.Rows[i].ItemArray[2].ToString(),
                        dataTable.Rows[i].ItemArray[1].ToString(),                        
                        dataTable.Rows[i].ItemArray[3].ToString(),
                       pourcentage
                    };

                    ListViewItem lstListViewItem = new ListViewItem(items);
                    lstViewInventaire.Items.Add(lstListViewItem);
                }
            }
            catch (Exception exception)
            {
                MonMessageBox.ShowBox("Rapport vente", exception);
            }
        }
       
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        Bitmap _listeImpression;
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(_listeImpression, 5, 20, _listeImpression.Width, _listeImpression.Height);
            e.HasMorePages = false;
        }
      
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox3.Checked)
                {
                    checkBox1.Checked = false;
                    checkBox2.Checked = false;
                    var liste = from p in listeProduit()
                                orderby p.Designation
                                select p;
                    lstViewInventaire.Items.Clear();
                    foreach (var p in liste)
                    {
                        var items = new string[]
                    {
                        p.Designation,
                       p.Quantite.ToString(),
                        p.NombreVente.ToString(),
                        p.PrixVente.ToString(),
                       p.PrixTotal.ToString(),
                       p.NumeroMedicament.ToString()
                    };

                        var lstItems = new ListViewItem(items);
                        lstViewInventaire.Items.Add(lstItems);
                    }
                }
            }
            catch (Exception ex)
            { MonMessageBox.ShowBox("", ex); }
        }

        List<AppCode.Vente> listeProduit()
        {
            var listeItems = new List<AppCode.Vente>();
            foreach (ListViewItem lstItems in lstViewInventaire.Items)
            {
                var produit = new AppCode.Vente();
                produit.Designation= lstItems.SubItems[0].Text;
                produit.Quantite = Convert.ToInt32(lstItems.SubItems[1].Text);
                produit.PrixVente = Convert.ToDecimal(lstItems.SubItems[3].Text);
                produit.NombreVente = Convert.ToInt32(lstItems.SubItems[2].Text);
                produit.PrixTotal = Convert.ToDecimal(lstItems.SubItems[4].Text);
                produit.NumeroMedicament = lstItems.SubItems[5].Text;
                listeItems.Add(produit);
            }

            return listeItems;
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                var liste = from p in listeProduit()
                            orderby p.PrixTotal descending
                            select p;
                lstViewInventaire.Items.Clear();
                foreach (var p in liste)
                {
                    var items = new string[]
                    {
                        p.Designation,
                       p.Quantite.ToString(),
                        p.NombreVente.ToString(),
                        p.PrixVente.ToString(),
                       p.PrixTotal.ToString(),
                       p.NumeroMedicament.ToString()
                    };
                    var lstItems = new ListViewItem(items);
                    lstViewInventaire.Items.Add(lstItems);
                }
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox3.Checked = false;
                checkBox2.Checked = false;
                var liste = from p in listeProduit()
                            orderby p.NombreVente descending
                            select p;
                lstViewInventaire.Items.Clear();
                foreach (var p in liste)
                {
                    var items = new string[]
                    {
                        p.Designation,
                       p.Quantite.ToString(),
                        p.NombreVente.ToString(),
                        p.PrixVente.ToString(),
                       p.PrixTotal.ToString(),
                       p.NumeroMedicament.ToString()
                    };
                    var lstItems = new ListViewItem(items);
                    lstViewInventaire.Items.Add(lstItems);
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lstViewInventaire.Items.Count > 0)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "PDF Documents (*.pdf)|*.pdf";

                        var titre = "Statistique de vente du " + dateDebut.ToShortDateString() + " au " + dateFin.ToShortDateString();
                        sharpPDF.pdfDocument document = new sharpPDF.pdfDocument("christian", "cdali");
                        var jour = DateTime.Now.Day;
                        var mois = DateTime.Now.Month;
                        var year = DateTime.Now.Year;
                        var hour = DateTime.Now.Hour;
                        var min = DateTime.Now.Minute;
                        var sec = DateTime.Now.Second;
                        var date = jour.ToString() + "_" + mois.ToString() + "_" + year.ToString() + "_" + hour + "_" + min + "_" + sec;

                        var pathFolder = "C:\\Dossier Pharmacie";
                        if (!System.IO.Directory.Exists(pathFolder))
                        {
                            System.IO.Directory.CreateDirectory(pathFolder);
                        }
                        pathFolder = pathFolder + "\\Rapport vente";
                        if (!System.IO.Directory.Exists(pathFolder))
                        {
                            System.IO.Directory.CreateDirectory(pathFolder);
                        }
                        sfd.InitialDirectory = pathFolder;
                        var s = titre;
                        if (titre.Contains("/"))
                        { s = s.Replace("/", "_"); }
                        sfd.FileName = s + "_imprimé_le_" + date + ".pdf";

                        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {

                            var count = lstViewInventaire.Items.Count;

                            var index = (lstViewInventaire.Items.Count) / 45;

                            for (var i = 0; i <= index; i++)
                            {
                                if (i * 45 < count)
                                {
                                    var _listeImpression = AppCode.ImprimerRaportVente.ImprimerStatistique(lstViewInventaire, titre, i);

                                    var inputImage = @"cdali" + i;
                                    // Create an empty page
                                    sharpPDF.pdfPage pageIndex = document.addPage();

                                    document.addImageReference(_listeImpression, inputImage);
                                    sharpPDF.Elements.pdfImageReference img1 = document.getImageReference(inputImage);
                                    pageIndex.addImage(img1, -10, 0, pageIndex.height, pageIndex.width);
                                }
                            }

                            document.createPDF(sfd.FileName);
                            System.Diagnostics.Process.Start(sfd.FileName);

                        }

                    }
                
            }
            catch (Exception)
            {
            }
        }
    }
}
