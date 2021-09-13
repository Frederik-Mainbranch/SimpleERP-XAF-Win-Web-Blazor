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
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo.DB;
using DevExpress.ExpressApp.ConditionalAppearance;
using Auftragserfassung_Blazor.Module.Helpers;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    #region Klasse
    [DefaultClassOptions]
    [DefaultProperty("Bezeichnung")]
    public class ArtikelUntergruppe : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ArtikelUntergruppe(Session session)
            : base(session)
        {
        }


        #endregion

        #region Override Methoden


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if(IsLoading == false && IsSaving == false && BearbeitungDurchViewController == false && IsDeleted == false)
            {
                if(propertyName == nameof(ArtikelGruppe) && newValue != null)
                {
                    if(ArtikelGruppe.ArtikelUntergruppenListe.Count == 0)
                    {
                        ArtikelUntergruppenNummer = 1;
                    }
                    else
                    {
                        int neueNummer = 0;
                        foreach (ArtikelUntergruppe artikelUntergruppe in ArtikelGruppe.ArtikelUntergruppenListe)
                        {
                            if(artikelUntergruppe.ArtikelUntergruppenNummer > neueNummer)
                            {
                                neueNummer = artikelUntergruppe.ArtikelUntergruppenNummer;
                            }
                        }
                        ArtikelUntergruppenNummer = neueNummer + 1;
                    }
                }
                else if (propertyName == nameof(Steuersatz) && Steuersatz != null)
                {
                   // AktionsHelper2000 verwendeteSteuerErmittler = new AktionsHelper2000(Session);

                    foreach (Artikel artikel in ArtikelUntergruppeArtikelListe)
                    {
                        if (artikel.Steuersatz == (Steuer)oldValue)
                        {
                            artikel.Steuersatz = (Steuer)newValue;
                        }
                    }
                }
                else if (propertyName == nameof(BenutzteGruppenRabatte) && BearbeitungDurchAfterConstruction == false)
                {
                    AktionsHelper2000 aktionsHelper = new AktionsHelper2000(Session);
                    aktionsHelper.UpdateAktuellenRabatt_Untergruppe(this);

                    foreach (Artikel artikel in ArtikelUntergruppeArtikelListe)
                    {
                        aktionsHelper.UpdateAktuellenRabatt_Artikel(artikel);
                        aktionsHelper.UpdateAktuellenPreis(artikel);
                    }
                }
            }
        }


        protected override void OnSaving()
        {
            base.OnSaving();
            if(IsDeleted == false && BearbeitungDurchViewController == false)
            {
                txtListenHelper txtListenHelper = new txtListenHelper();
                string[] eigenschaften_inhalt = new string[] {ArtikelGruppe.Bezeichnung };
                string[] eigenschaften_typ = new string[] { "ArtikelGruppe" };
                txtListenHelper.ÜberprüfeAufVorkommenInListeMitExtraEigenschaften(nameof(ArtikelUntergruppe), Bezeichnung, eigenschaften_typ, eigenschaften_inhalt);
            }
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            BearbeitungDurchAfterConstruction = true;

            BenutzteGruppenRabatte = true;

            BearbeitungDurchAfterConstruction = false;
        }


        #endregion

        #region Properties

        private string _Bezeichnung;
        public string Bezeichnung
        {
            get { return _Bezeichnung; }
            set { SetPropertyValue<string>(nameof(Bezeichnung), ref _Bezeichnung, value); }
        }


        private int _ArtikelUntergruppenNummer;
        [ModelDefault("AllowEdit", "false")]
        public int ArtikelUntergruppenNummer
        {
            get { return _ArtikelUntergruppenNummer; }
            set { SetPropertyValue<int>(nameof(ArtikelUntergruppenNummer), ref _ArtikelUntergruppenNummer, value); }
        }


        private ArtikelGruppe _ArtikelGruppe;
        [ImmediatePostData]
        [Association("ArtikelUntergruppe-ArtikelGruppe")]
        [RuleRequiredField]
        public ArtikelGruppe ArtikelGruppe
        {
            get { return _ArtikelGruppe; }
            set { SetPropertyValue<ArtikelGruppe>(nameof(ArtikelGruppe), ref _ArtikelGruppe, value); }
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


        private string _AngewandterRabatt;
        [ModelDefault("AllowEdit", "false")]
        public string AngewandterRabatt
        {
            get { return _AngewandterRabatt; }
            set { SetPropertyValue<string>(nameof(AngewandterRabatt), ref _AngewandterRabatt, value); }
        }


        private bool _BenutzteGruppenRabatte;
        [ImmediatePostData]
        [VisibleInListView(false)]
        public bool BenutzteGruppenRabatte
        {
            get { return _BenutzteGruppenRabatte; }
            set { SetPropertyValue<bool>(nameof(BenutzteGruppenRabatte), ref _BenutzteGruppenRabatte, value); }
        }


        #endregion

        #region Listen

        [DevExpress.Xpo.Aggregated, Association("ArtikelUntergruppe-Artikel")]
        public XPCollection<Artikel> ArtikelUntergruppeArtikelListe
        {
            get { return GetCollection<Artikel>(nameof(ArtikelUntergruppeArtikelListe)); }
        }

        [DevExpress.Xpo.Aggregated, Association("ArtikelUntergruppe-Aktionsrabatt")]
        public XPCollection<Aktionsrabatt> AktionsrabattListe
        {
            get { return GetCollection<Aktionsrabatt>(nameof(AktionsrabattListe)); }
        }


        #endregion

        #region Non Persistent Properties

        private bool _BearbeitungDurchViewController;
        [NonPersistent]
        [Browsable(false)]
        public bool BearbeitungDurchViewController
        {
            get { return _BearbeitungDurchViewController; }
            set { SetPropertyValue<bool>(nameof(BearbeitungDurchViewController), ref _BearbeitungDurchViewController, value); }
        }

        public int AnzahlDerProdukte
        {
            get { return ArtikelUntergruppeArtikelListe.Count; }
        }


        private bool _BearbeitungDurchAfterConstruction;
        [NonPersistent]
        [Browsable(false)]
        public bool BearbeitungDurchAfterConstruction
        {
            get { return _BearbeitungDurchAfterConstruction; }
            set { SetPropertyValue<bool>(nameof(BearbeitungDurchAfterConstruction), ref _BearbeitungDurchAfterConstruction, value); }
        }

        #endregion
    }
}