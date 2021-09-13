using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using Auftragserfassung_Blazor.Module.Helpers;
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
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    public partial class Artikel_txtDB : txtDatenbankViewController
    {
        public Artikel_txtDB()
        {
            InitializeComponent();
            TargetObjectType = typeof(Artikel);
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Root;
        }

        protected override void SetzteZielObject(Session session, int anzahl, Type type)
        {
            XPCollection<ArtikelGruppe> ArtikelGruppeListe = new XPCollection<ArtikelGruppe>(session);
            if(ArtikelGruppeListe.Count == 0)
            {
                throw new UserFriendlyException("Es sind noch keine Artikelgruppen angelegt!");
            }
            CriteriaOperator istStandard = new BinaryOperator("IstStandard", "True");
            Steuer standardSteuer = ((Steuer)session.FindObject(typeof(Steuer), istStandard));
            AktionsHelper2000 aktionsHelper = new AktionsHelper2000(session);

            for (int i = 0; i < anzahl; i++)
            {
                Artikel artikel = (Artikel)Activator.CreateInstance(type, session);
                artikel.BearbeitungDurchViewController = true;

                //
                artikel.BenutzteGruppenRabatte = true;
                artikel.BenutzeUntergruppenRabatte = true;

                int zufälligeArtikelgruppe = zufallsWertFeld.Next(0, ArtikelGruppeListe.Count);
                artikel.ArtikelGruppe = ArtikelGruppeListe[zufälligeArtikelgruppe];
                BinaryOperator criteria = new BinaryOperator("ArtikelGruppe", artikel.ArtikelGruppe);
                XPCollection<ArtikelUntergruppe> ArtikelUntergruppeListe = new XPCollection<ArtikelUntergruppe>(session, criteria);
                if(ArtikelUntergruppeListe.Count == 0)
                {
                    throw new UserFriendlyException("Es sind noch keine Artikeluntergruppen angelegt!");
                }

                int zufälligeArtikelUntergruppe = zufallsWertFeld.Next(0, ArtikelUntergruppeListe.Count);
                //ArtikelUntergruppe
                if (ArtikelUntergruppeListe.Count > 0)
                {
                    artikel.ArtikelUntergruppe = ArtikelUntergruppeListe[zufälligeArtikelUntergruppe];
                }
                else
                {
                    artikel.ArtikelUntergruppe = null;
                }
                //Bezeichnung
                artikel.Bezeichnung = ZufälligerWertString(Properties.Resources.Artikelnamen_englisch);
                //Preis
                int preis = zufallsWertFeld.Next(0, 2);
                if (preis == 0)
                {
                    artikel.StandardPreis = double.Parse(ZufälligerWertString(Properties.Resources.preise_unter_100));
                }
                else
                {
                    artikel.StandardPreis = double.Parse(ZufälligerWertString(Properties.Resources.preise_zwischen_100_und_1000));
                }
                //Steuersatz

                if (artikel.ArtikelUntergruppe.Steuersatz != null)
                {
                    artikel.Steuersatz = artikel.ArtikelUntergruppe.Steuersatz;
                }
                else if(artikel.ArtikelGruppe.Steuersatz != null)
                {
                    artikel.Steuersatz = artikel.ArtikelGruppe.Steuersatz;
                }
                else
                {
                    artikel.Steuersatz = standardSteuer;
                }

                //rabatt
                aktionsHelper.UpdateAktuellenRabatt_Artikel(artikel);
                aktionsHelper.UpdateAktuellenPreis(artikel);

                //aktueller Preis

                //

                artikel.BearbeitungDurchViewController = false;
            }
        }
    }
}
