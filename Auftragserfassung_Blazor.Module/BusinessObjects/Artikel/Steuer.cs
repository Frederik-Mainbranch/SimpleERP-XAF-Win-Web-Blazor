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
using DevExpress.XtraEditors;
using System.Diagnostics;
using Auftragserfassung_Blazor.Module.Helpers;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(AktuellerSteuersatz))]

    public class Steuer : BaseObject
    { 
        public Steuer(Session session)
            : base(session)
        {
        }

        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false)
            {
                if(propertyName == nameof(IstStandard) && IstStandard == true)
                {
                    XPCollection<Steuer> SteuerListe = new XPCollection<Steuer>(Session);

                    foreach (Steuer steuer in SteuerListe)
                    {
                        if(steuer.Oid != this.Oid && steuer.IstStandard == true)
                        {
                            steuer.IstStandard = false;
                        }
                    }
                    //UpdateArtikelSteuern();
                }
                else if(propertyName == nameof(IstSteuerfrei) && IstSteuerfrei == true && StandardSteuersatz != 0)
                {
                    StandardSteuersatz = 0;
                }
            }
        }


        public override void AfterConstruction()
        {
            XPCollection<Steuer> SteuerListe = new XPCollection<Steuer>(Session);

            if(SteuerListe.Count == 0)
            {
                IstStandard = true;
            }
        }


        protected override void OnDeleting()
        {
            base.OnDeleting();
            if(IstStandard == true)
            {
                Random rnd = new Random();
                CriteriaOperator criteria = new BinaryOperator("IstStandard", false);
                XPCollection<Steuer> SteuerListe = new XPCollection<Steuer>(Session, criteria);

                SteuerListe[rnd.Next(0, SteuerListe.Count)].IstStandard = true;
            }
        }


        protected override void OnSaving()
        {
            base.OnSaving();
            if(IsDeleted == false)
            {
                //UpdateArtikelSteuern();

                AktuellerSteuersatz = StandardSteuersatz;
                foreach (VoruebergehendeSteuer aktion in VoruebergehendeSteuerListe)
                {
                    ZeitraumHelper2 zeitraumHelper = new ZeitraumHelper2(aktion, aktion.AktionStart, aktion.AktionEnde);
                    if (zeitraumHelper.IstAktionAktiv() == true)
                    {
                        AktuellerSteuersatz = aktion.AktionsSteuer;
                        break;
                    }
                }

                #region//alt
                //XPCollection artikelListe = new XPCollection(Session, typeof(Artikel));
                //double standartSteuer = verwendeteSteuerErmittler.ErmittleStandartSteuer();

                //foreach (Artikel artikel in artikelListe)
                //{
                //    artikel.verwendeteSteuer = verwendeteSteuerErmittler.ErmittleVerwendeteSteuerMitStandartSteuer(artikel, standartSteuer);
                //}
                #endregion
            }
        }

        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Methoden ---------------------------------------------

        //private void UpdateArtikelSteuern()
        //{
        //    verwendeteSteuerErmittler verwendeteSteuerErmittler = new verwendeteSteuerErmittler(Session);
        //    verwendeteSteuerErmittler.AktualisiereFürAlleArtikel_VerwendeteSteuer();
        //}

        #region//alt

        //public double ErmittleStandartSteuer()
        //{
        //    CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("IstStandart", "True"), new NullOperator("GCRecord"));
        //    return ((Steuer)Session.FindObject(typeof(Steuer), criteria)).HöheDesSteuersatzes;
        //}

        //public double ErmittleArtikelSteuerMitStandartSteuer(Artikel artikel, double standartSteuer)
        //{
        //    if (artikel.Steuersatz != null)
        //    {
        //        if (artikel.Steuersatz.HöheDesSteuersatzes != 0 || artikel.Steuersatz.IstSteuerfrei == true)
        //        {
        //            return artikel.Steuersatz.HöheDesSteuersatzes;
        //        }
        //    }

        //    if (artikel.ArtikelUntergruppe != null)
        //    {
        //        if (artikel.ArtikelUntergruppe.Steuersatz != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.Steuersatz.HöheDesSteuersatzes != 0 || artikel.ArtikelUntergruppe.Steuersatz.IstSteuerfrei == true)
        //            {
        //                return artikel.ArtikelUntergruppe.Steuersatz.HöheDesSteuersatzes;
        //            }
        //        }
        //        else if (artikel.ArtikelUntergruppe.ArtikelGruppe != null)
        //        {
        //            if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz != null)
        //            {
        //                if (artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.HöheDesSteuersatzes != 0 || artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.IstSteuerfrei == true)
        //                {
        //                    return artikel.ArtikelUntergruppe.ArtikelGruppe.Steuersatz.HöheDesSteuersatzes;
        //                }
        //            }
        //        }
        //    }

        //    //Standart Wert der Steuer, wenn keine andere gefunden wurde
        //    return standartSteuer;
        //}

        //public double ErmittleArtikelSteuerOhneStandartSteuer(Artikel artikel)
        //{
        //    double standartSteuer = ErmittleStandartSteuer();
        //    return ErmittleArtikelSteuerMitStandartSteuer(artikel, standartSteuer);
        //}
        #endregion
        //-------------------------------- Methoden ---------------------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private string _Bezeichnung;
        public string Bezeichnung
        {
            get { return _Bezeichnung; }
            set { SetPropertyValue<string>(nameof(Bezeichnung), ref _Bezeichnung, value); }
        }


        private double _StandartSteuersatz;
        [Appearance("Sperre StandartSteuersatz", TargetItems = nameof(StandardSteuersatz), Criteria = nameof(IstSteuerfrei) + " == true", Enabled = false)]
        public double StandardSteuersatz
        {
            get { return _StandartSteuersatz; }
            set { SetPropertyValue<double>(nameof(StandardSteuersatz), ref _StandartSteuersatz, value); }
        }

        private double _AktuellerSteuersatz;
        [ModelDefault("AllowEdit", "false")]
        public double AktuellerSteuersatz
        {
            get { return _AktuellerSteuersatz; }
            set { SetPropertyValue<double>(nameof(AktuellerSteuersatz), ref _AktuellerSteuersatz, value); }
        }


        private bool _IstStandart;
        [ImmediatePostData]
        [Appearance("Sperre IstStandart", TargetItems = nameof(IstStandard), Criteria = nameof(IstStandard) + " = true", Enabled = false)]
        public bool IstStandard
        {
            get { return _IstStandart; }
            set { SetPropertyValue<bool>(nameof(IstStandard), ref _IstStandart, value); }
        }


        private bool _IstSteuerfrei;
        [ImmediatePostData]
        public bool IstSteuerfrei
        {
            get { return _IstSteuerfrei; }
            set { SetPropertyValue<bool>(nameof(IstSteuerfrei), ref _IstSteuerfrei, value); }
        }


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------


        [DevExpress.Xpo.Aggregated, Association("Steuer-VoruebergehendeSteuer")]
        public XPCollection<VoruebergehendeSteuer> VoruebergehendeSteuerListe
        {
            get { return GetCollection<VoruebergehendeSteuer>(nameof(VoruebergehendeSteuerListe)); }
        }


        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------



        //-------------------------------- Non Persistent Properties ---------------------------------------------
        #region//alt
        //private ArtikelGruppe _ArtikelGruppe;
        //[Association("Steuersatz-ArtikelGruppe")]
        //public ArtikelGruppe ArtikelGruppe
        //{
        //    get { return _ArtikelGruppe; }
        //    set { SetPropertyValue<ArtikelGruppe>(nameof(ArtikelGruppe), ref _ArtikelGruppe, value); }
        //}


        //private ArtikelUntergruppe _ArtikelUntergruppe;
        //public ArtikelUntergruppe ArtikelUntergruppe
        //{
        //    get { return _ArtikelUntergruppe; }
        //    set { SetPropertyValue<ArtikelUntergruppe>(nameof(ArtikelUntergruppe), ref _ArtikelUntergruppe, value); }
        //}


        //private Artikel _Artikel;
        //public Artikel Artikel
        //{
        //    get { return _Artikel; }
        //    set { SetPropertyValue<Artikel>(nameof(Artikel), ref _Artikel, value); }
        //}
        #endregion
    }
}