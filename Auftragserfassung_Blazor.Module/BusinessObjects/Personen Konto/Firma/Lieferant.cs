using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DisplayNameAttribute = DevExpress.Xpo.DisplayNameAttribute;
using DevExpress.ExpressApp.ConditionalAppearance;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Xpo.DB;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]

    public class Lieferant : Firma
    { 
        public Lieferant(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        protected override void OnSaving()
        {
            base.OnSaving();

            if(IsDeleted == false && BearbeitungDurchViewController == false)
            {
                if (Lieferantennummer == 0)
                {
                    SelectedData Maxnummer = Session.ExecuteQuery("SELECT MAX(Kundennummer) FROM Kunde");
                    if ((Maxnummer.ResultSet[0].Rows[0].Values[0] != null))
                    {
                        Lieferantennummer = int.Parse(Maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
                    }
                    else
                    {
                        Lieferantennummer = 1;
                    }
                }
            }
        }

        //---------------------------- Override Methode -------------------------------


        //-------------------------------- Properties ---------------------------------------------






        private int _Lieferantennummer;
        [ModelDefault("AllowEdit", "false")]
        [Appearance("Verstecke Lieferantennummer", criteria: nameof(Lieferantennummer) + "= 0", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]

        public int Lieferantennummer
        {
            get { return _Lieferantennummer; }
            set { SetPropertyValue<int>(nameof(Lieferantennummer), ref _Lieferantennummer, value); }
        }




        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------


        [DevExpress.Xpo.Aggregated, Association("Lieferant-LieferantenLieferung")]
        public XPCollection<LieferantenLieferung> LieferantenLieferungenListe
        {
            get { return GetCollection<LieferantenLieferung>(nameof(LieferantenLieferungenListe)); }
        }


        [Association("Lieferant-Artikel")]
        public XPCollection<Artikel> ArtikelListe
        {
            get { return GetCollection<Artikel>(nameof(ArtikelListe)); }
        }


        //[Association("Lieferant-ArtikelUntergruppe")]
        //public XPCollection<ArtikelUntergruppe> ArtikelUntergruppenListe
        //{
        //    get { return GetCollection<ArtikelUntergruppe>(nameof(ArtikelUntergruppenListe)); }
        //}


        //[Association("Lieferant-ArtikelGruppe")]
        //public XPCollection<ArtikelGruppe> ArtikelGruppenListe
        //{
        //    get { return GetCollection<ArtikelGruppe>(nameof(ArtikelGruppenListe)); }
        //}




        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------



        //-------------------------------- Non Persistent Properties ---------------------------------------------
    }
}