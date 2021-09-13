using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.IO;
using Auftragserfassung_Blazor.Module.Helpers;

namespace Auftragserfassung_Blazor.Module.Controllers
{
//    public class AddZufälligeArtikel : ObjectViewController<DevExpress.ExpressApp.ListView, Artikel>
//    {
//        public AddZufälligeArtikel()
//        {
//            ParametrizedAction addArtikel = new ParametrizedAction(
//            this, "Füge zufällige Artikel hinzu", PredefinedCategory.Menu, typeof(int));
//            addArtikel.Execute += AddArtikelAction_Execute;
//        }


//            private void AddArtikelAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
//            {
//                int benutzerEingabe = (int)(e.ParameterCurrentValue);

//                if (benutzerEingabe < 1)
//                {
//                    XtraMessageBox.Show("Bitte geben Sie einen Wert größer als 0 ein!");
//                    ((ParametrizedAction)sender).Value = 0;
//                }
//                else
//                {
//                    if (XtraMessageBox.Show("Wollen Sie wirklich " + benutzerEingabe + " zufällige Artikel hinzufügen?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
//                    {
//                        string artikel_Bezeichnung = @"C:\Users\frederik\Documents\Einstellungen\AuftragserfassungXAF\Artikel\artikel bezeichnung.txt";
//                        string artikel_Preis = @"C:\Users\frederik\Documents\Einstellungen\AuftragserfassungXAF\Artikel\preise.txt";
//                        string[] artikel_Bezeichnung_zeilen = File.ReadAllLines(artikel_Bezeichnung);
//                        string[] artikel_preis_zeilen = File.ReadAllLines(artikel_Preis);

//                        Random rnd = new Random();

//                        using (IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Artikel)))
//                        {
//                            for (int i = 0; i < benutzerEingabe; i++)
//                            {
//                                int a = rnd.Next(0, artikel_Bezeichnung_zeilen.Length);
//                                int b = rnd.Next(0, artikel_preis_zeilen.Length);
//                                int c = rnd.Next(0, 2);

//                                Artikel artikel = objectSpace.CreateObject<Artikel>();
//                                artikel.Bezeichnung = "" + artikel_Bezeichnung_zeilen[a];

//                                if (c == 0)
//                                {
//                                    artikel.Steuersatz = 7;
//                                }
//                                else
//                                {
//                                    artikel.Steuersatz = 19;
//                                }
//                                try
//                                {
//                                    artikel.Preis = double.Parse(artikel_preis_zeilen[b]);
//                                }
//                                catch (Exception)
//                                {
//                                }
//                                try
//                                {
//                                    objectSpace.CommitChanges();
//                                }
//                                catch (Exception)
//                                {
//                                    MessageBox.Show("Fehler beim erstellen des Artikels mit folgender Bezeichnung: \n" + artikel.Bezeichnung);
//                                }
//                            }
//                            ((ParametrizedAction)sender).Value = 0;
//                            View.Refresh(true);
//                        }
//                    }
//                }
//            }

//        }//Klasse
    }//Namespace

    //public class AddZehnArtikelController : ObjectViewController<ListView, Artikel>
    //{
    //    public AddZehnArtikelController()
    //    {
    //        SimpleAction addZehnArtikel = new SimpleAction(this, "Erstelle 10 Artikel mit ObjectSpace", PredefinedCategory.Edit);
    //        addZehnArtikel.Execute += AddZehnArtikelAction_Execute;
    //    }
    //    private void AddZehnArtikelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    //    {
    //        for (int i = 0; i < 10; i++)
    //        {
    //            using (IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Artikel)))
    //            {
    //                Artikel artikel = objectSpace.CreateObject<Artikel>();
    //                artikel.Bezeichnung = artikel + "";
    //                artikel.Preis = artikel.BerechneteArtikelnummer * artikel.BerechneteArtikelnummer;
    //                objectSpace.CommitChanges();
    //                artikel.Bezeichnung = artikel + "";
    //                artikel.Preis = artikel.BerechneteArtikelnummer * artikel.BerechneteArtikelnummer;
    //                objectSpace.CommitChanges();
    //            }
    //        }
    //        View.Refresh(true);
    //    }
    //}

    //public class AddZehnArtikelController2 : ObjectViewController<DetailView, Artikel>
    //{
    //    public AddZehnArtikelController2()
    //    {
    //        SimpleAction addZehnArtikel = new SimpleAction(this, "Preis + 10€ Version 2", PredefinedCategory.Edit);
    //        addZehnArtikel.Execute += AddZehnArtikelAction_Execute;
    //    }
    //    private void AddZehnArtikelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    //    {
    //        string debug1 = sender.ToString();
    //        string debug2 = (((Artikel)(e.CurrentObject)).GetType()).ToString();
    //        string debug3 = e.ToString();
    //        string debug4 = e.Action.ToString();

    //        double t1 = (e.CurrentObject as Artikel).Preis; 
    //        (e.CurrentObject as Artikel).Preis += 10;
    //        double t2 = (e.CurrentObject as Artikel).Preis;

    //        using (IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Artikel)))
    //        {
    //            double t3 = (e.CurrentObject as Artikel).Preis;
    //            (e.CurrentObject as Artikel).Preis += 10;
    //            double t4 = (e.CurrentObject as Artikel).Preis;

    //          //  ((Artikel)e.CurrentObject)
    //        }
    //        View.Refresh();
    //    }
    //}

    //public class ChangePreis : ObjectViewController<DetailView, Artikel>
    //{
    //    public ChangePreis()
    //    {
    //        SingleChoiceAction changePreis = new SingleChoiceAction(this, "Preisgdsgasgs + 10€", PredefinedCategory.Edit);
    //        changePreis.Execute += PreisPlus10_Execute;
    //    }
    //    private void PreisPlus10_Execute(object sender, SimpleActionExecuteEventArgs e)
    //    {
    //        string debug1 = sender.ToString();
    //        string debug2 = e.CurrentObject.ToString();
    //        string debug3 = e.ToString();
    //        string debug4 = e.Action.ToString();

    //        //for (int i = 0; i < 10; i++)
    //        //{
    //            using (IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Artikel)))
    //            {
    //        //        Artikel artikel = objectSpace.CreateObject<Artikel>();
    //        //        artikel.Bezeichnung = artikel + "";
    //        //        artikel.Preis = artikel.BerechneteArtikelnummer * artikel.BerechneteArtikelnummer;
    //        //        objectSpace.CommitChanges();
    //        //        artikel.Bezeichnung = artikel + "";
    //        //        artikel.Preis = artikel.BerechneteArtikelnummer * artikel.BerechneteArtikelnummer;
    //        //        objectSpace.CommitChanges();
    //            }
    //        //}
    //        View.Refresh(true);
    //    }
    //}

