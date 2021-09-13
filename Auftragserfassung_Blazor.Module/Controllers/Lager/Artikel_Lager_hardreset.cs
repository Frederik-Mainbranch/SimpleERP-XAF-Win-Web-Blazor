using Auftragserfassung_Blazor.Module.BusinessObjects;
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
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    public partial class Artikel_Lager_hardreset : ViewController
    {
        SimpleAction action_sa_Artikel_Lager_hardreset;
        ParametrizedAction action_pa_Artikel_ErschaffeLieferschein;
        public Artikel Artikel { get; set; }

        public Artikel_Lager_hardreset()
        {
            InitializeComponent();
            TargetObjectType = typeof(Artikel);
            TargetViewType = ViewType.DetailView;

            action_sa_Artikel_Lager_hardreset = new SimpleAction(this, "Artikel_Lager_hardreset", PredefinedCategory.View);
            action_sa_Artikel_Lager_hardreset.TargetObjectType = typeof(Artikel);
            action_sa_Artikel_Lager_hardreset.TargetViewType = ViewType.DetailView;
            action_sa_Artikel_Lager_hardreset.Execute += Action_Artikel_Lager_hardreset_Execute;

            action_pa_Artikel_ErschaffeLieferschein = new ParametrizedAction(this, "Artikel_ErschaffeLieferschein", PredefinedCategory.View, typeof(int));
            action_pa_Artikel_ErschaffeLieferschein.TargetObjectType = typeof(Artikel);
            action_pa_Artikel_ErschaffeLieferschein.ToolTip = "Eingegebene Anzahl = Liefermenge";
            action_pa_Artikel_ErschaffeLieferschein.TargetViewType = ViewType.DetailView;
            action_pa_Artikel_ErschaffeLieferschein.Execute += Action_Artikel_ErschaffeLieferschein_Execute;
        }

        private void Action_Artikel_ErschaffeLieferschein_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            Session session = ((XPObjectSpace)this.ObjectSpace).Session;
            Artikel = (Artikel)(e.CurrentObject);
            Lieferant lieferant = null;

            if (Artikel.LieferantenListe.Count == 0)
            {
                throw new UserFriendlyException("Es wurde kein Lieferant gefunden!");
            }
            else
            {
                lieferant = Artikel.LieferantenListe[0];
            }

            LieferantenLieferung neueLieferung = new LieferantenLieferung(session);
            neueLieferung.Lieferant = lieferant;
            neueLieferung.LieferantenLieferungWurdeKommittet = true;
            neueLieferung.Lieferdatum = DateTime.Today;

            SelectedData maxnummer = session.ExecuteQuery($"SELECT MAX({nameof(LieferantenLieferung.LieferantenLieferscheinnummer)}) FROM {nameof(LieferantenLieferung)}");
            if ((maxnummer.ResultSet[0].Rows[0].Values[0] != null))
            {
                neueLieferung.LieferantenLieferscheinnummer = int.Parse(maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
            }
            else
            {
                neueLieferung.LieferantenLieferscheinnummer = 1;
            }

            BinaryOperator bo_wareneingang = new BinaryOperator($"{nameof(Lager.Wareneingang)}", "true");
            XPCollection<Lager> alleWareneingangsLager = new XPCollection<Lager>(session, bo_wareneingang);
            bool treffer = false;
            foreach (Lager wareneingangsLager in alleWareneingangsLager)
            {
                if(treffer == true)
                {
                    break;
                }

                foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in wareneingangsLager.Lager_ArtikeLager_ZugehoerigkeitsListe)
                {
                    if(zugehoerigkeit.Artikel == Artikel)
                    {
                        treffer = true;
                        neueLieferung.Wareneingang_Lager = wareneingangsLager;
                        break;
                    }
                }
            }
            if(neueLieferung.Wareneingang_Lager == null)
            {
                throw new UserFriendlyException("Es konnte kein Wareneingangslager ermittelt werden!");
            }

            LieferantenLieferungPosition neuePosi = new LieferantenLieferungPosition(session);
            neuePosi.LieferantenLieferung = neueLieferung;
            neuePosi.Artikel = Artikel;
            neuePosi.Lieferant = lieferant;
            neuePosi.Liefermenge = (int)(e.ParameterCurrentValue);
            neuePosi.Positionsnummer = 1;

            ((ParametrizedAction)sender).Value = 0;

            if (ObjectSpace.IsModified == true)
            {
                ObjectSpace.CommitChanges();
                View.Refresh(true);
            }
        }

        private void Action_Artikel_Lager_hardreset_Execute(object sender, SimpleActionExecuteEventArgs e)
        { //Setzt alle Lagerplätze für den Artikel zurück und löscht alle Warenbewegungen, die der Artikel beinhalten

            Session session = ((XPObjectSpace)this.ObjectSpace).Session;
            Artikel = (Artikel)(e.CurrentObject);
            Artikel.Save();
            List<Lager> modifizierteLager = new List<Lager>();

            BinaryOperator bo_artikel = new BinaryOperator("Artikel", Artikel);
            XPCollection<Lagerplatz> lagerplatzListe = new XPCollection<Lagerplatz>(session, bo_artikel);

            foreach (Lagerplatz lagerplatz in lagerplatzListe)
            {
                lagerplatz.Artikel = null;
                lagerplatz.AnzahlDerArtikel = 0;
                lagerplatz.Anzahl_Reserviert = 0;
                lagerplatz.LagerplatzIstGesperrt = false;
                lagerplatz.LagerplatzIstReserviert = false;
                lagerplatz.ReserviertFuerWarenBewegung = null;
                if (modifizierteLager.Contains(lagerplatz.Lager) == false)
                {
                    modifizierteLager.Add(lagerplatz.Lager);
                }
            }

            foreach (Lager lager in modifizierteLager)
            {
                lager.Save();
            }

            XPCollection<Waren_Bewegung> waren_BewegungListe = new XPCollection<Waren_Bewegung>(session, bo_artikel);
            for (int i = waren_BewegungListe.Count -1; i >= 0; i--)
            {
                waren_BewegungListe[i].Delete();
            }

            if (ObjectSpace.IsModified == true)
            {
                ObjectSpace.CommitChanges();
                View.Refresh(true);
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
            //action_sa_Artikel_Lager_hardreset.Execute -= Action_Artikel_Lager_hardreset_Execute;
            //action_pa_Artikel_ErschaffeLieferschein.Execute -= Action_Artikel_ErschaffeLieferschein_Execute;
        }
    }
}
