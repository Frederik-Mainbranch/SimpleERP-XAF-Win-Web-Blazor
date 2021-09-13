using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    public partial class leereLager : ViewController
    {
        public leereLager()
        {
            InitializeComponent();
            TargetObjectType = typeof(Lager);
            TargetViewType = ViewType.DetailView;
        }

        private void LagerLeeren_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Session session = ((XPObjectSpace)this.ObjectSpace).Session;
            Lager lager = (Lager)e.CurrentObject;
            BinaryOperator binaryOperator = new BinaryOperator("Lager", lager.Oid);
            XPCollection<Lagerplatz> lagerplatzListe = new XPCollection<Lagerplatz>(session, binaryOperator);
            //lagerplatzListe.Load();

            foreach (Lagerplatz lagerplatz in lagerplatzListe)
            {
                lagerplatz.Artikel = null;
                lagerplatz.AnzahlDerArtikel = 0;
                lagerplatz.Anzahl_Reserviert = 0;
                lagerplatz.LagerplatzIstGesperrt = false;
                lagerplatz.LagerplatzIstReserviert = false;
                lagerplatz.ReserviertFuerWarenBewegung = null;
            }
            lager.Save();
            ObjectSpace.CommitChanges();
            View.Refresh(true);
        }
    }
}
