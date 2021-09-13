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
using DevExpress.Xpo.DB;
using Auftragserfassung_Blazor.Module.Helpers;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("Bezeichnung")]

    public class ArtikelGruppe : BaseObject
    { 
        public ArtikelGruppe(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false && IsDeleted == false && BearbeitungDurchViewController == false)
            {
                if (propertyName == nameof(Steuersatz) && Steuersatz != null)
                {
                    //SteuerHelper2000 verwendeteSteuerErmittler = new SteuerHelper2000(Session);

                    foreach (ArtikelUntergruppe artikelUntergruppe in ArtikelUntergruppenListe)
                    {
                        foreach (Artikel artikel in artikelUntergruppe.ArtikelUntergruppeArtikelListe)
                        {
                            if(artikel.Steuersatz == (Steuer)oldValue)
                            {
                                artikel.Steuersatz = (Steuer)newValue;
                                //artikel.verwendeteSteuer = verwendeteSteuerErmittler.ErmittleVerwendeteSteuer(artikel);
                            }
                        }
                    }
                }
            }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            SelectedData Maxnummer = Session.ExecuteQuery("SELECT MAX(ArtikelgruppenNummer) FROM ArtikelGruppe WHERE GCRecord IS NULL");
            if ((Maxnummer.ResultSet[0].Rows[0].Values[0] != null))
            {
                ArtikelgruppenNummer = int.Parse(Maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
            }
            else
            {
                ArtikelgruppenNummer = 1;
            }

            BinaryOperator criteria = new BinaryOperator("IstStandard", true);
            Steuersatz = (Steuer)Session.FindObject(typeof(Steuer), criteria);
        }


        protected override void OnSaving()
        {
            base.OnSaving();

            txtListenHelper txtListenHelper = new txtListenHelper();
            txtListenHelper.ÜberprüfeAufVorkommenInListe(nameof(ArtikelGruppe), Bezeichnung);
        }


        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private string _Bezeichnung;
        public string Bezeichnung
        {
            get { return _Bezeichnung; }
            set { SetPropertyValue<string>(nameof(Bezeichnung), ref _Bezeichnung, value); }
        }


        private int _ArtikelgruppenNummer;
        [ModelDefault("AllowEdit", "false")]
        public int ArtikelgruppenNummer
        {
            get { return _ArtikelgruppenNummer; }
            set { SetPropertyValue<int>(nameof(ArtikelgruppenNummer), ref _ArtikelgruppenNummer, value); }
        }


        private Steuer _Steuersatz;
        public Steuer Steuersatz
        {
            get { return _Steuersatz; }
            set { SetPropertyValue<Steuer>(nameof(Steuersatz), ref _Steuersatz, value); }
        }


        private double _AktuellerRabatt;
        [ModelDefault("AllowEdit", "false")]
        public double AktuellerRabatt
        {
            get { return _AktuellerRabatt; }
            set { SetPropertyValue<double>(nameof(AktuellerRabatt), ref _AktuellerRabatt, value); }
        }


        //private double _ArtikelGruppenRabatt;
        //public double ArtikelGruppenRabatt
        //{
        //    get { return _ArtikelGruppenRabatt; }
        //    set { SetPropertyValue<double>(nameof(ArtikelGruppenRabatt), ref _ArtikelGruppenRabatt, value); }
        //}


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------


        [DevExpress.Xpo.Aggregated, Association("ArtikelUntergruppe-ArtikelGruppe")]
        public XPCollection<ArtikelUntergruppe> ArtikelUntergruppenListe
        {
            get { return GetCollection<ArtikelUntergruppe>(nameof(ArtikelUntergruppenListe)); }
        }


        [DevExpress.Xpo.Aggregated, Association("ArtikelGruppe-Aktionsrabatt")]
        public XPCollection<Aktionsrabatt> AktionsRabattListe
        {
            get { return GetCollection<Aktionsrabatt>(nameof(AktionsRabattListe)); }
        }


        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------


        public int AnzahlDerUntergruppen
        {
            get { return ArtikelUntergruppenListe.Count; }
        }

        private int anzahlDerArtikel;
        public int AnzahlDerArtikel
        {
            get {
                anzahlDerArtikel = 0;
                foreach (ArtikelUntergruppe artikelUntergruppe in ArtikelUntergruppenListe)
                {
                    anzahlDerArtikel += artikelUntergruppe.AnzahlDerProdukte;
                }
                return anzahlDerArtikel; }
        }


        private bool _BearbeitungDurchViewController;
        [NonPersistent]
        [Browsable(false)]
        public bool BearbeitungDurchViewController
        {
            get { return _BearbeitungDurchViewController; }
            set { SetPropertyValue<bool>(nameof(BearbeitungDurchViewController), ref _BearbeitungDurchViewController, value); }
        }

        //-------------------------------- Non Persistent Properties ---------------------------------------------
    }
}