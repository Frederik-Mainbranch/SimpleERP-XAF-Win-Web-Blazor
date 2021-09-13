using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Auftragserfassung_Blazor.Module.Helpers;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ArtikelUntergruppen_reset_txtDB : ViewController
    {
        private SimpleAction ArtikelUntergruppen_reset;

        public ArtikelUntergruppen_reset_txtDB()
        {
            InitializeComponent();
            TargetObjectType = typeof(ArtikelGruppe);
            TargetViewType = ViewType.ListView;

            ArtikelUntergruppen_reset = new SimpleAction(this, "ArtikelGruppen_reset_txtDB", PredefinedCategory.Edit);
            ArtikelUntergruppen_reset.Execute += ArtikelUntergruppen_reset_Execute;
        }

        private void ArtikelUntergruppen_reset_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            txtListenHelper txtListenHelper = new txtListenHelper();
            Session session = ((XPObjectSpace)this.ObjectSpace).Session;
            string[] artikelUntergruppeTxt = txtListenHelper.BesorgeGanzeListe("ArtikelUntergruppe");
            string[] artikelUntergruppe_ArtikelGruppeTxt = txtListenHelper.BesorgeGanzeListe("ArtikelUntergruppe_ArtikelGruppe");
            List<ArtikelGruppe> zugefügteArtikelGruppen = new List<ArtikelGruppe>();

            NullOperator criteria = new NullOperator("GCRecord");
            XPCollection<ArtikelUntergruppe> vorhandeneUntergruppen =  new XPCollection<ArtikelUntergruppe>(session, criteria);


            //int counterArtikelGruppenNummer = 0;
            //SelectedData Maxnummer = session.ExecuteQuery("SELECT MAX(ArtikelgruppenNummer) FROM ArtikelGruppe WHERE GCRecord IS NULL");
            //if ((Maxnummer.ResultSet[0].Rows[0].Values[0] != null))
            //{
            //    counterArtikelGruppenNummer = int.Parse(Maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
            //}
            //else
            //{
            //    counterArtikelGruppenNummer = 1;
            //}

            for (int j = 0; j < artikelUntergruppeTxt.Length; j++)
            {
                bool untergruppeSchonVorhanden = false;
                for (int i = 0; i < vorhandeneUntergruppen.Count; i++)
                {
                    if ((vorhandeneUntergruppen[i]).Bezeichnung == artikelUntergruppeTxt[j])
                    {
                        untergruppeSchonVorhanden = true;
                        break;
                    }
                }

                if(untergruppeSchonVorhanden == false)
                {
                    ArtikelUntergruppe artikelUntergruppe = (ArtikelUntergruppe)Activator.CreateInstance(typeof(ArtikelUntergruppe), session);
                    artikelUntergruppe.BearbeitungDurchViewController = true;


                    bool artikelGruppeWurdeSchonHinzugefügt = false;
                    int positionDerArtikelGruppe = 0;
                    for (int i = 0; i < zugefügteArtikelGruppen.Count; i++)
                    {
                        if ((zugefügteArtikelGruppen[i]).Bezeichnung == artikelUntergruppe_ArtikelGruppeTxt[j])
                        {
                            artikelGruppeWurdeSchonHinzugefügt = true;
                            positionDerArtikelGruppe = i;
                            break;
                        }
                    }

                    //ArtikelGruppe
                    BinaryOperator Bop_ArtikelGruppe_Bezeichnung = new BinaryOperator("Bezeichnung", artikelUntergruppe_ArtikelGruppeTxt[j]);
                    if ((session.FindObject(typeof(ArtikelGruppe), Bop_ArtikelGruppe_Bezeichnung) == null)
                        && (artikelGruppeWurdeSchonHinzugefügt == false))
                    {
                        ArtikelGruppe artikelGruppe = new ArtikelGruppe(session);
                        artikelGruppe.Bezeichnung = artikelUntergruppe_ArtikelGruppeTxt[j];
                        //artikelGruppe.ArtikelgruppenNummer = counterArtikelGruppenNummer;
                        //counterArtikelGruppenNummer++;
                        artikelUntergruppe.ArtikelGruppe = artikelGruppe;
                        zugefügteArtikelGruppen.Add(artikelGruppe);
                    }
                    else if (artikelGruppeWurdeSchonHinzugefügt == true)
                    {
                        artikelUntergruppe.ArtikelGruppe = zugefügteArtikelGruppen[positionDerArtikelGruppe];
                    }
                    else
                    {
                        artikelUntergruppe.ArtikelGruppe = (ArtikelGruppe)session.FindObject(typeof(ArtikelGruppe), Bop_ArtikelGruppe_Bezeichnung);
                    }

                    //Bezeichnung
                    artikelUntergruppe.Bezeichnung = artikelUntergruppeTxt[j];

                    //UntergruppenNummer
                    if (artikelUntergruppe.ArtikelGruppe.ArtikelUntergruppenListe.Count == 0)
                    {
                        artikelUntergruppe.ArtikelUntergruppenNummer = 1;
                    }
                    else
                    {
                        int neueNummer = 0;
                        foreach (ArtikelUntergruppe artikelUntergruppe2 in artikelUntergruppe.ArtikelGruppe.ArtikelUntergruppenListe)
                        {
                            if (artikelUntergruppe2.ArtikelUntergruppenNummer > neueNummer)
                            {
                                neueNummer = artikelUntergruppe2.ArtikelUntergruppenNummer;
                            }
                        }
                        artikelUntergruppe.ArtikelUntergruppenNummer = neueNummer + 1;
                    }

                    this.ObjectSpace.CommitChanges();
                }
            }
            this.View.Refresh(true);
        }
    }
}
