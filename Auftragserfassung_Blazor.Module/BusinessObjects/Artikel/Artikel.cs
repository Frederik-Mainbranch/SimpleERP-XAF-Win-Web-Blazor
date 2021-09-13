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
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ConditionalAppearance;
using DisplayNameAttribute = DevExpress.Xpo.DisplayNameAttribute;
using DevExpress.XtraEditors;
using System.Diagnostics;
using Auftragserfassung_Blazor.Module.Helpers;
using Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager;
using DevExpress.Xpo.DB;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    #region//Klasse
    [DefaultClassOptions]
    [DefaultProperty("Bezeichnung")]

    public class Artikel : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Artikel(Session session)
            : base(session)
        {
        }

        #endregion

        #region//Override Methoden

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false && BearbeitungDurchViewController == false && IsDeleted == false)
            {
                if(propertyName == nameof(ArtikelGruppe))
                {
                    ArtikelUntergruppe = null;
                    if(newValue != null)
                    {
                        if(ArtikelGruppe.Steuersatz != null)
                        {
                            Steuersatz = ArtikelGruppe.Steuersatz;
                        }
                        else
                        {
                            CriteriaOperator criteria = new BinaryOperator("IstStandard", "True");
                            Steuersatz = (Steuer)Session.FindObject(typeof(Steuer), criteria);
                        }
                    }
                    else
                    {
                        Steuersatz = null;
                    }
                }
                else if(propertyName == nameof(ArtikelUntergruppe) && newValue != null)
                {
                    if(ArtikelUntergruppe.Steuersatz != null)
                    {
                        Steuersatz = ArtikelUntergruppe.Steuersatz;
                    }
                    else
                    {
                        if (ArtikelGruppe.Steuersatz != null)
                        {
                            Steuersatz = ArtikelGruppe.Steuersatz;
                        }
                        else
                        {
                            CriteriaOperator criteria = new BinaryOperator("IstStandard", "True");
                            Steuersatz = (Steuer)Session.FindObject(typeof(Steuer), criteria);
                        }
                    }
                }
                else if(propertyName == nameof(BenutzteGruppenRabatte) || propertyName == nameof(BenutzeUntergruppenRabatte))
                {
                    if(BearbeitungDurchAfterConstruction == false)
                    {
                        AktionsHelper2000 aktionsHelper = new AktionsHelper2000(Session);
                        aktionsHelper.UpdateAktuellenRabatt_Artikel((Artikel)This);
                        aktionsHelper.UpdateAktuellenPreis((Artikel)This);
                    }
                }
            }
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            BearbeitungDurchAfterConstruction = true;

            BenutzteGruppenRabatte = true;
            BenutzeUntergruppenRabatte = true;

            BearbeitungDurchAfterConstruction = false;
        }


        protected override void OnSaving()
        {
            base.OnSaving();
            if(IsDeleted == false)
            {
                AktionsHelper2000 aktionsHelper = new AktionsHelper2000(Session);
                aktionsHelper.UpdateAktuellenRabatt_Artikel((Artikel)This);
                aktionsHelper.UpdateAktuellenPreis((Artikel)This);
                BerechneAnzahlAnVerfuegbarenArtikeln();
            }
        }

        #endregion
        //------------------------------------------------------------------------------------------
        #region//Methoden

        //public void BerechneAktuellenPreis()
        //{
        //    //Überprüfung, ob es einen Aktionspreis gibt, wenn ja, wird mit diesen weitergerechnet, wenn nein, dann der Standard Preis genommen
        //    double dummy = StandardPreis;
        //    foreach (Aktionspreis aktionspreis in AktionspreiseListe)
        //    {
        //        if (DateTime.Today >= aktionspreis.AktionStart && DateTime.Today <= aktionspreis.AktionEnde && aktionspreis.AktionPreis != -1)
        //        {
        //            dummy = aktionspreis.AktionPreis;
        //            break;
        //        }
        //    }

        //    //Überprüfung auf Aktionsrabatte

        //    //wenn kein Aktionspreis gefunden wurde, ist _AktuellerPreis noch "StandardPreis"
        //    AktuellerPreis = dummy;
        //}


        public void BerechneAnzahlAnVerfuegbarenArtikeln()
        {
            BinaryOperator bo_artikel = new BinaryOperator("Artikel", this);

            XPCollection<Lagerplatz> lagerplatzListe = new XPCollection<Lagerplatz>(PersistentCriteriaEvaluationBehavior.InTransaction, Session, bo_artikel);
            AnzahlImLager = lagerplatzListe.Sum(x => x.AnzahlDerArtikel);

            CriteriaOperator criteriaOperator_l = CriteriaOperator.And(bo_artikel, new BinaryOperator(nameof(LieferantenLieferungPosition.PositionIstVollständig), "false"));
            XPCollection<LieferantenLieferungPosition> l_posiListe = new XPCollection<LieferantenLieferungPosition>(PersistentCriteriaEvaluationBehavior.InTransaction, Session, criteriaOperator_l);
            AnzahlBestelltBeimLieferanten = l_posiListe.Sum(x => x.Liefermenge);

            CriteriaOperator criteriaOperator_b = CriteriaOperator.And(bo_artikel, new BinaryOperator(nameof(BestellungsPosition.PositionIstVollständig), "false"));
            XPCollection<BestellungsPosition> b_posiListe = new XPCollection<BestellungsPosition>(PersistentCriteriaEvaluationBehavior.InTransaction, Session, criteriaOperator_b);
            AnzahlReserviert = b_posiListe.Sum(x => x.AnzahlOffeneRestmenge);

            CriteriaOperator criteriaOperator_wb = CriteriaOperator.And(bo_artikel, new BinaryOperator(nameof(Waren_Bewegung.WareHatZielErreicht), "false"));
            XPCollection<Waren_Bewegung> waren_BewegungenListe = new XPCollection<Waren_Bewegung>(PersistentCriteriaEvaluationBehavior.InTransaction, Session, criteriaOperator_wb);
            AnzahlImVersandprozess = waren_BewegungenListe.Sum(x => x.Anzahl);

           // XPCollection<Lagerplatz> waren_BewegungenListe = new XPCollection<Waren_Bewegung>(PersistentCriteriaEvaluationBehavior.InTransaction, Session, criteriaOperator_wb);


            AnzahlVerfuegbar = AnzahlImLager - AnzahlReserviert;
            if (AnzahlVerfuegbar < 0)
            {
                AnzahlFehlmenge = AnzahlReserviert - AnzahlBestelltBeimLieferanten - AnzahlImLager;
                if(AnzahlFehlmenge < 0)
                {
                    AnzahlFehlmenge = 0;
                }
                AnzahlVerfuegbar = 0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void ReserviereArtikelNachBestellung(int anzahlZuReservieren)
        {
            LagerHelper4000 lagerHelper = new LagerHelper4000(Session, this, false);
            lagerHelper.ReserviereArtikel_Im_Lager(anzahlZuReservieren);
        }

        #region //alt

        //[Action(Caption = "Erstelle 10 Artikel mit der Session", ConfirmationMessage = "Wollen Sie wirklich 10 Artikel erstellen?", ImageName = "Attention", AutoCommit = true)]
        //public void ErstelleZehnArtikel()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        Artikel neuerArtikel = new Artikel(this.Session);
        //        neuerArtikel.Bezeichnung = neuerArtikel + "" + i;
        //        neuerArtikel.Preis = i * i;
        //    }
        //}

        //[Action()]
        //public void ÄndereAnzahlImLager()
        //{

        //}

        //[Action(Caption = "Erstelle 10 Artikel mit dem ObjectSpace", ConfirmationMessage = "Wollen Sie wirklich 10 Artikel erstellen?", ImageName = "Attention", AutoCommit = true)]
        //public void ErstelleZehnArtikel2()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        Artikel CreateObject<Artikel>()

        //    }

        //}


        //private void BestimmeAktionsStatus()
        //{
        //    foreach (Aktionspreis aktPreise in Aktionspreise)
        //    {
        //        if (DateTime.Now > aktPreise.AktionStart && DateTime.Now < aktPreise.AktionEnde)
        //        {
        //            aktPreise.AktionAktiv = true;
        //        }
        //        else
        //        {
        //            aktPreise.AktionAktiv = false;
        //        }
        //    }
        //}



        #endregion
        #endregion
        //------------------------------------------------------------------------------------------
        #region//Properties


        private string _Bezeichnung;
        //[RuleUniqueValue]
        [RuleRequiredField]
        public string Bezeichnung
        {
            get { return _Bezeichnung; }
            set { SetPropertyValue<string>(nameof(Bezeichnung), ref _Bezeichnung, value); }
        }


        private double _StandardPreis;
        [ImmediatePostData]
        [RuleRequiredField]
        public double StandardPreis
        {
            get { return _StandardPreis; }
            set { SetPropertyValue<double>(nameof(StandardPreis), ref _StandardPreis, value); }
        }


        private double _AktuellerPreis;
        [DisplayNameAttribute("Aktueller Netto Preis")]
        public double AktuellerPreis
        {
            get { return _AktuellerPreis; }
            set { SetPropertyValue<double>(nameof(AktuellerPreis), ref _AktuellerPreis, value); }
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
        public string AngewandterRabatt //String bzw. Hinweis an den User, welcher Rabatt benutzt wurde
        {
            get { return _AngewandterRabatt; }
            set { SetPropertyValue<string>(nameof(AngewandterRabatt), ref _AngewandterRabatt, value); }
        }




        //private bool _BenutzeGruppenRabatte;
        //[ImmediatePostData]
        //public bool BenutzeGruppenRabatte
        //{
        //    get { return _BenutzeGruppenRabatte; }
        //    set { SetPropertyValue<bool>(nameof(BenutzeGruppenRabatte), ref _BenutzeGruppenRabatte, value); }
        //}


        private Steuer _Steuersatz;
        public Steuer Steuersatz
        {
            get { return _Steuersatz; }
            set { SetPropertyValue<Steuer>(nameof(Steuersatz), ref _Steuersatz, value); }
        }


        private ArtikelGruppe _ArtikelGruppe;
        [ImmediatePostData]
        [RuleRequiredField]
        public ArtikelGruppe ArtikelGruppe
        {
            get { return _ArtikelGruppe; }
            set { SetPropertyValue<ArtikelGruppe>(nameof(ArtikelGruppe), ref _ArtikelGruppe, value); }
        }


        private ArtikelUntergruppe _ArtikelUntergruppe;
        [DataSourceProperty("ArtikelGruppe.ArtikelUntergruppenListe")]
        [ImmediatePostData]
        [Association("ArtikelUntergruppe-Artikel")]
        [Appearance("Sperre ArtikelUntergruppe", TargetItems = nameof(ArtikelUntergruppe), Criteria = nameof(ArtikelGruppe) + " = NULL", Enabled = false)]
        [RuleRequiredField]
        public ArtikelUntergruppe ArtikelUntergruppe
        {
            get { return _ArtikelUntergruppe; }
            set { SetPropertyValue<ArtikelUntergruppe>(nameof(ArtikelUntergruppe), ref _ArtikelUntergruppe, value); }
        }


        private bool _BenutzteGruppenRabatte;
        [ImmediatePostData]
        [VisibleInListView(false)]
        public bool BenutzteGruppenRabatte
        {
            get { return _BenutzteGruppenRabatte; }
            set { SetPropertyValue<bool>(nameof(BenutzteGruppenRabatte), ref _BenutzteGruppenRabatte, value); }
        }


        private bool _BenutzeUntergruppenRabatte;
        [ImmediatePostData]
        [VisibleInListView(false)]
        public bool BenutzeUntergruppenRabatte
        {
            get { return _BenutzeUntergruppenRabatte; }
            set { SetPropertyValue<bool>(nameof(BenutzeUntergruppenRabatte), ref _BenutzeUntergruppenRabatte, value); }
        }






        // Lager


        private int _AnzahlImLager;
        [ModelDefault("AllowEdit", "false")]
        [DisplayNameAttribute("Anzahl in allen Lagern")]
        public int AnzahlImLager
        {
            get { return _AnzahlImLager; }
            set { SetPropertyValue<int>(nameof(AnzahlImLager), ref _AnzahlImLager, value); }
        }


        private int _AnzahlReserviert;
        [ModelDefault("AllowEdit", "false")]
        [DisplayNameAttribute("Reserviert")]
        public int AnzahlReserviert
        {
            get { return _AnzahlReserviert; }
            set { SetPropertyValue<int>(nameof(AnzahlReserviert), ref _AnzahlReserviert, value); }
        }


        private int _AnzahlVerfuegbar;
        [ModelDefault("AllowEdit", "false")]
        [DisplayNameAttribute("Verfügbar")]
        public int AnzahlVerfuegbar
        {
            get { return _AnzahlVerfuegbar; }
            set { SetPropertyValue<int>(nameof(AnzahlVerfuegbar), ref _AnzahlVerfuegbar, value); }
        }


        private int _AnzahlFehlmenge;
        [ModelDefault("AllowEdit", "false")]
        [DisplayNameAttribute("Fehlmenge")]
        public int AnzahlFehlmenge
        {
            get { return _AnzahlFehlmenge; }
            set { SetPropertyValue<int>(nameof(AnzahlFehlmenge), ref _AnzahlFehlmenge, value); }
        }


        private int _AnzahlImVersandprozess;
        [ModelDefault("AllowEdit", "false")]
        [DisplayNameAttribute("Im Versandprozess")]
        public int AnzahlImVersandprozess
        {
            get { return _AnzahlImVersandprozess; }
            set { SetPropertyValue<int>(nameof(AnzahlImVersandprozess), ref _AnzahlImVersandprozess, value); }
        }



        private int _AnzahlBestelltBeimLieferanten;
        [ModelDefault("AllowEdit", "false")]
        [DisplayNameAttribute("Bestellt beim Lieferanten")]
        public int AnzahlBestelltBeimLieferanten
        {
            get { return _AnzahlBestelltBeimLieferanten; }
            set { SetPropertyValue<int>(nameof(AnzahlBestelltBeimLieferanten), ref _AnzahlBestelltBeimLieferanten, value); }
        }



        #endregion

        #region//Listen

        [DevExpress.Xpo.Aggregated, Association("Artikel-Aktionspreis")]
        [ExpandObjectMembers(ExpandObjectMembers.Always)]
        public XPCollection<Aktionspreis> AktionspreiseListe
        {
            get { return GetCollection<Aktionspreis>(nameof(AktionspreiseListe)); }
        }


        [DevExpress.Xpo.Aggregated, Association("Artikel-Aktionsrabatt")]
        [ExpandObjectMembers(ExpandObjectMembers.Always)]
        public XPCollection<Aktionsrabatt> AktionsrabatteListe
        {
            get { return GetCollection<Aktionsrabatt>(nameof(AktionsrabatteListe)); }
        }


        [Association("Lieferant-Artikel")]
        public XPCollection<Lieferant> LieferantenListe
        {
            get { return GetCollection<Lieferant>(nameof(LieferantenListe)); }
        }



        [DevExpress.Xpo.Aggregated, Association("Artikel-Lager_ArtikeLager_Zugehoerigkeit")]
        public XPCollection<Lager_ArtikeLager_Zugehoerigkeit> Lager_ArtikeLager_ZugehoerigkeitsListe
        {
            get { return GetCollection<Lager_ArtikeLager_Zugehoerigkeit>(nameof(Lager_ArtikeLager_ZugehoerigkeitsListe)); }
        }


        [DevExpress.Xpo.Aggregated, Association("Artikel-Lagerplatz")]
        public XPCollection<Lagerplatz> LagerplatzListe
        {
            get { return GetCollection<Lagerplatz>(nameof(LagerplatzListe)); }
        }





        //[DevExpress.Xpo.Aggregated, Association]
        //[ExpandObjectMembers(ExpandObjectMembers.Always)]
        //public XPCollection<AktionsSteuer> Aktionssteuern
        //{
        //    get { return GetCollection<AktionsSteuer>(nameof(Aktionssteuern)); }
        //}


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