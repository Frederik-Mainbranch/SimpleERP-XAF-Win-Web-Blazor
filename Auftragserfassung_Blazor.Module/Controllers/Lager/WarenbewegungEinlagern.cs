using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class WarenbewegungEinlagern : ViewController
    {
        public WarenbewegungEinlagern()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
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
        }

        private void Action_WarenbewegungEinlagern_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            List<Lager> modifizierteLagerListe = new List<Lager>();
            List<Artikel> modifizierteArtikelListe = new List<Artikel>();
            foreach (Waren_Bewegung waren_Bewegung in e.SelectedObjects)
            {
                if(waren_Bewegung.WareHatZielErreicht == false)
                {
                    Lagerplatz lagerplatz_ziel = waren_Bewegung.Lagerplatz_Ziel;

                    //Lagerplatz Ziel
                    lagerplatz_ziel.Artikel = waren_Bewegung.Artikel;
                    lagerplatz_ziel.AnzahlDerArtikel = waren_Bewegung.Anzahl;
                    lagerplatz_ziel.LagerplatzIstReserviert = false;
                    lagerplatz_ziel.ReserviertFuerWarenBewegung = null;

                    if(lagerplatz_ziel.Lager.Warenausgang == true)
                    {
                        lagerplatz_ziel.Artikel.AnzahlImLager -= waren_Bewegung.Anzahl;
                        lagerplatz_ziel.Artikel.AnzahlImVersandprozess += waren_Bewegung.Anzahl;
                    }

                    //Lagerplatz Herkunft
                    waren_Bewegung.Lagerplatz_Herkunft.Artikel = null;
                    waren_Bewegung.Lagerplatz_Herkunft.AnzahlDerArtikel = 0;
                    waren_Bewegung.Lagerplatz_Herkunft.Anzahl_Reserviert = 0;
                    waren_Bewegung.Lagerplatz_Herkunft.LagerplatzIstReserviert = false;
                    waren_Bewegung.Lagerplatz_Herkunft.ReserviertFuerWarenBewegung = null;
                    
                    //Warenbewegung
                    waren_Bewegung.WareHatZielErreicht = true;

                    //jedes modifiziertes Lager wird zum Schluss gespeichert, damit die Lagerauslastung neu berechnet werden kann
                    if (modifizierteLagerListe.Contains(waren_Bewegung.Lager_Herkunft) == false)
                    {
                        modifizierteLagerListe.Add(waren_Bewegung.Lager_Herkunft);
                    }
                    if (modifizierteLagerListe.Contains(lagerplatz_ziel.Lager) == false)
                    {
                        modifizierteLagerListe.Add(lagerplatz_ziel.Lager);
                    }
                    if (modifizierteArtikelListe.Contains(lagerplatz_ziel.Artikel) == false)
                    {
                        modifizierteArtikelListe.Add(lagerplatz_ziel.Artikel);
                    }
                }
            }

            foreach (Lager lager in modifizierteLagerListe)
            {
                lager.Save();
            }

            foreach (Artikel artikel in modifizierteArtikelListe)
            {
                artikel.BerechneAnzahlAnVerfuegbarenArtikeln();
            }

            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
            }
        }

    }
}
